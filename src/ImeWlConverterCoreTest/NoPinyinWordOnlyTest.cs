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

using System.Text;
using Xunit;
using Studyzy.IMEWLConverter.IME;

namespace Studyzy.IMEWLConverter.Test;

public class NoPinyinWordOnlyTest : BaseTest
{
    public NoPinyinWordOnlyTest()
    {
        importer = new NoPinyinWordOnly();
        exporter = new NoPinyinWordOnly();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    protected override string StringData => Resource4Test.NoPinyinWordOnly;

    [Fact]
    public void TestExport()
    {
        var txt = exporter.Export(WlListData)[0];
        Assert.Equal("深蓝测试\r\n词库转换\r\n", txt);
    }

    [Fact]
    public void TestExportLine()
    {
        var txt = exporter.ExportLine(WlData);
        Assert.Equal("深蓝测试", txt);
    }

    [Fact]
    public void TestImport()
    {
        var wl = ((IWordLibraryTextImport)importer).ImportText(StringData);
        Assert.Equal(10, wl.Count);
    }

    [Fact]
    public void TestImportFile()
    {
        var wll = importer.Import(GetFullPath("纯汉字.txt"));
        Assert.True(wll.Count > 0);
    }
}
