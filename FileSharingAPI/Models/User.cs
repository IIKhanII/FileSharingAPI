using System;
using System.ComponentModel.DataAnnotations;

namespace FileSharingAPI.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
    }
}
