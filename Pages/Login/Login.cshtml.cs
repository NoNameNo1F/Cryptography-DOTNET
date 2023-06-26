using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cryptography.Pages;

public class LoginModel : PageModel
{
    private readonly ILogger<LoginModel> _logger;

    public readonly UserService userService;
    public LoginModel(ILogger<LoginModel> logger, UserService _userService)
    {
        _logger = logger;
        userService = _userService;
    }

    public User user { get; set; }
    public void OnGet(int? id)
    {
    //     if( id != null){
    //         user = userService.Find(id.Value);
    //     }
    //    else{

    //    }
    }
    public void OnPost(){
        var header_form = this.Request.Form;
        foreach(var form in header_form){
            Console.WriteLine($"key: {form.Key} = value {form.Value}");
        }
        // Console.WriteLine(this.Request.GetDisplayUrl());
    }
}
