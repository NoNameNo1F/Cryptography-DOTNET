using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;

namespace Cryptography.Pages
{
    public class DownloadModel : PageModel
    {
            // Khai báo môi trường để lưu file 
        private readonly IWebHostEnvironment _environment;
        public DownloadModel(IWebHostEnvironment environment) {
            _environment = environment;
        }
        //[Required(ErrorMessage ="Chọn file upload")]
        // GIới hạn những đuôi mở rộng được chọn
        //[FileExtensions(Extensions="jpg, png, gif, jpeg,")] 
        // Lúc này cái dòng trên này sẽ thay bằng dòng khác
        // của Attributes CheckFileExtension mà mình định nghĩa
        public IFormFile FileUpload { get; set; } // la chuỗi , nên cái đuôi ảnh kia ngu vl


        public string StatusMessage { get; set; } ="";
        //////////////////////////////////////////
        public bool IsFolderEmpty(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath);
            return files.Length == 0;
        }
        public string FilePath(){
            string folderPath = Path.GetFullPath("./wwwroot/uploads");

            if (IsFolderEmpty(folderPath))
            {
                return "empty";
            }
            else
            {
                return folderPath;
            }
        }
        /////////////////////////////////////////
        public void OnGet()
        {
        }
    
        public async Task<IActionResult> OnPostAsync(List<string> selectedFiles){
        // public void OnPost(){
            // if (selectedFiles != null && selectedFiles.Any())
            // {
            //     var folderPath = "./wwwroot/uploads";
            //     var zipFileName = $"download_{DateTime.Now:yyyyMMddHHmmss}.zip";
            //     var zipFilePath = Path.Combine(folderPath, zipFileName);

            //     using (var zipArchive = new ZipArchive(new FileStream(zipFilePath, FileMode.Create)))
            //     {
            //         foreach (var fileName in selectedFiles)
            //         {
            //             var filePath = Path.Combine(folderPath, fileName);
            //             if (System.IO.File.Exists(filePath))
            //             {
            //                 zipArchive.CreateEntryFromFile(filePath, fileName);
            //             }
            //         }
            //     }

            //     return PhysicalFile(zipFilePath, "application/zip", zipFileName);
            // }

            return Page();
        }
        public void OnPost(){
            StatusMessage = "Chua co lam me gi het";
        }
    }
}
