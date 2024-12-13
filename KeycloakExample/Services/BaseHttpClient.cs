using KeycloakExample.Helpers;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace KeycloakExample.Services;

public abstract class BaseHttpClient
{
    protected async Task<TResponse> GetAsync<TResponse>(
    string action,
    bool skipAttachingToken = false,
    CancellationToken stoppingToken = default)
    {
        Uri url = await CreateUrlAsync(action);
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        return await SendInternalAsync<TResponse>(
            requestMessage: requestMessage,
            content: null,
            contentType: ApiMediaType.Json,
            skipAttachingToken: skipAttachingToken,
            stoppingToken: stoppingToken);
    }

    protected async Task<TResponse> PostAsync<TResponse>(
        string action,
        object? content,
        ApiMediaType mediaType = ApiMediaType.Json,
        bool skipAttachingToken = false,
        CancellationToken stoppingToken = default)
    {
        Uri url = await CreateUrlAsync(action);
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
        return await SendInternalAsync<TResponse>(
            requestMessage: requestMessage,
            content: content,
            contentType: mediaType,
            skipAttachingToken: skipAttachingToken,
            stoppingToken: stoppingToken);
    }

    protected async Task<TResponse> PutAsync<TResponse>(
        string action,
        object? content,
        ApiMediaType mediaType = ApiMediaType.Json,
        bool skipAttachingToken = false,
        CancellationToken stoppingToken = default)
    {
        Uri url = await CreateUrlAsync(action);
        using var requestMessage = new HttpRequestMessage(HttpMethod.Put, url);
        return await SendInternalAsync<TResponse>(
            requestMessage: requestMessage,
            content: content,
            contentType: mediaType,
            skipAttachingToken: skipAttachingToken,
            stoppingToken: stoppingToken);
    }

    private async Task<TResponse> SendInternalAsync<TResponse>(
        HttpRequestMessage requestMessage,
        object? content,
        ApiMediaType contentType = ApiMediaType.Json,
        bool skipAttachingToken = false,
        CancellationToken stoppingToken = default)
    {
        // todo: log ?

        try
        {
            if (content is not null)
            {
                requestMessage.Content = ConvertToHttpContent(content, contentType);
            }

            if (!skipAttachingToken)
            {
                await AttachTokenToRequestAsync(requestMessage, stoppingToken);
            }

            using HttpClient client = await CreateHttpClientAsync();
            using HttpResponseMessage response = await client.SendAsync(requestMessage, cancellationToken: stoppingToken);
            string? body = await response.Content.ReadAsStringAsync(stoppingToken);

            //log.StatusCode = (int)response.StatusCode;
            //log.IsSuccessStatusCode = response.IsSuccessStatusCode;
            //log.DataReceived = body;

            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                await RemoveTokenFromCacheAsync();
            }

            return MapBodyToRequestedType();

            TResponse MapBodyToRequestedType()
            {
                try
                {
                    if (typeof(TResponse) == typeof(bool))
                    {
                        return (TResponse)(object)true;
                    }

                    if (string.IsNullOrEmpty(body))
                    {
                        return default!;
                    }

                    if (typeof(TResponse) == typeof(string) || typeof(TResponse) == typeof(int) || typeof(TResponse) == typeof(long))
                    {
                        return (TResponse)(object)body;
                    }

                    return body.FromJson<TResponse>()!;
                }
                catch (Exception)
                {
                    //todo: log
                    return default!;
                }
            }
        }
        catch (Exception)
        {
            //todo: log
            return default!;
        }
        finally
        {
            // todo: log ?
        }
    }

    protected abstract ValueTask<HttpClient> CreateHttpClientAsync();

    protected abstract ValueTask<Uri> CreateUrlAsync(string action);

    protected abstract Task AttachTokenToRequestAsync(HttpRequestMessage request, CancellationToken stoppingToken = default);

    protected abstract Task RemoveTokenFromCacheAsync();

    private HttpContent ConvertToHttpContent<TContent>(TContent content, ApiMediaType mediaType)
    {
        switch (mediaType)
        {
            case ApiMediaType.Json:
                return new StringContent(content.ToJson() ?? "", Encoding.UTF8, MediaTypeNames.Application.Json);

            case ApiMediaType.Xml:
                return new StringContent(content?.ToString() ?? "", Encoding.UTF8, MediaTypeNames.Application.Xml);

            case ApiMediaType.Html:
                return new StringContent(content?.ToString() ?? "", Encoding.UTF8, MediaTypeNames.Text.Html);

            case ApiMediaType.FormUrlEncoded:
                if (content is IEnumerable<KeyValuePair<string, string>> keyValuePairs)
                {
                    return new FormUrlEncodedContent(keyValuePairs);
                }

                if (content is Dictionary<string, string> dictionary)
                {
                    return new FormUrlEncodedContent(dictionary);
                }

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null);
        }

        return new StringContent(string.Empty);
    }
}

public enum ApiMediaType
{
    Json,
    Xml,
    Html,
    FormUrlEncoded
}