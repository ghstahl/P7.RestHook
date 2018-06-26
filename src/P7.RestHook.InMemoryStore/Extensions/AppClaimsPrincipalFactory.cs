using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace P7.RestHook.InMemoryStore.Extensions
{
    public class AppClaimsPrincipalFactory<TUser> : UserClaimsPrincipalFactory<TUser>
        where TUser : IdentityUser
    {
        public AppClaimsPrincipalFactory(
            UserManager<TUser> userManager
            , IOptions<IdentityOptions> optionsAccessor
        )
            : base(userManager, optionsAccessor)
        {

        }

        public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            var principal = await base.CreateAsync(user);
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