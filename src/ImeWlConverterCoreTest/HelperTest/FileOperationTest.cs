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
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;
using Studyzy.IMEWLConverter.Helpers;

namespace Studyzy.IMEWLConverter.Test.HelperTest;

public class FileOperationTest
{
    [Theory]
    [InlineData("Test/u8nobomzy.txt", "UTF-8")]
    [InlineData("Test/luna_pinyin_export.txt", "UTF-8")]
    [InlineData("Test/gbzy.txt", "GB18030")]
    [InlineData("Test/QQPinyin.txt", "Unicode")]
    public void TestGetFileEncoding(string path, string encoding)
    {
        path = GetFullPath(path);
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var e = FileOperationHelper.GetEncodingType(path);
        Assert.Equal(Encoding.GetEncoding(encoding).EncodingName, e.EncodingName);
        var txt = FileOperationHelper.ReadFile(path);
    }

    [Fact]
    public void TestCodePagesEncodingProviderRequired()
    {
        // After registration, GB2312 encoding should be available
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Assert.Equal("Chinese Simplified (GB2312)", Encoding.GetEncoding("GB2312").EncodingName);
    }

    [Fact]
    public void TestWriteFile()
    {
        var path = GetFullPath("WriteTest.txt");
        var content = "Hello Word!";
        Assert.True(FileOperationHelper.WriteFile(path, Encoding.UTF8, content));
        Assert.True(File.Exists(path));
        File.Delete(path);
    }

    protected static string GetFullPath(string fileName)
    {
        return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
    }
}
