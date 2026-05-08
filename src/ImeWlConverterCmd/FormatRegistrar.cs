using System.Collections.Generic;
using Studyzy.IMEWLConverter.IME;

namespace Studyzy.IMEWLConverter;

/// <summary>
/// Registers all known IME formats without reflection.
/// </summary>
internal static class FormatRegistrar
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
}
