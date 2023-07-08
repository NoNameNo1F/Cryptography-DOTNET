using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;
using System.Text.Json;

namespace Cryptography.Pages {
    
    public class Module4Model : PageModel
    {
            // Khai báo môi trường để lưu file 
        private readonly IWebHostEnvironment _environment;
        public Module4Model( IWebHostEnvironment environment) {
            _environment = environment;
        }

        public string StatusMessage { get; set; } ="";


        // MODULE 4 //
        ////////////////////////////////////////// MODULE: GENERATE RSA KEY ///////////////////////////////////
        public string GenerateRSAKey(){
            using (RSA rsa = RSA.Create()){
                byte[] pubKey = rsa.ExportRSAPublicKey();
                byte[] privateKey = rsa.ExportRSAPrivateKey();
                return "Public Key: " + Convert.ToBase64String(pubKey) + 
                                "\nPrivate Key: " + Convert.ToBase64String(privateKey);
            }
        }
        ////////////////////////////////////////// MODULE: GENERATE RSA KEY  ///////////////////////////////////

        public IActionResult OnGet()
        {
            return Page();
        }

        
        public void OnPost(){
            StatusMessage = GenerateRSAKey();
        }
    }
}