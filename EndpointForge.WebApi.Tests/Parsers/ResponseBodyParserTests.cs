using EndpointForge.WebApi.Parsers;
using EndpointForge.WebApi.Tests.Fakes;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Parsers;

public class ResponseBodyParserTests
{
    #region General Placeholder Tests
    [Fact]
    public async Task When_ProcessResponseBodyAndBodyContainsNoPlaceholders_Expect_StreamContainsTheOriginalBody()
    {
        const string body = "This is a response body without any placeholders.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory("");
        var responseBodyParser = new ResponseBodyParser(stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, new Dictionary<string, string>());
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(body);
    }
    
    [Fact]
    public async Task When_ProcessResponseBodyWithInvalidPlaceholder_Expect_StreamContainsPlaceholder()
    {
        const string body = "The placeholder '{{generate}}' is not valid.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory(null);
        var responseBodyParser = new ResponseBodyParser(stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, new Dictionary<string, string>());
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(body);
    }
    
    [Fact]
    public async Task When_ProcessResponseBodyWithInvalidPlaceholderInstruction_Expect_StreamContainsThePlaceholder()
    {
        const string body = "The placeholder instruction '{{test:guid}}' is not valid.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory(null);
        var responseBodyParser = new ResponseBodyParser(stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, new Dictionary<string, string>());
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(body);
    }
    
    [Fact]
    public async Task When_ProcessResponseBodyWithEmptyPlaceholder_Expect_StreamContainsThePlaceholder()
    {
        const string body = "The placeholder instruction '{{}}' is not empty.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory(null);
        var responseBodyParser = new ResponseBodyParser(stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, new Dictionary<string, string>());
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(body);
    }
    
        
    [Fact]
    public async Task When_ProcessResponseBodyWithUnknownPlaceholder_Expect_StreamContainsPlaceholder()
    {
        const string body = "The placeholder '{{generate:test}}' is unknown.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory(null);
        var responseBodyParser = new ResponseBodyParser(stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, new Dictionary<string, string>());
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(body);
    }
    #endregion
    
    #region Generate Placeholder Tests
    [Fact]
    public async Task When_ProcessResponseBodyWithValidGeneratePlaceholder_Expect_StreamHasPlaceholderReplaced()
    {
        const string body = "The value '{{generate:test}}' is from a rule.";
        const string expectedBody = "The value 'test' is from a rule.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory("test");
        var responseBodyParser = new ResponseBodyParser(stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, new Dictionary<string, string>());
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(expectedBody);
    }
    
    [Fact]
    public async Task When_ProcessResponseBodyWithTwoValidGeneratePlaceholders_Expect_StreamHasPlaceholdersReplaced()
    {
        const string body = "The value '{{generate:test}}' is from a rule, as `{{generate:test}}` is also.";
        const string expectedBody = "The value 'test' is from a rule, as `test` is also.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory("test");
        var responseBodyParser = new ResponseBodyParser(stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, new Dictionary<string, string>());
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(expectedBody);
    }
    #endregion
    
    #region Insert Placeholder Tests
    [Fact]
    public async Task When_ProcessResponseBodyWithValidInsertPlaceholder_Expect_StreamHasPlaceholderWithValue()
    {
        const string body = "The value '{{insert:test-parameter}}' is from a parameter.";
        const string expectedBody = "The value 'parameter-value' is from a parameter.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory();
        var responseBodyParser = new ResponseBodyParser(stubEndpointForgeRuleFactory);
        var parameters = new Dictionary<string, string>
        {
            {"test-parameter", "parameter-value"}
        };
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, parameters);
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(expectedBody);
    }
    
    [Fact]
    public async Task When_ProcessResponseBodyWithTwoValidInsertPlaceholders_Expect_StreamHasPlaceholdersReplacedWithValues()
    {
        const string body = "The values '{{insert:test-parameter}}' and '{{insert:test-parameter}}' are from parameters.";
        const string expectedBody = "The values 'parameter-value' and 'parameter-value' are from parameters.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory();
        var responseBodyParser = new ResponseBodyParser(stubEndpointForgeRuleFactory);
        var parameters = new Dictionary<string, string>
        {
            {"test-parameter", "parameter-value"}
        };
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, parameters);
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(expectedBody);
    }
    
    [Fact]
    public async Task When_ProcessResponseBodyWithTwoDifferentInsertPlaceholders_Expect_StreamHasPlaceholdersReplacedWithValues()
    {
        const string body = "The values '{{insert:test-parameter-1}}' and '{{insert:test-parameter-2}}' are from parameters.";
        const string expectedBody = "The values 'parameter-value-1' and 'parameter-value-2' are from parameters.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory();
        var responseBodyParser = new ResponseBodyParser(stubEndpointForgeRuleFactory);
        var parameters = new Dictionary<string, string>
        {
            {"test-parameter-1", "parameter-value-1"},
            {"test-parameter-2", "parameter-value-2"}
        };
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, parameters);
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(expectedBody);
    }
    
    [Fact]
    public async Task When_ProcessResponseBodyWithoutParameterAndWithInsertPlaceholder_Expect_StreamHasPlaceholdersReplacedWithNull()
    {
        const string body = "The value '{{insert:test-parameter-1}}' is not found and so null is written";
        const string expectedBody = "The value 'null' is not found and so null is written";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory();
        var responseBodyParser = new ResponseBodyParser(stubEndpointForgeRuleFactory);
        var parameters = new Dictionary<string, string>
        {
            {"test-parameter-2", "parameter-value-2"}
        };
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, parameters);
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(expectedBody);
    }
    #endregion
}