namespace EndpointForge.WebApi.Models;

public record DeserializeResult<T>(T? Result = null, IResult? ErrorResult = null) where T : class;