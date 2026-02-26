namespace ERP.Services.Abstractions.CommonServices;
public interface IWebService
{
    Task<string> DownloadAsync(string url);
}