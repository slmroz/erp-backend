using ERP.Services.Abstractions.CommonServices;

namespace ERP.Infrastructure.CommonServices;
internal class WebService : IWebService
{
    private readonly IHttpClientFactory _clientFactory;

    public WebService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }
    public async Task<string> DownloadAsync(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Accept", "text/html, application/xhtml+xml, image/jxr, */*");
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
        request.Headers.Add("AcceptEncoding", "gzip, deflate");
        request.Headers.Add("AcceptLanguage", "pl-PL");

        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return responseText;
        }
        else
        {
            throw new Exception("Web request failed");
        }
    }
}
