using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;


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
        // [FileExtensions(Extensions ="jpg, png, gif, jpeg")]
        [Required(ErrorMessage = "Cần có {0} để mã hóa ")]
        [Display(Name = "File Mã Hóa")]
        public IFormFile FileUpload { get; set; } // la chuỗi , nên cái đuôi ảnh kia ngu vl


        [Display(Name = "Khóa AES")]
        public byte[] AESKey { get; set; } 
////////////////////////////////////////// ENCRYPTING FILE ///////////////////////////////////
        public byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                        // Write all data to the stream.
                        // csEncrypt.Write(plainTextBytes, 0, plainTextBytes.Length);
                        // csEncrypt.FlushFinalBlock();
                        // encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
        public string EncryptFile(string inputFile, string outputFile, byte[] Key, byte[] IV)
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
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                // Khởi tạo encryptor để tiến hành thay đổi stream.
                //ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(Key, IV);
                // Mã 
                // using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                // using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                // using (CryptoStream csEncrypt = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
                // {
                //     // using (StreamWriter encWriter = new(csEncrypt)){
                //     //     encWriter.WriteLine("Testig112312313");
                //     // }
                //     // Copy the input file data to the encryption stream.
                //     fsInput.CopyTo(csEncrypt);
                // }


                byte[] plainText = System.IO.File.ReadAllBytes(inputFile);
                byte[] hashValue = SHA256.HashData(plainText);
                string temp = Convert.ToHexString(hashValue);


                // byte[] cipherText = System.Text.Encoding.UTF8.GetBytes
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // Create a CryptoStream for decryption with the decryptor and MemoryStream
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        // Read the encrypted file data

                        // Write the encrypted data to the CryptoStream
                        csEncrypt.Write(plainText, 0, plainText.Length);
                        csEncrypt.FlushFinalBlock();

                        // Get the decrypted data as a byte array
                        byte[] encryptedData = msEncrypt.ToArray();

                        // Write the decrypted data to the output file
                        System.IO.File.WriteAllBytes(outputFile, encryptedData);
                    }
                }
                return temp;
            }
        }
        
        public string DecryptFile(string inputFile, string outputFile, byte[] Key, byte[] IV)
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
                // aesAlg.Key = Key;
                // aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                // Khởi tạo encryptor để tiến hành thay đổi stream.
                //ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(Key, IV);
                // Mã 
                // using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                // using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
                // using (CryptoStream csDecrypt = new CryptoStream(fsOutput, decryptor, CryptoStreamMode.Write))
                // {
                //     // Copy the input file data to the encryption stream.
                //     fsInput.CopyTo(csDecrypt);
                // }


                string temp= String.Empty;
                // Create a MemoryStream to hold the decrypted data
                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    // Create a CryptoStream for decryption with the decryptor and MemoryStream
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        // Read the encrypted file data
                        byte[] encryptedData = System.IO.File.ReadAllBytes(inputFile);

                        // Write the encrypted data to the CryptoStream
                        csDecrypt.Write(encryptedData, 0, encryptedData.Length);
                       
                        //csDecrypt.FlushFinalBlock();

                        // Get the decrypted data as a byte array
                        byte[] decryptedData = msDecrypt.ToArray();
                
                        byte[] hashValue = SHA256.HashData(decryptedData);
                        temp = Convert.ToHexString(hashValue);
                        // Calculate the checksum of the decrypted data
                        // byte[] checksum = CalculateChecksum(decryptedData);

                        // // Verify the integrity of the decrypted data by comparing the checksum
                        // if (!VerifyChecksum(encryptedData, checksum))
                        // {
                        //     throw new Exception("Integrity check failed. The decrypted data may have been tampered with.");
                        // }

                        // Write the decrypted data to the output file
                        System.IO.File.WriteAllBytes(outputFile, decryptedData);
                    }
                }
                return temp;
            }
        }
        
        ////////////////////////////////////////// ENCRYPTING FILE ///////////////////////////////////

        ////////////////////////////////////////// ENCRYPTING KEY  ///////////////////////////////////
        // public void EncryptKey(string exportFile, byte[] aesKey, byte[] rsaPriKey, byte[] rsaPubKey){
        //     // Kiểm tra tham số 
        //     if (exportFile == null || exportFile.Length <= 0)
        //         throw new ArgumentNullException("exportFile");
        //     if (aesKey == null || aesKey.Length <= 0)
        //         throw new ArgumentNullException("aesKey");
        //     if (rsaPriKey == null || rsaPriKey.Length <= 0)
        //         throw new ArgumentNullException("rsaPriKey");
        //     if (rsaPubKey == null || rsaPubKey.Length <= 0)
        //         throw new ArgumentNullException("rsaPubKey");

        //     // Tạo key RSA và mã hóa key AES
        //     using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider()){
        //         RSAParameters publicKey = rsa.ExportParameters(false);
        //         RSAParameters rsaPubKey = rsa.ExportParameters(false);
        //     }
        // }
        ////////////////////////////////////////// ENCRYPTING KEY  ///////////////////////////////////
        public string StatusMessage { get; set; } ="";
        public IActionResult OnGet()
        {
            return Page();
        }

        // public async Task<IActionResult> OnPostAsync(){
        /////////////////////////////////////////==FLOW HANDLE==//////////////////////////////////////////
        // Mã hóa File với AES                                                                          //
        // Lưu file vào folder ./encrypted                                                              //
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
                    var filepath = Path.Combine(_environment.WebRootPath, "uploads", FileUpload.FileName);
                    // tạo biên lưu filestream , file mode, tạo stream để đỗ dữ liệu vào
                    using var filestream = new FileStream(filepath, FileMode.Create);
                    // Binding dữ liệu đến
                    FileUpload.CopyTo(filestream);
                    filestream.Close();
                }

// 2 Mã File được tải lên với AES Key và file được lưu lại vào trong folder /encrypted
                if(flag){
                    var filePath = Path.Combine(_environment.WebRootPath, "uploads", FileUpload.FileName);
                    var fileEncryptPath = Path.Combine(_environment.WebRootPath, "encrypted", FileUpload.FileName);

                    // Sinh khóa AES va luu lai vao AESKey để mã hóa với RSA
                    string hash1, hash2 = String.Empty;
                    using (Aes myAes = Aes.Create())
                    {
                        hash1 = EncryptFile(filePath, fileEncryptPath, myAes.Key , myAes.IV);
                        //AESKey = System.Text.Encoding.Default.GetString(myAes.Key);
                        AESKey = myAes.Key;
                    }
                    // Sau khi mã hóa thì xóa file Upload lên 
                    System.IO.File.Delete(filePath);
                    // using (RSA myRsa = RSA.Create()){
                    
                    // }
                    using (Aes myAes = Aes.Create())
                    {
                        hash2 = DecryptFile(fileEncryptPath, filePath, AESKey, myAes.IV);
                        //AESKey = System.Text.Encoding.Default.GetString(myAes.Key);
                        // AESKey = myAes.Key;

                    }
                    bool same = Convert.FromHexString(hash1).SequenceEqual(Convert.FromHexString(hash2));
                    Console.WriteLine("Hash1:" + hash1);
                    Console.WriteLine("Hash2:" + hash2);
                    //Display whether or not the hash values are the same.
                    if (same)
                    {
                        
                        Console.WriteLine("The hash codes match.");
                    }
                    else
                    {
                        Console.WriteLine("The hash codes do not match.");
                    }
                }
                // if(flag){
                //     var fileEncryptPath = Path.Combine(_environment.WebRootPath, "encrypted", FileUpload.FileName);
                //     var fileDecryptPath = Path.Combine(_environment.WebRootPath, "uploads", FileUpload.FileName);
                //     // Lấy khóa để mã hóa
                //     using (Aes myAes = Aes.Create())
                //     {
                //         DecryptFile(fileEncryptPath, fileDecryptPath, AESKey, myAes.IV);
                //         //AESKey = System.Text.Encoding.Default.GetString(myAes.Key);
                //         // AESKey = myAes.Key;

                //     }
                // }
                
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
                // return RedirectToPage("/Download/Download");
                StatusMessage = "Key của bạn là: " + Convert.ToBase64String(AESKey);
                //StatusMessage = "Key của bạn là: ";
                RedirectToPage("/Download/Download");
            }
            else{
                StatusMessage = "Dữ liệu gửi đến chưa phù hợp";
                // return RedirectToPage("/Download/Download");
            }
        }
    }
}