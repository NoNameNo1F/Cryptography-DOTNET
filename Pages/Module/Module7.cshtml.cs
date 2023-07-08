using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;
using System.Text.Json;

namespace Cryptography.Pages {
    
    public class Module7Model : PageModel
    {
            // Khai báo môi trường để lưu file 
        private readonly IWebHostEnvironment _environment;
        public Module7Model( IWebHostEnvironment environment) {
            _environment = environment;
        }


        [Required(ErrorMessage = "Cần có {0} mới mã hóa được ")]  
        [Display(Name = "Message")]
        public string Message { get; set; } = "";

        public string StatusMessage { get; set; } ="";

        // MODULE 7 //
        ////////////////////////////////////////// MODULE: HASHING STRING ///////////////////////////////////
        public byte[] HashString(byte[] message){
            // Hash voi dang SHA-256
            return SHA256.HashData(message);
        }
        ////////////////////////////////////////// HASHING STRING ///////////////////////////////////
        
        public IActionResult OnGet()
        {
            return Page();
        }
        
        public void OnPost(){
            
            Message = this.Request.Form["Message"];
            if(Message != null){
                byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(Message);
                byte[] hashedMessage = HashString(messageBytes);
                StatusMessage = "Hashed của " + Message + " là: " + Convert.ToBase64String(hashedMessage);
            }
            else{
                StatusMessage = "Không có chuỗi";
            }
        }
    }
}