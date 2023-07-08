using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;
using System.Text.Json;

namespace Cryptography.Pages {
    
    public class Module5Model : PageModel
    {
            // Khai báo môi trường để lưu file 
        private readonly IWebHostEnvironment _environment;
        public Module5Model( IWebHostEnvironment environment) {
            _environment = environment;
        }

        [Required(ErrorMessage = "Cần có {0} mới mã hóa được ")]  
        [Display(Name = "Message")]
        public string Message { get; set; } = "";

        public string StatusMessage { get; set; } ="";
    
        // MODULE 5 //
        ////////////////////////////////////////// MODULE: ENCRYPTING STRING(RSA) ///////////////////////////////////
        public byte[] EncryptString(byte[] message, byte[] publicKey){
            using (RSA rsa = RSA.Create()){
                rsa.ImportRSAPublicKey(publicKey, out _);
                return rsa.Encrypt(message, RSAEncryptionPadding.OaepSHA256);
            }
        }
        ////////////////////////////////////////// MODULE: ENCRYPTING STRING(RSA) ///////////////////////////////////

        public IActionResult OnGet()
        {
            return Page();
        }

        public void OnPost(){

            Message = this.Request.Form["Message"];

            if (Message != null){
                byte[] kPrivate = new byte[0];
                using (RSA rsa = RSA.Create()){
                    byte[] kPublic = rsa.ExportRSAPublicKey();
                    kPrivate = rsa.ExportRSAPrivateKey();
                    byte[] plaintext = System.Text.Encoding.ASCII.GetBytes(Message);
                    string cipher = Convert.ToBase64String(EncryptString(plaintext,kPublic));
                    //AESKey = System.Text.Encoding.Default.GetString(myAes.Key);
                    string fileEncryptPath = Path.Combine(_environment.WebRootPath, "encrypted", "cipher.txt");
                    System.IO.File.WriteAllText(fileEncryptPath, cipher);
                }
                StatusMessage = Convert.ToBase64String(kPrivate);
            }
            else{
                throw new ArgumentNullException("Không có chuỗi");
            }
        }
    }
}