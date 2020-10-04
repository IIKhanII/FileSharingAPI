using System;
using System.ComponentModel.DataAnnotations;

namespace FileSharingAPI.Models
{
    public class FileModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
