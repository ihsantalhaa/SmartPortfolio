using Microsoft.AspNetCore.Identity;

namespace SmartPortfolio.Models
{
    public class IUser : IdentityUser<int>
    {
        public ICollection<Portfolio>? Portfolios { get; set; }
    }
}
