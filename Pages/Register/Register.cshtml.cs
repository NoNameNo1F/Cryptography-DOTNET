using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cryptography.Pages;

public class RegisterModel : PageModel
{
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(ILogger<RegisterModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    //    if( Request.RouteValues["id"] != null){
    //         int id = int.Parse(Request.RouteValues["id"].ToString());
    //    }
    //    else{

    //    }
    }
    public void OnPost(){
        
    }
}
