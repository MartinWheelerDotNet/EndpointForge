using EndpointForge.WebApi.Parsers;
using EndpointForge.WebApi.Tests.Fakes;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Parsers;

public class ResponseBodyParserTests
{
    [Fact]
    public async Task When_ProcessResponseBodyAndBodyContainsNoPlaceholders_Expect_StreamContainsTheOriginalBody()
    {
        const string body = "This is a response body without any placeholders.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory("");
        var responseBodyParser = new ResponseBodyParser(stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body);
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(body);
    }
    
    [Fact]
    public async Task When_ProcessResponseBodyWithValidGeneratePlaceholder_Expect_StreamHasPlaceholderReplaced()
    {
        const string body = "The value '{{generate:test}}' is from a rule.";
        const string expectedBody = "The value 'test' is from a rule.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory("test");
        var responseBodyParser = new ResponseBodyParser(stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body);
        
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
        
        await responseBodyParser.ProcessResponseBody(stream, body);
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(expectedBody);
    }
    
    [Fact]
    public async Task When_ProcessResponseBodyWithUnknownPlaceholder_Expect_StreamContainsPlaceholder()
    {
        const string body = "The placeholder '{{generate:test}}' is unknown.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory(null);
        var responseBodyParser = new ResponseBodyParser(stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body);
        
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
        
        await responseBodyParser.ProcessResponseBody(stream, body);
        
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
        
        await responseBodyParser.ProcessResponseBody(stream, body);
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(body);
    }
    
    [Fact]
    public async Task When_ProcessResponseBodyWithEmptyPlacer_Expect_StreamContainsThePlaceholder()
    {
        const string body = "The placeholder instruction '{{}}' is not empty.";
        var stubEndpointForgeRuleFactory = new FakeEndpointForgeRuleFactory(null);
        var responseBodyParser = new ResponseBodyParser(stubEndpointForgeRuleFactory);
        var stream = new MemoryStream();
        
        await responseBodyParser.ProcessResponseBody(stream, body);
        
        stream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(stream).ReadToEndAsync();
        
        responseBody.Should().Be(body);
    }
}