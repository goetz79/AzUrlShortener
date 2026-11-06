using Azure.Core;
using Azure.Identity;
using System.Net.Http.Headers;

namespace Cloud5mins.ShortenerTools.TinyBlazorAdmin;

public class ApiAuthenticationHandler : DelegatingHandler
{
    private readonly ManagedIdentityCredential credential;

    public ApiAuthenticationHandler()
    {
        var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID")
            ?? throw new InvalidOperationException("AZURE_CLIENT_ID is not configured.");

        credential = new ManagedIdentityCredential(
            ManagedIdentityId.FromUserAssignedClientId(clientId));
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await credential.GetTokenAsync(
            new TokenRequestContext(
                ["api://5eeeeeea-eff8-4029-96a6-6feb3f106415/.default"]),
            cancellationToken);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token.Token);

        return await base.SendAsync(request, cancellationToken);
    }
}

