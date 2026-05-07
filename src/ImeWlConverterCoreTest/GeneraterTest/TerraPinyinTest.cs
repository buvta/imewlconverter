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
using Studyzy.IMEWLConverter.Entities;
using Studyzy.IMEWLConverter.Generaters;

namespace Studyzy.IMEWLConverter.Test.GeneraterTest;

public class TerraPinyinTest
{
    private readonly IWordCodeGenerater generater = new TerraPinyinGenerater();

    [Fact]
    public void TestPinyin2TerraPinyin()
    {
        var wl = new WordLibrary
        {
            Word = "深蓝",
            Rank = 123,
            PinYin = new[] { "shen", "lan" },
            CodeType = CodeType.Pinyin
        };
        generater.GetCodeOfWordLibrary(wl);
    }

    [Theory]
    [InlineData("曾经", "ceng2 jin1")]
    [InlineData("曾毅", "zeng1 yi4")]
    [InlineData("音乐", "yin1 yue4")]
    [InlineData("快乐", "kuai4 le4")]
    public void TestChar2TerraPinyin(string word, string pinyin)
    {
        var wl = new WordLibrary
        {
            Word = word,
            Rank = 123,
            CodeType = CodeType.NoCode
        };
        generater.GetCodeOfWordLibrary(wl);
    }
}
