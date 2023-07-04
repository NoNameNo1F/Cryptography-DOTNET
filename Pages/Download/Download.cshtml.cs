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

        public void OnGet()
        {
        }
        public void OnPost(){
        }
    }
}
