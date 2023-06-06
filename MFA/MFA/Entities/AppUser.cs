using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MFA.Entities
{
    public class AppUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string  Country { get; set; }
        public AppUser AppUser { get; set; }
        [Required]
        public string AppUserId { get; set; }
    }
}
