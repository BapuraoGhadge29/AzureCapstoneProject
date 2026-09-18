using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using RetailBanking.Repository;
using System.Security.Claims;

namespace RetailBanking.Models
{
    public class RoleClaimsTransformation : IClaimsTransformation
    {
        private readonly RetailBankingDbContext _context;

        public RoleClaimsTransformation(RetailBankingDbContext context)
        {
            _context = context;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identity as ClaimsIdentity;

            if (identity == null ||
                !identity.IsAuthenticated)
            {
                return principal;
            }

            var email =
                principal.FindFirst("preferred_username")?.Value ??
                principal.FindFirst("upn")?.Value ??
                principal.FindFirst("unique_name")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return principal;
            }

            var existingRole =
                identity.Claims.Any(x =>
                    x.Type == ClaimTypes.Role);

            if (existingRole)
            {
                return principal;
            }

            var user =
                await _context.UserRoles
                    .FirstOrDefaultAsync(x =>
                        x.Email == email);

            if (user != null)
            {
                identity.AddClaim(
                    new Claim(
                        ClaimTypes.Role,
                        user.RoleName));
            }

            return principal;
        }
    }
}
