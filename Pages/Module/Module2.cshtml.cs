using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;
using System.Text.Json;

namespace Cryptography.Pages {
    
    public class Module2Model : PageModel
    {
            // Khai báo môi trường để lưu file 
        private readonly IWebHostEnvironment _environment;
        public Module2Model( IWebHostEnvironment environment) {
            _environment = environment;
        }
        [BindProperty]
        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "Cần có {0} để mã hóa ")]
        [Display(Name = "File Mã Hóa")]
        public IFormFile FileUpload { get; set; }


        [Display(Name = "Khóa AES")]
        public byte[] AESKey { get; set; } = new byte[0];
        [Display(Name = "IV")]
        public byte[] IV { get; set; } = new byte[0];

        public string StatusMessage { get; set; } ="";


        // MODULE 2 //
        ////////////////////////////////////////// MODULE: ENCRYPTING FILE(AES) ///////////////////////////////////
        public void EncryptFile(string inputFile, string outputFile, byte[] Key, byte[] IV)
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
                
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(Key, IV);
                // Mã 
                using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (CryptoStream csEncrypt = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
                {
                    // Copy the input file data to the encryption stream.
                    fsInput.CopyTo(csEncrypt);
                }
            }
        }
        ////////////////////////////////// MODULE: ENCRYPTING FILE(AES) ///////////////////////////////////

        public IActionResult OnGet()
        {
            return Page();
        }
        

        public void OnPost(){
            if (ModelState.IsValid == true)
            {
            //1 Lưu file tải xuống để mã hóa
                bool flag = false;
                if(FileUpload != null){
                    flag = true;
                    string filePath = Path.Combine(_environment.WebRootPath, "uploads", FileUpload.FileName);
                    // tạo biên lưu filestream , file mode, tạo stream để đỗ dữ liệu vào
                    using var fs = new FileStream(filePath, FileMode.Create);
                    // Binding dữ liệu đến
                    FileUpload.CopyTo(fs);
                    fs.Close();
                }

// 2 Mã File được tải lên với AES Key và file được lưu lại vào trong folder /encrypted
                string jsonString = "";
                if(flag){
                    string filePath = Path.Combine(_environment.WebRootPath, "uploads", FileUpload.FileName);
                    string fileEncryptPath = Path.Combine(_environment.WebRootPath, "encrypted", FileUpload.FileName);

                    // Sinh khóa AES va luu lai vao AESKey để mã hóa với RSA
                    using (Aes myAes = Aes.Create())
                    {
                        EncryptFile(filePath, fileEncryptPath, myAes.Key , myAes.IV);
                        //AESKey = System.Text.Encoding.Default.GetString(myAes.Key);
                        AESKey = myAes.Key;
                        IV = myAes.IV;
                    }

                    // Sau khi mã hóa thì xóa file Upload lên 
                    System.IO.File.Delete(filePath);
                    

                    var jsonKey = new {
                        Ks = Convert.ToBase64String(AESKey),
                        IV = Convert.ToBase64String(IV)
                    };
                    // Serialize the JSON object
                    jsonString = JsonSerializer.Serialize(jsonKey);
                }
                StatusMessage = jsonString;
                //return RedirectToPage("/Module/Module");
            }
            else{
                StatusMessage = "Dữ liệu gửi đến chưa phù hợp";
                //return RedirectToPage("/Module/Module");
            }
        }
    }
}