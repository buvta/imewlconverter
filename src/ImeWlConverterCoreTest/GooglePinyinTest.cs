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

using System;
using Xunit;
using Studyzy.IMEWLConverter.IME;

namespace Studyzy.IMEWLConverter.Test;

public class GooglePinyinTest : BaseTest
{
    public GooglePinyinTest()
    {
        exporter = new GooglePinyin();
        importer = new GooglePinyin();
    }

    protected override string StringData => Resource4Test.GooglePinyin;

    [Fact]
    public void TestExport()
    {
        var txt = exporter.Export(WlListData)[0];
        Assert.True(txt.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Length == 2);
    }

    [Fact]
    public void TestExportLine()
    {
        var txt = exporter.ExportLine(WlData);
        Assert.Equal("深蓝测试\t10\tshen lan ce shi", txt);
    }

    [Fact]
    public void TestImport()
    {
        var list = ((IWordLibraryTextImport)importer).ImportText(StringData);
        Assert.NotNull(list);
        Assert.Equal(10, list.Count);
    }
}
