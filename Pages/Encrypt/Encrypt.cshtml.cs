using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cryptography.Pages;

public class EncryptModel : PageModel
{
    private readonly ILogger<EncryptModel> _logger;

    public EncryptModel(ILogger<EncryptModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        
    }
}
