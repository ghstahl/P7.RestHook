using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P7.RestHook.ClientManagement;
using P7.RestHook.ClientManagement.Models;

namespace RestHookHost.ViewComponents
{
    public class ClientViewComponent : ViewComponent
    {
        private IRestHookClientManagementStore _restHookClientManagementStore;
        private IHttpContextAccessor _contextAccessor;

        public ClientViewComponent(IHttpContextAccessor contextAccessor,
            IRestHookClientManagementStore restHookClientManagementStore)
        {
            _contextAccessor = contextAccessor;
            _restHookClientManagementStore = restHookClientManagementStore;
        }

        public HookUserClientRecord HookUserClientRecord { get; private set; }
        public HookUserWithClients HookUserWithClients { get; private set; }
        public async Task<IViewComponentResult> InvokeAsync(string clientId)
        {
            HookUserClientRecord = null;
            var result = await _restHookClientManagementStore
                    .FindHookUserAsync(_contextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(x => x.Type == "normailzed_id").Value);
            HookUserWithClients = result.Data;
            if (HookUserWithClients != null)
            {
                var clientRecord = HookUserWithClients.Clients.FirstOrDefault(x => x.ClientId == clientId);

                HookUserClientRecord = new HookUserClientRecord()
                {
                    UserId = HookUserWithClients.UserId,
                    ClientWithHookRecords = clientRecord
                };
            }
           
            return View(HookUserClientRecord);
        }
    }
}
 