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

using System.Linq;
using Xunit;
using Studyzy.IMEWLConverter.Generaters;

namespace Studyzy.IMEWLConverter.Test.GeneraterTest;

public class PinyinTest
{
    private readonly IWordCodeGenerater generater;

    public PinyinTest()
    {
        generater = new PinyinGenerater();
    }

    [Fact]
    public void TestGetOneWordPinyin()
    {
    }

    [Theory]
    [InlineData("曾毅", "zeng yi")]
    [InlineData("音乐", "yin yue")]
    [InlineData("快乐", "kuai le")]
    [InlineData("银行", "yin hang")]
    [InlineData("行走", "xing zou")]
    [InlineData("〇〇七", "ling ling qi")]
    public void TestGetLongWordsPinyin(string str, string py)
    {
        var result = generater.GetCodeOfString(str);
        Assert.Contains(py, result.ToCodeString(" ").ToArray());
    }
}
