/*
 *   Copyright © 2022 yfdyh000

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

using System;
using Xunit;
using Studyzy.IMEWLConverter.Entities;
using Studyzy.IMEWLConverter.IME;

namespace Studyzy.IMEWLConverter.Test;

public class QQPinyinQcelTest : BaseTest
{
    public QQPinyinQcelTest()
    {
        importer = new QQPinyinQcel();
    }

    protected override string StringData => throw new NotImplementedException();

    [Fact]
    public void TestImportLine()
    {
        Assert.ThrowsAny<Exception>(
            () => { importer.ImportLine("test"); }
        );
    }

    [Theory]
    [InlineData("星际战甲.qcel")]
    public void TestImportQcelWithAlphabet(string filePath)
    {
        var lib = importer.Import(GetFullPath(filePath));
        Assert.True(lib.Count > 0);

        Assert.Equal(4675, lib.Count);
        Assert.Equal(CodeType.Pinyin, lib[0].CodeType);
        Assert.Equal("a'ka'ta", lib[2].PinYinString);
        Assert.Equal("a'ka'ta'r'i'v'wai'guan", lib[3].PinYinString);
        Assert.Equal(0, lib[0].Rank);
        Assert.Equal("zuo", lib[4670].SingleCode);
        Assert.Equal("阿卡塔", lib[2].Word);
        Assert.Equal("阿卡塔riv外观", lib[3].Word);
    }

    [Theory]
    [InlineData("星际战甲.qcel")]
    public void TestListQcelInfo(string filePath)
    {
        var info = QQPinyinQcel.ReadQcelInfo(GetFullPath(filePath));
        Assert.NotNull(info);
        Assert.NotEmpty(info);

        Assert.Equal("4675", info["CountWord"]);
        Assert.Equal("星际战甲warframe国际服", info["Name"]);
        Assert.Equal("射击游戏", info["Type"]);
        Assert.Contains("词条来源是灰机wiki-warframe中文维基的中英文对照表", info["Info"]);
        Assert.Contains("肿瘤 三叶坚韧 狂风猛踢 寒冰之力", info["Sample"]);
    }
}
