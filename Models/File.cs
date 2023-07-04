using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptography.models
{
    // [Table("post")]
    public class File {
        [Column(TypeName ="ntext")]
        // [FileExtensions(Extensions ="jpg, png, gif, jpeg,txt,docx")]
        [Required(ErrorMessage = "Cần có {0} để mã hóa ")]
        [Display(Name = "File Upload")]
        public IFormFile FileUpload { get; set; } // la chuỗi , nên cái đuôi ảnh kia ngu vl
    }
}
