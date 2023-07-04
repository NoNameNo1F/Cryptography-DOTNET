using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;
using System.Text.Json;

namespace Cryptography.Pages {
    public class EncryptModel : PageModel
    {
            // Khai báo môi trường để lưu file 
        private readonly IWebHostEnvironment _environment;
        public EncryptModel( IWebHostEnvironment environment) {
            _environment = environment;
        }
        [BindProperty]
        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "Cần có {0} để mã hóa ")]
        [Display(Name = "File Mã Hóa")]
        public IFormFile FileUpload { get; set; }


        [Display(Name = "Khóa AES")]
        public byte[] AESKey { get; set; } 
        [Display(Name = "IV")]
        public byte[] IV { get; set; } 
////////////////////////////////////////// ENCRYPTING FILE ///////////////////////////////////
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
        ////////////////////////////////////////// ENCRYPTING FILE ///////////////////////////////////
  
        ////////////////////////////////////////// HASHING PRIVATE KEY ///////////////////////////////////
        public byte[] HashPrivateKey(byte[] privateK){
            // Hash voi dang SHA-1
            return SHA1.HashData(privateK);
        }
        ////////////////////////////////////////// HASHING PRIVATE KEY ///////////////////////////////////
        public string StatusMessage { get; set; } ="";
        public IActionResult OnGet()
        {
            return Page();
        }
        
        // public async Task<IActionResult> OnPostAsync(){
        /////////////////////////////////////////==FLOW HANDLE==//////////////////////////////////////////
        // Mã hóa File với AES                           (DONE)                                         //
        // Lưu file vào folder ./encrypted                (DONE)                                        //
        // Sinh khóa PubKey va PrivateKey                                                               //
        // Mã AES key bằng PubKey ==> Kx                                                                //
        // Dùng SHA-1 Hash PrivateKey ==>HKPrivate  Key                                                 //
        // lưu json 2 key trên vào 1 file {"Kx": ???,"HKPrivate: ???"}                                  //
        // He thong xuat ra PrivateKey  ==>                                                             //
        /////////////////////////////////////////==FLOW HANDLE==//////////////////////////////////////////
        public void OnPost(){
        // public void OnPost(){
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
                byte[] Kx = new byte[0];
                byte[] HKPrivate = new byte[0];
                byte[] kPrivate = new byte[0];
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
                    // Tạo RSA key để mã hóa Ks
                    using (RSA rsa = RSA.Create()){
                        byte[] kPublic = rsa.ExportRSAPublicKey();
                        kPrivate = rsa.ExportRSAPrivateKey();
                        //Kx = rsa.EncryptValue(AESKey);
                        Kx = rsa.Encrypt(AESKey, RSAEncryptionPadding.OaepSHA256);
                        HKPrivate = HashPrivateKey(kPrivate);
                    }

                    // Tao json va luu vao file: Kx và HKPrivate
                    var jsonKey = new {
                        Kx = Convert.ToBase64String(Kx),
                        IV = Convert.ToBase64String(IV),
                        HKPrivate = Convert.ToBase64String(HKPrivate)
                    };
                    // Serialize the JSON object
                    string jsonString = JsonSerializer.Serialize(jsonKey);

                    // Save the JSON to a file
                    string nameSaveKey = FileUpload.FileName + "-metadata.json";
                    string fileSaveKey = Path.Combine(_environment.WebRootPath, "keysaver", nameSaveKey);
                    
                    System.IO.File.WriteAllText(fileSaveKey, jsonString);
                    // // Tao file chua keyPrivate tam thoi , khi nguoi dung download key thi se xoa no
                    // string tempSaveKey = Path.Combine(_environment.WebRootPath, "keysaver", FileUpload.FileName + "-key.txt");
                    // System.IO.File.WriteAllText(tempSaveKey, Convert.ToBase64String(kPrivate));
                }
                StatusMessage = Convert.ToBase64String(kPrivate);
                //RedirectToPage("/Download/Download");
            }
            else{
                StatusMessage = "Dữ liệu gửi đến chưa phù hợp";
                // return RedirectToPage("/Download/Download");
            }
        }
    }
}