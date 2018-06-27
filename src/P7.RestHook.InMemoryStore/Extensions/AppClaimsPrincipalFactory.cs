using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace P7.RestHook.InMemoryStore.Extensions
{
    public class AppClaimsPrincipalFactory<TUser> : UserClaimsPrincipalFactory<TUser>
        where TUser : IdentityUser
    {
        private static List<string> _knownIds;
        private static List<string> KnownIds => _knownIds ??
                                                                  (_knownIds =
                                                                      new List<string>
                                                                      {
                                                                          ClaimTypes.NameIdentifier
                                                                      });
        public AppClaimsPrincipalFactory(
            UserManager<TUser> userManager
            , IOptions<IdentityOptions> optionsAccessor
        )
            : base(userManager, optionsAccessor)
        {

        }

        Claim FindNormalizedUserIdClaim(ClaimsPrincipal principal)
        {
            var oneMustExistResult = (principal.Claims.Where(item => KnownIds.Contains(item.Type))).ToList();
            var claim = oneMustExistResult.FirstOrDefault();
            return claim ?? null;
        }
        public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            var principal = await base.CreateAsync(user);
            var claimId = FindNormalizedUserIdClaim(principal);
            ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("normailzed_id",claimId.Value));
            /*
             * get more claims.
             * */
            /*
            var claims = await _postAuthClaimsProvider.FetchClaims(principal);
            if (claims != null)
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(claims);
            }
            */
            return principal;
        }
    }
}