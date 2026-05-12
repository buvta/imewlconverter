# Code Review: refactor 分支代码审查报告

**审查日期**: 2026-05-12  
**审查范围**: `e77bdce..31836a2`（7 个提交，301 个文件，+16,984 / -20,270 行）  
**分支**: `refactor`

---

## 概述

本次重构将项目从单体结构转变为分层架构，三端（CLI、WinForms、macOS GUI）共用统一的 `ConversionPipeline` 转换引擎。整体架构设计优秀，但存在 2 个严重问题需要在合并前修复。

---

## 优点

1. **清晰的分层架构** — Abstractions → Core → Formats → CLI/GUI，依赖方向正确，内层不引用外层
2. **Source Generator 格式注册** — 通过 `[FormatPlugin]` 属性自动生成 DI 注册代码，彻底消除运行时反射，支持 AOT/Trimming
3. **统一 `ConversionPipeline`** — 三端共享同一个 7 步管道（Import→Filter→ChineseConvert→WordRank→CodeGen→RemoveEmpty→Export）
4. **现代 C# 模式全面应用** — `sealed record` 不可变模型、`required` 属性、`[GeneratedRegex]`、`IProgress<T>`、`CancellationToken`
5. **简洁的 DI 注册** — `services.AddAllFormats()` + `services.AddImeWlConverterCore()` 即可完成全部注册
6. **零编译警告**，测试通过，lint 通过

---

## 问题列表

### 严重（必须修复）

#### 1. Singleton `ConversionPipeline` 存在线程安全 / 状态泄漏风险

- **文件**: `src/ImeWlConverter.Core/ServiceCollectionExtensions.cs:22-23`
- **问题**: Pipeline 以 Singleton 注册，但 `SelfDefiningImporter` 等 Importer 有可变属性（`OrderSpec`、`PinyinSeparator` 等）。在 WinForms 中如果用户在两次转换之间修改了 SelfDefining 格式配置，后续所有转换都会共享被修改的 Importer 实例。
- **影响**: 顺序转换或并发取消重转时可能产出错误结果。
- **建议修复**: 将 `ConversionPipeline` 注册为 Transient/Scoped；或让 Importer 无状态化（配置通过 `ImportOptions` 传入而非实例属性）。

#### 2. WinForms: `streamExport` + `mergeTo1File` 冲突导致文件未写入

- **文件**: `src/IME WL Converter Win/Forms/MainForm.cs:301-310`
- **问题**: 当 `streamExport=true` 且 `mergeTo1File=true` 时，代码同时设置了 `OutputPath` 和 `OutputStream`。在 `ConversionPipeline.ExecuteMergedAsync:143-155` 中，`OutputStream` 优先检查，导致用户在保存对话框中选择的文件路径永远不会被写入。
- **影响**: 数据丢失 — 用户以为文件已保存但实际未写入。
- **建议修复**: 当 `streamExport=true` 时，不设置 `OutputStream`（或设为 null），仅设置 `OutputPath`。

---

### 重要（应该修复）

#### 3. DI 注入的 FilterPipeline 导致 GUI 过滤配置被静默忽略

- **文件**: `src/ImeWlConverter.Core/ServiceCollectionExtensions.cs:25` + `src/ImeWlConverter.Core/Pipeline/ConversionPipeline.cs:26-39`
- **问题**: DI 注册了一个空的 `FilterPipeline` 单例，并注入到 `ConversionPipeline` 构造函数。由于 `_externalFilterPipeline` 始终非 null，`BuildFilterPipeline(request.FilterConfig)` 永远不会被调用。
- **影响**: 在 WinForms 和 macOS GUI 中，用户通过过滤配置对话框设置的所有过滤条件被**静默忽略**。
- **建议修复**: 移除 `services.AddSingleton<FilterPipeline>()` 注册；或不在 `ConversionPipeline` 构造函数中注入 `FilterPipeline`，让管道始终从 `request.FilterConfig` 构建。

#### 4. 22 个测试被跳过，代表功能回退

- **文件**: `SelfDefiningTest.cs`（7 个）、`SougouPinyinScelExportTest.cs`（2 个）、`LlmWordRankGeneraterTest.cs`（5 个）、`Win10MsPinyinSelfStudyTest.cs`（1 个）等
- **问题**: SelfDefining ParsePattern 支持、SougouScel 导出、LLM 词频生成、Win10MsPinyin 自学习导出等功能迁移不完整。
- **影响**: 依赖这些功能的用户会发现功能缺失。SelfDefining 格式尤为关键 — 它是高级用户最灵活的格式。
- **建议**: 至少在发布说明中记录为已知缺失；理想情况下在发布前完成 SelfDefining ParsePattern 迁移。

#### 5. `RankPercentageFilter` 排序方向反转

- **文件**: `src/ImeWlConverter.Core/Filters/RankPercentageFilter.cs:13`
- **问题**: `entries.OrderBy(e => e.Rank).Take(count)` 保留 Rank 最低的词条。但项目约定 "Rank 越高 = 词频越高"，所以实际保留的是**最不常用**的词。
- **建议修复**: 改为 `OrderByDescending(e => e.Rank)`。

#### 6. CLI 绕过 DI 直接构造 Pipeline

- **文件**: `src/ImeWlConverterCmd/CommandBuilder.cs:188-194`
- **问题**: CLI 使用 `new ConversionPipeline(...)` 手动构造，绕过了 DI 注册的 `IConversionPipeline`。虽然 CLI 因此避开了问题 #3，但造成了三端行为不一致。
- **建议修复**: 修复问题 #3 后，CLI 也使用 `sp.GetRequiredService<IConversionPipeline>()`。

#### 7. `ConversionPipeline` 本身无单元测试

- **问题**: 只有 `FilterPipelineTest.cs`（3 个测试）。核心的 7 步管道编排逻辑（错误累积、简繁转换应用、编码生成跳过、流/文件输出）完全没有直接测试覆盖。
- **建议**: 补充测试覆盖至少：成功合并导出、逐文件导出、流输出、文件输出、过滤应用、错误累积。

#### 8. Formats 项目依赖 Core 项目，违反架构声明

- **文件**: `src/ImeWlConverter.Formats/ImeWlConverter.Formats.csproj:10`
- **问题**: 按架构图，Formats 应仅依赖 Abstractions。当前依赖 Core 是因为部分二进制格式需要 Core 的工具类（SharpZipLib 封装、PinyinDictionary 等）。
- **影响**: 如果 Core 未来需要引用 Formats 层类型会产生循环依赖；也使 Formats 层比必要的更重。
- **建议**: 长期将共享工具（SharpZipLib 封装等）下沉到 Abstractions 扩展包或独立 Helpers 包。

---

### 次要（可以改进）

| # | 文件 | 问题 | 建议 |
|---|------|------|------|
| 9 | `ImeWlConverter.Formats.csproj` | 重复引用 `SharpZipLib` 和 `UTF.Unknown`（Core 已引用，传递依赖足够） | 移除 Formats 项目中的重复引用 |
| 10 | `Abstractions/Options/FilterConfig.cs` | 使用可变 `{ get; set; }` 而非 `sealed record` / `init` | 改为 record 或 init 属性，配合 `with` 表达式 |
| 11 | `Abstractions/Options/ConversionOptions.cs:24-35` | `ChineseConversionMode` 枚举定义在类文件内，其他枚举都有独立文件 | 移至 `Enums/ChineseConversionMode.cs` |
| 12 | `ImeWlConverterCmd/CommandBuilder.cs:332` | `Console.Error.Write($"\r{value.Message}")` 短消息无法覆盖长消息残留字符 | 使用 `$"\r{value.Message,-80}"` 填充空格 |
| 13 | `Formats/SelfDefining/SelfDefiningImporter.cs:12` | 继承 `BinaryFormatImporter` 但实际处理文本（内部用 StreamReader） | 改为继承 `TextFormatImporter` |
| 14 | `ImeWlConverterMac/ViewModels/MainWindowViewModel.cs:619-637` | `RelayCommand` 基础设施类混在 ViewModel 文件中 | 拆分到独立文件 |
| 15 | WinForms `MainForm.cs` vs macOS `MainWindowViewModel.cs` | `AutoMatchImportType` 中使用的格式 ID 不一致（如 `"sougou_bin"` vs `"sougouBin"`） | 审计所有 `[FormatPlugin("id")]` 确保 ID 一致匹配 |

---

## 建议优先级

1. **立即修复** 问题 #2（数据丢失）和 #3（过滤静默失效）— 这两个是用户可感知的严重 bug
2. **尽快修复** 问题 #1（Singleton 状态泄漏）和 #5（排序反转）
3. **合并前** 确认问题 #4 中功能缺失是否可接受，并在发布说明中说明
4. **后续迭代** 处理问题 #6-#8 和次要问题

---

## 最终评估

| 维度 | 评价 |
|------|------|
| 架构设计 | ⭐⭐⭐⭐⭐ 优秀 |
| 代码质量 | ⭐⭐⭐⭐ 良好 |
| 测试覆盖 | ⭐⭐⭐ 一般（核心管道缺测试） |
| 生产就绪 | ❌ 需修复 #2 和 #3 后方可合并 |

**结论**: 架构设计出色，成功实现了三端统一转换引擎的目标。但 DI 注册缺陷导致 GUI 过滤功能失效（#3），以及 WinForms 数据丢失路径（#2），必须在合并前修复。修复这两个问题后，本分支可以合并，其余重要问题可作为后续工作跟踪。
