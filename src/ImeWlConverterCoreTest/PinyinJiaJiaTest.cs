/*
 *   Copyright © 2009-2020 studyzy(深蓝,曾毅)

 *   This program "IME WL Converter(深蓝词库转换)" is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.

 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.

 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using Xunit;
using Studyzy.IMEWLConverter.IME;

namespace Studyzy.IMEWLConverter.Test;

public class PinyinJiaJiaTest : BaseTest
{
    public PinyinJiaJiaTest()
    {
        importer = new PinyinJiaJia();
        exporter = new PinyinJiaJia();
    }

    protected override string StringData => Resource4Test.PinyinJiajia;

    [Fact]
    public void ExportLine()
    {
        var txt = exporter.ExportLine(WlData);
        Assert.Equal("深shen蓝lan测ce试shi", txt);
    }

    [Fact]
    public void ImportNoPinyin()
    {
        var wl = importer.ImportLine("深蓝测试");
        Assert.Equal(1, wl.Count);
        Assert.Equal("shen'lan'ce'shi", wl[0].PinYinString);
    }

    [Fact]
    public void ImportWithPinyinFull()
    {
        var wl = importer.ImportLine("深shen蓝lan居ju");
        Assert.Equal(1, wl.Count);
        Assert.Equal("shen'lan'ju", wl[0].PinYinString);
        Assert.Equal("深蓝居", wl[0].Word);
    }

    [Fact]
    public void ImportWithPinyinPart()
    {
        var wl = ((IWordLibraryTextImport)importer).ImportText(StringData);
        Assert.True(wl.Count >= 8);
        Assert.Equal("ren'min'hen'xing", wl[0].PinYinString);
        Assert.Equal("人民很行", wl[0].Word);
        Assert.Equal("ren'min'yin'hang", wl[1].PinYinString);
        Assert.Equal("人民银行", wl[1].Word);
        Assert.Equal("dong'li'wu'xian", wl[2].PinYinString);
        Assert.Equal("栋力无限", wl[2].Word);
    }
}
