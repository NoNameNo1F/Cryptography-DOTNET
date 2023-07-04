using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;
using System.Text.Json;
namespace Cryptography.Pages {
    public class JsonFile {
        public string Kx { get; set; }
        public string IV { get; set; }
        public string HKPrivate { get; set; }
    }
    public class DecryptModel : PageModel
    {
            // Khai báo môi trường để lưu file 
        private readonly IWebHostEnvironment _environment;
        public DecryptModel( IWebHostEnvironment environment) {
            _environment = environment;
        }
        [BindProperty]
        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "Cần có {0} để mã hóa ")]
        [Display(Name = "File Mã Hóa")]
        public IFormFile FileUpload { get; set; } // la chuỗi , nên cái đuôi ảnh kia ngu vl


        [BindProperty]
        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "Cần có {0} để giải mã ")]
        [Display(Name = "Key Mã Hóa")]
        public IFormFile KeyDecrypt { get; set; }

////////////////////////////////////////// DECRYPTING FILE ///////////////////////////////////
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

      
        public string StatusMessage { get; set; } ="";
        public IActionResult OnGet()
        {
            return Page();
        }

        public void OnPost(){
            
            if (ModelState.IsValid == true)
            {
                // Declare Variables
                bool flag = false;
                JsonFile json = new JsonFile();
                byte[] privateKey = new byte[0];

                // Kiem tra privateKey Hash co giong voi trong keysaver ko
                if(KeyDecrypt != null){
                    //using (FileStream fsKey = new FileStream(File.ReadAllBytes(KeyDecrypt)
                    // Doc PrivateKey tu file 
                    string keyStr = new StreamReader(KeyDecrypt.OpenReadStream()).ReadToEnd();
                    
                    // Decode tu Base64 ve byte[]
                    privateKey = Convert.FromBase64String(keyStr);
                    // Hash private Key SHA-1
                    byte[] HashPrivateKey = SHA1.HashData(privateKey);
                    
                    //////////////////////// Kiem tra Hash Co giong nhau khong ////////////////
                    // Lay Hash tu trong keysaver
                    string fileStoreKey = Path.Combine(_environment.WebRootPath, "keysaver", FileUpload.FileName + "-metadata.json");
                    string keyStoredDb = System.IO.File.ReadAllText(fileStoreKey);
                    // Class JsonFile gom Kx va HKPrivate
                    json = JsonSerializer.Deserialize<JsonFile>(keyStoredDb);
                    byte[] loadKey = Convert.FromBase64String(json.HKPrivate);
                    // Kiem tra HashPrivateKey
                    // Console.WriteLine(loadKey);
                    // Console.WriteLine(HashPrivateKey);
                    if(json.HKPrivate ==  Convert.ToBase64String(HashPrivateKey)) { // Đúng Key Private{
                        // Đặt lại điều kiện để đi giải mã file
                        flag = true;
                    }
                    else{
                        throw new ArgumentNullException(json.HKPrivate + " != " + Convert.ToBase64String(HashPrivateKey));
                    }
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

                    //////////////////////// Giải Kx để lấy lại AESKey ////////////////
                    // Tham số cần : 
                    // PrivateKey: privateKey byte[]
                    // Kx :        json.Kx    string
                    byte[] KxBytes = Convert.FromBase64String(json.Kx);
                    byte[] Ks = new byte[0];

                    byte[] IVBytes = Convert.FromBase64String(json.IV);

                    using (RSA rsa = RSA.Create()){
                        rsa.ImportRSAPrivateKey(privateKey, out _);
                        // RSAParameters privateKeyParameters = 
                        // Giải mã Ks
                        Ks = rsa.Decrypt(KxBytes, RSAEncryptionPadding.OaepSHA256);
                    }
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