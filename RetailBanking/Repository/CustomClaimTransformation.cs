using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace RetailBanking.Repository
{
    public class CustomClaimsTransformation
        : IClaimsTransformation
    {
        private readonly RetailBankingDbContext _dbcontext;

        public CustomClaimsTransformation(RetailBankingDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identity as ClaimsIdentity;

            var oid = principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (!string.IsNullOrEmpty(oid))
            {
                var role = await _dbcontext.UserRoles
                    .Where(x => x.UserObjectId == oid)
                    .Select(x => x.RoleName)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(role))
                {
                    identity!.AddClaim(
                        new Claim(ClaimTypes.Role, role));
                }
            }

            return principal;
        }
    }
}
