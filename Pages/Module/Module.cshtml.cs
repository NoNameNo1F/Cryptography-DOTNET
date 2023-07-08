using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;
using System.Text.Json;

namespace Cryptography.Pages {
    
    public class ModuleModel : PageModel
    {
            // Khai báo môi trường để lưu file 
        private readonly IWebHostEnvironment _environment;
        public ModuleModel( IWebHostEnvironment environment) {
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




        // MODULE 5 //
        ////////////////////////////////////////// MODULE: ENCRYPTING STRING(RSA) ///////////////////////////////////
        public byte[] EncryptString(byte[] message, byte[] publicKey){
            using (RSA rsa = RSA.Create()){
                rsa.ImportRSAPublicKey(publicKey, out _);
                return rsa.Encrypt(message, RSAEncryptionPadding.OaepSHA256);
            }
        }
        ////////////////////////////////////////// MODULE: ENCRYPTING STRING(RSA) ///////////////////////////////////




        // MODULE 6 //
        ////////////////////////////////////////// MODULE: DECRYPTING STRING(RSA) ///////////////////////////////////
         public byte[] DecryptString(byte[] cipher, byte[] privateKey){
            using (RSA rsa = RSA.Create()){
                rsa.ImportRSAPrivateKey(privateKey, out _);
                return rsa.Decrypt(cipher, RSAEncryptionPadding.OaepSHA256);
            }
        }
        ////////////////////////////////////////// MODULE: DECRYPTING STRING(RSA) ///////////////////////////////////



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
        
        public IActionResult OnPost(){
            string? options = this.Request.Form["moduleOp"];
            switch (options){
                case "Module-1":{
                    return RedirectToPage("/Module/Module1");
                }
                case "Module-2":{
                    return RedirectToPage("/Module/Module2");
                }
                case "Module-3":{
                    return RedirectToPage("/Module/Module3");            
                }
                case "Module-4":{
                    return RedirectToPage("/Module/Module4");
                }
                case "Module-5":{
                    return RedirectToPage("/Module/Module5");
                }
                case "Module-6":{
                    return RedirectToPage("/Module/Module6");
                }
                case "Module-7":{
                    return RedirectToPage("/Module/Module7"); 
                }
                default:
                    StatusMessage = "Lựa chọn của bạn không tồn tại!!";
                    return RedirectToPage("/Module/Module");
            }
        }
    }
}