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
using Studyzy.IMEWLConverter.Filters;

namespace Studyzy.IMEWLConverter.Test.FilterTest;

public class AllFilterTest
{
    [Theory]
    [InlineData("123.456", true)]
    [InlineData("abc!efg?", true)]
    [InlineData("1《深蓝词库转换》", false)]
    [InlineData("2大家，好", false)]
    [InlineData("3转换成功。", false)]
    public void ChinesePunctuationFilterTest(string word, bool isKeep)
    {
        var wl = new WordLibrary();
        wl.Word = word;
        var filter = new ChinesePunctuationFilter();
        Assert.Equal(isKeep, filter.IsKeep(wl));
    }

    [Theory]
    [InlineData("深蓝", true)]
    [InlineData("深 蓝", false)]
    [InlineData(" 深蓝", false)]
    [InlineData("深蓝 ", false)]
    public void SpaceFilterTest(string word, bool isKeep)
    {
        var wl = new WordLibrary();
        wl.Word = word;
        var filter = new SpaceFilter();
        Assert.Equal(isKeep, filter.IsKeep(wl));
    }
}
