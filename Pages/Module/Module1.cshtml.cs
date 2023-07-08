using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;
using System.Text.Json;

namespace Cryptography.Pages {
    
    public class Module1Model : PageModel
    {
            // Khai báo môi trường để lưu file 
        private readonly IWebHostEnvironment _environment;
        public Module1Model( IWebHostEnvironment environment) {
            _environment = environment;
        }

        public string StatusMessage { get; set; } ="";
    

        // MODULE 1 //
        ////////////////////////////////////////// MODULE: GENERATE KS ///////////////////////////////////
        public string GenerateKs(){
            using (Aes myAes = Aes.Create()){
                return Convert.ToBase64String(myAes.Key);
            }
        }
        ////////////////////////////////////////// MODULE: GENERATE KS ///////////////////////////////////

        public IActionResult OnGet()
        {
            return Page();
        }
     
        public void OnPost(){
            
           string Key = GenerateKs();
           StatusMessage = "Key AES: " + Key;
        }
    }
}