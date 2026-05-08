using System.Collections.Generic;
using System.Linq;
using Studyzy.IMEWLConverter.IME;

namespace Studyzy.IMEWLConverter;

/// <summary>
/// Registers all known IME formats without reflection.
/// </summary>
public static class FormatRegistrar
{
    public static (Dictionary<string, IWordLibraryImport> imports, Dictionary<string, IWordLibraryExport> exports, Dictionary<string, string> names)
        RegisterAll()
    {
        var imports = new Dictionary<string, IWordLibraryImport>();
        var exports = new Dictionary<string, IWordLibraryExport>();
        var names = new Dictionary<string, string>();

        // Import + Export
        Register(imports, exports, names, ConstantString.SOUGOU_PINYIN_C, ConstantString.SOUGOU_PINYIN, new SougouPinyin());
        Register(imports, exports, names, ConstantString.SOUGOU_XIBAO_SCEL_C, ConstantString.SOUGOU_XIBAO_SCEL, new SougouPinyinScel());
        Register(imports, exports, names, ConstantString.BAIDU_PINYIN_C, ConstantString.BAIDU_PINYIN, new BaiduPinyin());
        Register(imports, exports, names, ConstantString.BAIDU_SHOUJI_C, ConstantString.BAIDU_SHOUJI, new BaiduShouji());
        Register(imports, exports, names, ConstantString.BAIDU_SHOUJI_ENG_C, ConstantString.BAIDU_SHOUJI_ENG, new BaiduShoujiEng());
        Register(imports, exports, names, ConstantString.GOOGLE_PINYIN_C, ConstantString.GOOGLE_PINYIN, new GooglePinyin());
        Register(imports, exports, names, ConstantString.GBOARD_C, ConstantString.GBOARD, new Gboard());
        Register(imports, exports, names, ConstantString.RIME_C, ConstantString.RIME, new Rime());
        Register(imports, exports, names, ConstantString.FIT_C, ConstantString.FIT, new FitInput());
        Register(imports, exports, names, ConstantString.MS_PINYIN_C, ConstantString.MS_PINYIN, new MsPinyin());
        Register(imports, exports, names, ConstantString.WIN10_MS_PINYIN_C, ConstantString.WIN10_MS_PINYIN, new Win10MsPinyin());
        Register(imports, exports, names, ConstantString.WIN10_MS_WUBI_C, ConstantString.WIN10_MS_WUBI, new Win10MsWubi());
        Register(imports, exports, names, ConstantString.WIN10_MS_PINYIN_SELF_STUDY_C, ConstantString.WIN10_MS_PINYIN_SELF_STUDY, new Win10MsPinyinSelfStudy());
        Register(imports, exports, names, ConstantString.SINA_PINYIN_C, ConstantString.SINA_PINYIN, new SinaPinyin());
        Register(imports, exports, names, ConstantString.SHOUXIN_PINYIN_C, ConstantString.SHOUXIN_PINYIN, new ShouxinPinyin());
        Register(imports, exports, names, ConstantString.CHINESE_PYIM_C, ConstantString.CHINESE_PYIM, new ChinesePyim());
        Register(imports, exports, names, ConstantString.CANGJIE_PLATFORM_C, ConstantString.CANGJIE_PLATFORM, new CangjiePlatform());
        Register(imports, exports, names, ConstantString.CHAO_YIN_C, ConstantString.CHAO_YIN, new Chaoyin());
        Register(imports, exports, names, ConstantString.YAHOO_KEYKEY_C, ConstantString.YAHOO_KEYKEY, new YahooKeyKey());
        Register(imports, exports, names, ConstantString.QQ_SHOUJI_C, ConstantString.QQ_SHOUJI, new QQShouji());
        Register(imports, exports, names, ConstantString.WORD_ONLY_C, ConstantString.WORD_ONLY, new NoPinyinWordOnly());
        Register(imports, exports, names, ConstantString.JIDIAN_C, ConstantString.JIDIAN, new Jidian());
        Register(imports, exports, names, ConstantString.WUBI86_C, ConstantString.WUBI86, new Wubi86());
        Register(imports, exports, names, ConstantString.WUBI98_C, ConstantString.WUBI98, new Wubi98());
        Register(imports, exports, names, ConstantString.WUBI_NEWAGE_C, ConstantString.WUBI_NEWAGE, new WubiNewAge());
        Register(imports, exports, names, ConstantString.XIAOYA_WUBI_C, ConstantString.XIAOYA_WUBI, new XiaoyaWubi());
        Register(imports, exports, names, ConstantString.IFLY_IME_C, ConstantString.IFLY_IME, new iFlyIME());
        Register(imports, exports, names, ConstantString.PINYIN_JIAJIA_C, ConstantString.PINYIN_JIAJIA, new PinyinJiaJia());
        Register(imports, exports, names, ConstantString.ZIGUANG_PINYIN_C, ConstantString.ZIGUANG_PINYIN, new ZiGuangPinyin());

        // Import only
        Register(imports, exports, names, ConstantString.SOUGOU_PINYIN_BIN_C, ConstantString.SOUGOU_PINYIN_BIN, new SougouPinyinBinFromPython());
        Register(imports, exports, names, ConstantString.BAIDU_PINYIN_BACKUP_C, ConstantString.BAIDU_PINYIN_BACKUP, new BaiduPinyinBackup());
        Register(imports, exports, names, ConstantString.BAIDU_BDICT_C, ConstantString.BAIDU_BDICT, new BaiduPinyinBdict());
        Register(imports, exports, names, ConstantString.BAIDU_BCD_C, ConstantString.BAIDU_BCD, new BaiduShoujiBcd());
        Register(imports, exports, names, ConstantString.QQ_PINYIN_QPYD_C, ConstantString.QQ_PINYIN_QPYD, new QQPinyinQpyd());
        Register(imports, exports, names, ConstantString.QQ_PINYIN_QCEL_C, ConstantString.QQ_PINYIN_QCEL, new QQPinyinQcel());
        Register(imports, exports, names, ConstantString.LINGOES_LD2_C, ConstantString.LINGOES_LD2, new LingoesLd2());
        Register(imports, exports, names, ConstantString.RIME_USERDB_C, ConstantString.RIME_USERDB, new RimeUserDb());
        Register(imports, exports, names, ConstantString.ZIGUANG_PINYIN_UWL_C, ConstantString.ZIGUANG_PINYIN_UWL, new ZiGuangPinyinUwl());
        Register(imports, exports, names, ConstantString.JIDIAN_MBDICT_C, ConstantString.JIDIAN_MBDICT, new Jidian_MBDict());
        Register(imports, exports, names, ConstantString.EMOJI_C, ConstantString.EMOJI, new Emoji());

        // Export only
        Register(imports, exports, names, ConstantString.QQ_PINYIN_C, ConstantString.QQ_PINYIN, new QQPinyin());
        Register(imports, exports, names, ConstantString.QQ_PINYIN_ENG_C, ConstantString.QQ_PINYIN_ENG, new QQPinyinEng());
        Register(imports, exports, names, ConstantString.QQ_WUBI_C, ConstantString.QQ_WUBI, new QQWubi());
        Register(imports, exports, names, ConstantString.BING_PINYIN_C, ConstantString.BING_PINYIN, new BingPinyin());
        Register(imports, exports, names, ConstantString.LIBPINYIN_C, ConstantString.LIBPINYIN, new Libpinyin());
        Register(imports, exports, names, ConstantString.MAC_PLIST_C, ConstantString.MAC_PLIST, new MacPlist());
        Register(imports, exports, names, ConstantString.XIAOXIAO_C, ConstantString.XIAOXIAO, new Xiaoxiao());
        Register(imports, exports, names, ConstantString.XIAOXIAO_ERBI_C, ConstantString.XIAOXIAO_ERBI, new XiaoxiaoErbi());
        Register(imports, exports, names, ConstantString.JIDIAN_ZHENGMA_C, ConstantString.JIDIAN_ZHENGMA, new JidianZhengma());
        Register(imports, exports, names, ConstantString.SELF_DEFINING_C, ConstantString.SELF_DEFINING, new SelfDefining());
        Register(imports, exports, names, ConstantString.USER_PHRASE_C, ConstantString.USER_PHRASE, new UserDefinePhrase());
        Register(imports, exports, names, ConstantString.LIBIME_TEXT_C, ConstantString.LIBIME_TEXT, new LibIMEText());

        return (imports, exports, names);
    }

    /// <summary>
    /// Registers all formats keyed by display name with sort index for GUI usage.
    /// </summary>
    public static (Dictionary<string, IWordLibraryImport> imports, Dictionary<string, IWordLibraryExport> exports, List<string> sortedImportNames, List<string> sortedExportNames)
        RegisterAllForGui()
    {
        var imports = new Dictionary<string, IWordLibraryImport>();
        var exports = new Dictionary<string, IWordLibraryExport>();
        var importItems = new List<(string name, int index)>();
        var exportItems = new List<(string name, int index)>();

        // Import + Export
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.SOUGOU_PINYIN, 10, new SougouPinyin());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.SOUGOU_XIBAO_SCEL, 20, new SougouPinyinScel());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.BAIDU_PINYIN, 90, new BaiduPinyin());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.BAIDU_SHOUJI, 1000, new BaiduShouji());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.BAIDU_SHOUJI_ENG, 1010, new BaiduShoujiEng());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.GOOGLE_PINYIN, 110, new GooglePinyin());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.GBOARD, 111, new Gboard());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.RIME, 150, new Rime());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.FIT, 140, new FitInput());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.MS_PINYIN, 135, new MsPinyin());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.WIN10_MS_PINYIN, 130, new Win10MsPinyin());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.WIN10_MS_WUBI, 131, new Win10MsWubi());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.WIN10_MS_PINYIN_SELF_STUDY, 131, new Win10MsPinyinSelfStudy());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.SINA_PINYIN, 180, new SinaPinyin());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.SHOUXIN_PINYIN, 180, new ShouxinPinyin());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.CHINESE_PYIM, 177, new ChinesePyim());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.CANGJIE_PLATFORM, 230, new CangjiePlatform());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.CHAO_YIN, 190, new Chaoyin());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.YAHOO_KEYKEY, 200, new YahooKeyKey());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.QQ_SHOUJI, 1030, new QQShouji());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.WORD_ONLY, 2010, new NoPinyinWordOnly());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.JIDIAN, 190, new Jidian());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.WUBI86, 210, new Wubi86());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.WUBI98, 220, new Wubi98());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.WUBI_NEWAGE, 221, new WubiNewAge());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.XIAOYA_WUBI, 191, new XiaoyaWubi());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.IFLY_IME, 1050, new iFlyIME());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.PINYIN_JIAJIA, 120, new PinyinJiaJia());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.ZIGUANG_PINYIN, 170, new ZiGuangPinyin());

        // Import only
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.SOUGOU_PINYIN_BIN, 30, new SougouPinyinBinFromPython());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.BAIDU_PINYIN_BACKUP, 20, new BaiduPinyinBackup());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.BAIDU_BDICT, 100, new BaiduPinyinBdict());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.BAIDU_BCD, 1020, new BaiduShoujiBcd());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.QQ_PINYIN_QPYD, 60, new QQPinyinQpyd());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.QQ_PINYIN_QCEL, 60, new QQPinyinQcel());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.LINGOES_LD2, 200, new LingoesLd2());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.RIME_USERDB, 150, new RimeUserDb());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.ZIGUANG_PINYIN_UWL, 171, new ZiGuangPinyinUwl());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.JIDIAN_MBDICT, 190, new Jidian_MBDict());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.EMOJI, 999, new Emoji());

        // Export only
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.QQ_PINYIN, 50, new QQPinyin());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.QQ_PINYIN_ENG, 80, new QQPinyinEng());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.QQ_WUBI, 70, new QQWubi());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.BING_PINYIN, 135, new BingPinyin());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.LIBPINYIN, 175, new Libpinyin());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.MAC_PLIST, 150, new MacPlist());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.XIAOXIAO, 100, new Xiaoxiao());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.XIAOXIAO_ERBI, 100, new XiaoxiaoErbi());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.JIDIAN_ZHENGMA, 190, new JidianZhengma());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.SELF_DEFINING, 2000, new SelfDefining());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.USER_PHRASE, 110, new UserDefinePhrase());
        RegisterGui(imports, exports, importItems, exportItems, ConstantString.LIBIME_TEXT, 500, new LibIMEText());

        var sortedImportNames = importItems.OrderBy(x => x.index).Select(x => x.name).ToList();
        var sortedExportNames = exportItems.OrderBy(x => x.index).Select(x => x.name).ToList();

        return (imports, exports, sortedImportNames, sortedExportNames);
    }

    private static void Register<T>(
        Dictionary<string, IWordLibraryImport> imports,
        Dictionary<string, IWordLibraryExport> exports,
        Dictionary<string, string> names,
        string code,
        string displayName,
        T instance)
    {
        names[code] = displayName;
        if (instance is IWordLibraryImport importer)
            imports[code] = importer;
        if (instance is IWordLibraryExport exporter)
            exports[code] = exporter;
    }

    private static void RegisterGui<T>(
        Dictionary<string, IWordLibraryImport> imports,
        Dictionary<string, IWordLibraryExport> exports,
        List<(string name, int index)> importItems,
        List<(string name, int index)> exportItems,
        string displayName,
        int index,
        T instance)
    {
        if (instance is IWordLibraryImport importer)
        {
            imports[displayName] = importer;
            importItems.Add((displayName, index));
        }
        if (instance is IWordLibraryExport exporter)
        {
            exports[displayName] = exporter;
            exportItems.Add((displayName, index));
        }
    }
}
