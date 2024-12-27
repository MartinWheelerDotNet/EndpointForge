
using Microsoft.Extensions.Primitives;

namespace EndpointForge.WebApi.Models;

public abstract class MutableEndpointDataSource : EndpointDataSource
{
    private readonly Lock _endpointsLock = new();
    private readonly Lock _tokenLock = new();
    
    private readonly List<Endpoint> _endpoints = [];
    private CancellationTokenSource _cancellationTokenSource;
    private IChangeToken _changeToken;
    
    protected MutableEndpointDataSource()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _changeToken = new CancellationChangeToken(_cancellationTokenSource.Token);
    }

    public override IChangeToken GetChangeToken() => _changeToken;
    public override IReadOnlyList<Endpoint> Endpoints
    {
        get
        {
            lock (_endpointsLock)
            {
                return _endpoints.AsReadOnly();
            }
        }
    }

    protected void RefreshEndpoints()
    {
        lock (_tokenLock)
        {
            var oldCancellationTokenSource = _cancellationTokenSource;

            _cancellationTokenSource = new CancellationTokenSource();
            _changeToken = new CancellationChangeToken(_cancellationTokenSource.Token);

            oldCancellationTokenSource.Cancel();
        }
    }

    protected void AddEndpoint(Endpoint endpoint, bool apply = true)
    {
        lock (_endpointsLock)
        {
            _endpoints.Add(endpoint);
            if (apply) RefreshEndpoints();
        }
    }
}