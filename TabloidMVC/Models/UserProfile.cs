using Microsoft.Data.SqlClient.Server;
using System;
using System.ComponentModel;
using System.Runtime.Intrinsics.X86;

namespace TabloidMVC.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DisplayName("Display Name")]
        public string DisplayName { get; set; }
        public string Email { get; set; }

        [DisplayName("Join Date")]
        public DateTime CreateDateTime { get; set; }
        public string ImageLocation { get; set; }
        public bool Active { get; set; }
        public int UserTypeId { get; set; }
        public UserType UserType { get; set; }

        [DisplayName("Name")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}