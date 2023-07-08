using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;
using System.Text.Json;

namespace Cryptography.Pages {
    public class JsonFile2 {
        public string Ks { get; set; }
        public string IV { get; set; }
    }
    
    public class Module3Model : PageModel
    {
            // Khai báo môi trường để lưu file 
        private readonly IWebHostEnvironment _environment;
        public Module3Model( IWebHostEnvironment environment) {
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

        // MODULE 3 //
        ////////////////////////////////////////// MODULE: DECRYPTING FILE(AES) ///////////////////////////////////
        public void DecryptFile(string inputFile, string outputFile, byte[] Key, byte[] IV)
        {
            // Kiểm tra tham số 
            if (inputFile == null || inputFile.Length <= 0)
                throw new ArgumentNullException("inputFile");
            if (outputFile == null || outputFile.Length <= 0)
                throw new ArgumentNullException("outputFile");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Tạo key Aes với IV 
            using (Aes aesAlg = Aes.Create())
            {

                // Create an encryptor to perform the stream transform.
                // Khởi tạo encryptor để tiến hành thay đổi stream.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(Key, IV);

                using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
                using (CryptoStream csDecrypt = new CryptoStream(fsOutput, decryptor, CryptoStreamMode.Write))
                {
                    // Copy the input file data to the encryption stream.
                    fsInput.CopyTo(csDecrypt);
                }
            }
        }
        ////////////////////////////////////////// MODULE: DECRYPTING FILE(AES) ///////////////////////////////////




        public IActionResult OnGet()
        {
            return Page();
        }
    
        
       public void OnPost(){
            
            if (ModelState.IsValid == true)
            {
                // Declare Variables
                bool flag = false;
                JsonFile2 json = new JsonFile2();


                // Doc key tu File
                if(KeyDecrypt != null){
                    flag = true;
                    //using (FileStream fsKey = new FileStream(File.ReadAllBytes(KeyDecrypt)
                    // Doc Ks va IV tu file 
                    string keyStr = new StreamReader(KeyDecrypt.OpenReadStream()).ReadToEnd();
                    // Class JsonFile2 gom Ks va IV
                    
                    json = JsonSerializer.Deserialize<JsonFile2>(keyStr);
            
                }
                if(FileUpload != null && flag == true){
                    var filePath = Path.Combine(_environment.WebRootPath, "uploads", FileUpload.FileName);
                    // tạo biên lưu filestream , file mode, tạo stream để đỗ dữ liệu vào
                    using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                    // Binding dữ liệu đến
                    FileUpload.CopyTo(fs);
                    fs.Close();
                }
                // Key
                
                if(flag){
                    // Tạo đường dẫn để lấy file bị mã, và lưu file giải mã
                    string fileEncryptPath = Path.Combine(_environment.WebRootPath, "uploads", FileUpload.FileName);
                    string fileDecryptPath = Path.Combine(_environment.WebRootPath, "decrypted", FileUpload.FileName);

                    byte[] Ks = Convert.FromBase64String(json.Ks);
                    byte[] IVBytes = Convert.FromBase64String(json.IV);

                    // Giải mã file với Ks
                    using (Aes myAes = Aes.Create())
                    {
                        myAes.IV = IVBytes;
                        myAes.Key = Ks;
                        DecryptFile(fileEncryptPath, fileDecryptPath, myAes.Key, myAes.IV);
                    }
                    // Xóa file upload 
                    System.IO.File.Delete(fileEncryptPath);
                }
                
                StatusMessage = "File Của bạn đã được giải mã";
                //RedirectToPage("/Download/Download");
            }
            else{
                StatusMessage = "Dữ liệu gửi đến chưa phù hợp";
            }
        }
    }
}