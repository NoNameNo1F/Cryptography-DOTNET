using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
namespace Cryptography.Pages {
    public class DecryptModel : PageModel
    {
            // Khai báo môi trường để lưu file 
        private readonly IWebHostEnvironment _environment;
        public DecryptModel( IWebHostEnvironment environment) {
            _environment = environment;
        }
        [BindProperty]
        [DataType(DataType.Upload)]
        // [FileExtensions(Extensions ="jpg, png, gif, jpeg")]
        [Required(ErrorMessage = "Cần có {0} để mã hóa ")]
        [Display(Name = "File Mã Hóa")]
        public IFormFile FileUpload { get; set; } // la chuỗi , nên cái đuôi ảnh kia ngu vl


        [BindProperty]
        [DataType(DataType.Text)]
        // [FileExtensions(Extensions ="jpg, png, gif, jpeg")]
        [Required(ErrorMessage = "Cần có {0} để giải mã ")]
        [Display(Name = "Key Mã Hóa")]
        public string KeyDecrypt { get; set; } 

        [Display(Name = "Khóa AES")]
        public string AESKey { get; set; } 
////////////////////////////////////////// DECRYPTING FILE ///////////////////////////////////
        public void DecryptFile(string inputFile, string outputFile, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (inputFile == null || inputFile.Length <= 0)
                throw new ArgumentNullException("inputFile");
            if (outputFile == null || outputFile.Length <= 0)
                throw new ArgumentNullException("outputFile");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Create an Aes object with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                

                // Create the streams used for encryption.
                using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
                using (CryptoStream csDecrypt = new CryptoStream(fsOutput, decryptor, CryptoStreamMode.Write))
                {
                    // Copy the input file data to the encryption stream.
                    fsInput.CopyTo(csDecrypt);
                }
            }
        }

        public string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
        ////////////////////////////////////////// ENCRYPTING FILE ///////////////////////////////////
        public string StatusMessage { get; set; } ="";
        public IActionResult OnGet()
        {
            return Page();
        }

        // public async Task<IActionResult> OnPostAsync(){
        public void OnPost(){
        // public void OnPost(){
            if (ModelState.IsValid == true)
            {

                //_environment.WebRootPath
                //FileUpload.FileName
                bool flag = false;
                if(FileUpload != null){
                    flag = true;
                    var filepath = Path.Combine(_environment.WebRootPath, "uploads", FileUpload.FileName);
                    // tạo biên lưu filestream , file mode, tạo stream để đỗ dữ liệu vào
                    using var filestream = new FileStream(filepath, FileMode.Create);
                    // Binding dữ liệu đến
                    FileUpload.CopyTo(filestream);
                    filestream.Close();
                }
                if(flag){
                    var filePath = Path.Combine(_environment.WebRootPath, "uploads", FileUpload.FileName);
                    var fileEncryptPath = Path.Combine(_environment.WebRootPath, "encrypted", FileUpload.FileName);
                    using (Aes myAes = Aes.Create())
                    {
                        DecryptFile(filePath, fileEncryptPath, myAes.Key , myAes.IV);
                        AESKey = System.Text.Encoding.Default.GetString(myAes.Key);
                    }
                }
                /////////////////////////==FLOW HANDLE==///////////////////////
                // Sinh khóa AES
                // CryptoConfig cc = new CryptoConfig();
                // Aes aes = Aes.Create();
            //     using (Aes myAes = Aes.Create())
            // {

            //     // Encrypt the string to an array of bytes.
            //     byte[] encrypted = EncryptStringToBytes_Aes(original, myAes.Key, myAes.IV);

            //     // Decrypt the bytes to a string.
            //     string roundtrip = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);

            //     //Display the original data and the decrypted data.
            //     Console.WriteLine("Original:   {0}", original);
            //     var str = System.Text.Encoding.Default.GetString(encrypted);
            //     Console.WriteLine("Encrypted: {0}", str);
            //     Console.WriteLine("Round Trip: {0}", roundtrip);
            // }
                // Mã hóa File với AES
                // Lưu file vào folder ./encrypted
                // Sinh khóa PubKey va PrivateKey 
                // Mã AES key bằng PubKey ==> Kx
                // Dùng SHA-1 Hash PrivateKey ==>HKPrivate
                // lưu json 2 key trên vào 1 file {"Kx": ???,"HKPrivate: ???"}
                // He thong xuat ra PrivateKey
                /////////////////////////==FLOW HANDLE==///////////////////////
                // return RedirectToPage("/Download/Download");
                StatusMessage = "Key của bạn là: " + AESKey;
                RedirectToPage("/Download/Download");
            }
            else{
                StatusMessage = "Dữ liệu gửi đến chưa phù hợp";
                // return RedirectToPage("/Download/Download");
            }
        }
    }
}