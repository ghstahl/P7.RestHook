using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P7.RestHook.ClientManagement;

namespace RestHookHost.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private IRestHookClientManagementStore _restHookClientManagementStore;
        private IHttpContextAccessor _contextAccessor;

        public FooterViewComponent(IHttpContextAccessor contextAccessor,
            IRestHookClientManagementStore restHookClientManagementStore)
        {
            _contextAccessor = contextAccessor;
            _restHookClientManagementStore = restHookClientManagementStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            return View();
        }
    }
}