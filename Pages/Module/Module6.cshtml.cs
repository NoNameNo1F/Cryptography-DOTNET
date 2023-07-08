using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;
using System.Text.Json;

namespace Cryptography.Pages {
    
    public class Module6Model : PageModel
    {
            // Khai báo môi trường để lưu file 
        private readonly IWebHostEnvironment _environment;
        public Module6Model( IWebHostEnvironment environment) {
            _environment = environment;
        }
        [BindProperty]
        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "Cần có {0} để giải mã ")]
        [Display(Name = "File Bị Mã")]
        public IFormFile FileUpload { get; set; }

        [BindProperty]
        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "Cần có {0} để giải mã ")]
        [Display(Name = "Key Giải Mã")]
        public IFormFile KeyDecrypt { get; set; }
        public string StatusMessage { get; set; } ="";

        // MODULE 6 //
        ////////////////////////////////////////// MODULE: DECRYPTING STRING(RSA) ///////////////////////////////////
        public byte[] DecryptString(byte[] cipher, byte[] privateKey){
            using (RSA rsa = RSA.Create()){
                rsa.ImportRSAPrivateKey(privateKey, out _);
                return rsa.Decrypt(cipher, RSAEncryptionPadding.OaepSHA256);
            }
        }
        ////////////////////////////////////////// MODULE: DECRYPTING STRING(RSA) ///////////////////////////////////


        public IActionResult OnGet()
        {
            return Page();
        }

        public void OnPost(){

            if (ModelState.IsValid == true)
            {
                // Declare Variables
                bool flag = false;
                string cipher = "";
                string Key = "";
                if(KeyDecrypt != null){
                    flag = true;
                    Key = new StreamReader(KeyDecrypt.OpenReadStream()).ReadToEnd();
                    
                }
                if(FileUpload != null && flag == true){
                    cipher = new StreamReader(FileUpload.OpenReadStream()).ReadToEnd();
                }
                // Key
                
                if(flag){
                    byte[] privateKey = Convert.FromBase64String(Key);
                    byte[] cipherBytes = Convert.FromBase64String(cipher);
                    StatusMessage = "Chuỗi của bạn là: " + System.Text.Encoding.ASCII.GetString(DecryptString(cipherBytes, privateKey));
                }
            }
            else{
                StatusMessage = "Dữ liệu gửi đến chưa phù hợp";
            }
        }
    }
}