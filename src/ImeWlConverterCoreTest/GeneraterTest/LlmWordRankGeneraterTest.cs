using Xunit;
using Studyzy.IMEWLConverter.Generaters;

namespace Studyzy.IMEWLConverter.CoreTest.GeneraterTest;

public class LlmWordRankGeneraterTest
{
    [Fact]
    public void TestParseRank()
    {
        var generater = new LlmWordRankGenerater();
        var json = @"{
  ""choices"": [
    {
      ""message"": {
        ""content"": ""{\""苹果\"": 850000}""
      }
    }
  ]
}";
        var rank = generater.ParseRank(json);
        Assert.Equal(850000, rank);
    }

    [Fact]
    public void TestParseRankRegex()
    {
        var generater = new LlmWordRankGenerater();
        var json = @"{
  ""choices"": [
    {
      ""message"": {
        ""content"": ""\\\""苹果\\\"": 12345""
      }
    }
  ]
}";
        var rank = generater.ParseRank(json);
        Assert.Equal(12345, rank);
    }

    [Fact]
    public void TestParseRanksJson()
    {
        var generater = new LlmWordRankGenerater();
        var json = @"{
  ""choices"": [
    {
      ""message"": {
        ""content"": ""{\""苹果\"": 850000, \""香蕉\"": 700000}""
      }
    }
  ]
}";
        var ranks = generater.ParseRanks(json);
        Assert.Equal(850000, ranks["苹果"]);
        Assert.Equal(700000, ranks["香蕉"]);
    }

    [Fact]
    public void TestParseRanksRegex()
    {
        var generater = new LlmWordRankGenerater();
        var json = @"{
  ""choices"": [
    {
      ""message"": {
        ""content"": ""以下是词频：\n\""苹果\"": 850000\n\""香蕉\"": 700000""
      }
    }
  ]
}";
        var ranks = generater.ParseRanks(json);
        Assert.Equal(850000, ranks["苹果"]);
        Assert.Equal(700000, ranks["香蕉"]);
    }

    [Fact]
    public void TestGetFullApiEndpoint()
    {
        var generater = new LlmWordRankGenerater();

        generater.Config.ApiEndpoint = "https://api.deepseek.com";
        Assert.Equal("https://api.deepseek.com/v1/chat/completions", generater.GetFullApiEndpoint());

        generater.Config.ApiEndpoint = "https://api.deepseek.com/";
        Assert.Equal("https://api.deepseek.com/v1/chat/completions", generater.GetFullApiEndpoint());

        generater.Config.ApiEndpoint = "https://api.deepseek.com/v1";
        Assert.Equal("https://api.deepseek.com/v1/chat/completions", generater.GetFullApiEndpoint());

        generater.Config.ApiEndpoint = "https://api.deepseek.com/v1/";
        Assert.Equal("https://api.deepseek.com/v1/chat/completions", generater.GetFullApiEndpoint());

        generater.Config.ApiEndpoint = "https://api.deepseek.com/v1/chat/completions";
        Assert.Equal("https://api.deepseek.com/v1/chat/completions", generater.GetFullApiEndpoint());

        generater.Config.ApiEndpoint = "https://api.deepseek.com/v1/chat/completions/";
        Assert.Equal("https://api.deepseek.com/v1/chat/completions/", generater.GetFullApiEndpoint());
    }
}
