using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P7.RestHook.ClientManagement;
using P7.RestHook.ClientManagement.Models;

namespace RestHookHost.ViewComponents
{
    public class ClientsViewComponent : ViewComponent
    {
        private IRestHookClientManagementStore _restHookClientManagementStore;
        private IHttpContextAccessor _contextAccessor;

        public ClientsViewComponent(IHttpContextAccessor contextAccessor,
            IRestHookClientManagementStore restHookClientManagementStore)
        {
            _contextAccessor = contextAccessor;
            _restHookClientManagementStore = restHookClientManagementStore;
        }

        public HookUser HookUser { get; private set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _restHookClientManagementStore
                    .FindHookUserAsync(_contextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(x => x.Type == "normailzed_id").Value);
            HookUser = result.Data;
            return View(HookUser);
        }
    }
}
