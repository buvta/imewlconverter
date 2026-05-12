using ImeWlConverter.Core.WordRank;
using Xunit;

namespace Studyzy.IMEWLConverter.CoreTest.GeneraterTest;

public class LlmWordRankGeneraterTest
{
    [Fact]
    public void TestParseRank()
    {
        var generator = new LlmWordRankGenerator();
        var json = """
        {
          "choices": [
            {
              "message": {
                "content": "{\"苹果\": 850000}"
              }
            }
          ]
        }
        """;
        var rank = generator.ParseRank(json);
        Assert.Equal(850000, rank);
    }

    [Fact]
    public void TestParseRankRegex()
    {
        var generator = new LlmWordRankGenerator();
        var json = """
        {
          "choices": [
            {
              "message": {
                "content": "\"苹果\": 12345"
              }
            }
          ]
        }
        """;
        var rank = generator.ParseRank(json);
        Assert.Equal(12345, rank);
    }

    [Fact]
    public void TestParseRanksJson()
    {
        var generator = new LlmWordRankGenerator();
        var json = """
        {
          "choices": [
            {
              "message": {
                "content": "{\"苹果\": 850000, \"香蕉\": 700000}"
              }
            }
          ]
        }
        """;
        var ranks = generator.ParseRanks(json);
        Assert.Equal(850000, ranks["苹果"]);
        Assert.Equal(700000, ranks["香蕉"]);
    }

    [Fact]
    public void TestParseRanksRegex()
    {
        var generator = new LlmWordRankGenerator();
        var json = """
        {
          "choices": [
            {
              "message": {
                "content": "以下是词频：\n\"苹果\": 850000\n\"香蕉\": 700000"
              }
            }
          ]
        }
        """;
        var ranks = generator.ParseRanks(json);
        Assert.Equal(850000, ranks["苹果"]);
        Assert.Equal(700000, ranks["香蕉"]);
    }

    [Fact]
    public void TestGetFullApiEndpoint()
    {
        var generator = new LlmWordRankGenerator();

        generator.Config = new LlmConfig { ApiEndpoint = "https://api.openai.com/v1/chat/completions" };
        Assert.Equal("https://api.openai.com/v1/chat/completions", generator.GetFullApiEndpoint());

        generator.Config = new LlmConfig { ApiEndpoint = "https://api.openai.com/v1" };
        Assert.Equal("https://api.openai.com/v1/chat/completions", generator.GetFullApiEndpoint());

        generator.Config = new LlmConfig { ApiEndpoint = "https://api.openai.com/v1/" };
        Assert.Equal("https://api.openai.com/v1/chat/completions", generator.GetFullApiEndpoint());

        generator.Config = new LlmConfig { ApiEndpoint = "https://custom-api.example.com" };
        Assert.Equal("https://custom-api.example.com/v1/chat/completions", generator.GetFullApiEndpoint());

        generator.Config = new LlmConfig { ApiEndpoint = "https://custom-api.example.com/" };
        Assert.Equal("https://custom-api.example.com/v1/chat/completions", generator.GetFullApiEndpoint());
    }
}
