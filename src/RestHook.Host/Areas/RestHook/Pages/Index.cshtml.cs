using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using P7.RestHook.ClientManagement;
using P7.RestHook.ClientManagement.Models;

namespace RestHookHost.Areas.RestHook.Pages
{
    public class IndexModel : PageModel
    {
        private IRestHookClientManagementStore _restHookClientManagementStore;
        public HookUserClientsRecord HookUserClientsRecord { get; set; }
        public IndexModel(IRestHookClientManagementStore restHookClientManagementStore)
        {
            _restHookClientManagementStore = restHookClientManagementStore;
        }
        public async void OnGetAsync(string clientId)
        {
            var result =
                await _restHookClientManagementStore.FindHookUserClientsAsync(User.Claims
                    .FirstOrDefault(x => x.Type == "normailzed_id").Value);
            HookUserClientsRecord = result.Data;
            ClientId = clientId;
        }

        public string ClientId { get; set; }
    }
}