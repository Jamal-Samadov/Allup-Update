using Microsoft.AspNetCore.Identity;

namespace Allup.DAL.Entities
{
    public class User : IdentityUser
    {
        public string? FristName { get; set; }   
        public string? LastName { get; set; }   
    }
}
