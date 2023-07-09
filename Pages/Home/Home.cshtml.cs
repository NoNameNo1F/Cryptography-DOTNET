using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IPinfo;
using IPinfo.Models;
namespace Cryptography.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }
    public IPResponse userIpinfo { get; set; }
    public string GetOperatingSystem(string userAgent)
    {
        int start = userAgent.IndexOf('(') + 1;
        int end = userAgent.IndexOf(')');

        if (start >= 0 && end >= 0 && end > start)
        {
            return userAgent.Substring(start, end - start);
        }

        return string.Empty;
    }

    public string GetBrowser(string userAgent)
    {
        int start = userAgent.IndexOf(" Gecko") + 8;
        int end = userAgent.IndexOf(' ', start);

        if (start >= 6 && end >= 0 && end > start)
        {
            return userAgent.Substring(start, end - start);
        }

        return string.Empty;
    }
    public void OnGet()
    {
        // Khởi tạo Ipinfo ip
        string? token = WebApplication.CreateBuilder().Configuration.GetConnectionString("token-Ipinfo");
        IPinfoClient ip = new IPinfoClient.Builder()
                            .AccessToken(token)
                            .Build();
        IPResponse iPResponse = ip.IPApi.GetDetails();
        userIpinfo = iPResponse;
    }
}
