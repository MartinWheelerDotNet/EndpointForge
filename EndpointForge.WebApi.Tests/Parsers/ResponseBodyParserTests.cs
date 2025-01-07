using EndpointForge.Abstractions.Models;
using EndpointForge.WebApi.Parsers;
using EndpointForge.WebApi.Tests.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EndpointForge.WebApi.Tests.Parsers;

public class ResponseBodyParserTests
{
    private static readonly ILogger<ResponseBodyParser> StubLogger = new NullLogger<ResponseBodyParser>();
    #region General Placeholder Tests
    [Fact]
    public async Task When_ProcessResponseBodyAndBodyContainsNoPlaceholders_Expect_StreamContainsTheOriginalBody()
    {
        const string body = "This is a response body without any placeholders.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory(new FakeGeneratorRule());
        var responseBodyParser = new ResponseBodyParser(StubLogger, stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, new());
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(body);
    }
    
    [Fact]
    public async Task When_ProcessResponseBodyWithInvalidPlaceholder_Expect_StreamContainsPlaceholder()
    {
        const string body = "The placeholder '{{generate}}' is not valid.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory();
        var responseBodyParser = new ResponseBodyParser(StubLogger, stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, new());
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(body);
    }
    
    [Fact]
    public async Task When_ProcessResponseBodyWithInvalidPlaceholderInstruction_Expect_StreamContainsThePlaceholder()
    {
        const string body = "The placeholder instruction '{{test:guid}}' is not valid.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory();
        var responseBodyParser = new ResponseBodyParser(StubLogger, stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, new());
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(body);
    }
    
    [Fact]
    public async Task When_ProcessResponseBodyWithEmptyPlaceholder_Expect_StreamRemovesThePlaceholder()
    {
        const string body = "The placeholder instruction '{{}}' is empty.";
        const string expectedBody = "The placeholder instruction '' is empty.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory();
        var responseBodyParser = new ResponseBodyParser(StubLogger, stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, new());
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(expectedBody);
    }
    
        
    [Fact]
    public async Task When_ProcessResponseBodyWithUnknownGeneratePlaceholder_Expect_StreamContainsPlaceholder()
    {
        const string body = "The placeholder '{{generate:test}}' is unknown.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory(
            new FakeGeneratorRule("generate", "otherType"));
        var responseBodyParser = new ResponseBodyParser(StubLogger, stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, new());
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(body);
    }
    
    [Fact]
    public async Task When_ProcessResponseBodyWithUnknownInsertPlaceholder_Expect_StreamContainsPlaceholder()
    {
        const string body = "The placeholder '{{insert:test:test-parameter}}' is unknown.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory(
            new FakeGeneratorRule("insert", "other-type"));
        var responseBodyParser = new ResponseBodyParser(StubLogger, stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(
            stream, 
            body, 
            [new("static", "test-parameter", "parameter-value")]);
        
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
        const string expectedBody = "The value 'test-value' is from a rule.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory(
            new FakeGeneratorRule("test", "test-value"));
        var responseBodyParser = new ResponseBodyParser(StubLogger, stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, new());
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(expectedBody);
    }
    
    [Fact]
    public async Task When_ProcessResponseBodyWithTwoValidGeneratePlaceholders_Expect_StreamHasPlaceholdersReplaced()
    {
        const string body = "The value '{{generate:test}}' is from a rule, as `{{generate:test}}` is also.";
        const string expectedBody = "The value 'test-value' is from a rule, as `test-value` is also.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory(
            new FakeGeneratorRule("test", "test-value"));
        var responseBodyParser = new ResponseBodyParser(StubLogger, stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, new());
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(expectedBody);
    }
    #endregion
    
    #region Insert Placeholder Tests
    [Fact]
    public async Task When_ProcessResponseBodyWithValidInsertPlaceholder_Expect_StreamHasPlaceholderWithValue()
    {
        const string body = "The value '{{insert:parameter:test-parameter}}' is from a parameter.";
        const string expectedBody = "The value 'parameter-value' is from a parameter.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory(
            new FakeInsertRule("parameter"));
        var responseBodyParser = new ResponseBodyParser(StubLogger, stubEndpointForgeRuleFactory);
        var parameters = new List<EndpointForgeParameterDetails>
        {
            new("static", "test-parameter", "parameter-value")
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
        const string body = "The values '{{insert:parameter:test-parameter}}' and '{{insert:parameter:test-parameter}}' are from parameters.";
        const string expectedBody = "The values 'parameter-value' and 'parameter-value' are from parameters.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory(
            new FakeInsertRule("parameter"));
        var responseBodyParser = new ResponseBodyParser(StubLogger, stubEndpointForgeRuleFactory);
        var parameters = new List<EndpointForgeParameterDetails>
        {
            new("static", "test-parameter", "parameter-value")
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
        const string body = "The values '{{insert:parameter:test-parameter-1}}' and '{{insert:parameter:test-parameter-2}}' are from parameters.";
        const string expectedBody = "The values 'parameter-value-1' and 'parameter-value-2' are from parameters.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory(
            new FakeInsertRule("parameter"));
        var responseBodyParser = new ResponseBodyParser(StubLogger, stubEndpointForgeRuleFactory);
        var parameters = new List<EndpointForgeParameterDetails>
        {
            new("static", "test-parameter-1", "parameter-value-1"),
            new("static", "test-parameter-2", "parameter-value-2")
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
        const string body = "The parameter is not found and so '{{insert:parameter:test-parameter-1}}' is written.";
        const string expectedBody = "The parameter is not found and so 'null' is written.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory(
            new FakeInsertRule("parameter"));
        var responseBodyParser = new ResponseBodyParser(StubLogger, stubEndpointForgeRuleFactory);
        var parameters = new List<EndpointForgeParameterDetails>
        {
            new("static", "test-parameter-2", "parameter-value-2")
        };
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body, parameters);
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(expectedBody);
    }
    #endregion
}