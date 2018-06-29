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
        public HookUserClientsRecord HookUserClientsRecord { get; private set; }
        public async Task<IViewComponentResult> InvokeAsync(string clientId)
        {
            HookUserClientRecord = null;
            HookUserClientsRecord = await _restHookClientManagementStore
                    .FindHookUserClientAsync(_contextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(x => x.Type == "normailzed_id").Value);
            if (HookUserClientsRecord != null)
            {
                var clientRecord = HookUserClientsRecord.Clients.FirstOrDefault(x => x.ClientId == clientId);

                HookUserClientRecord = new HookUserClientRecord()
                {
                    UserId = HookUserClientsRecord.UserId,
                    Client = clientRecord
                };
            }
           
            return View(HookUserClientRecord);
        }
    }
}
