using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using P7.RestHook.ClientManagement;

namespace RestHookHost.Areas.RestHook.Pages
{
    public class EditClientModel : PageModel
    {
        private IRestHookClientManagementStore _restHookClientManagementStore;
        public string ReturnUrl { get; set; }
        public string ClientId { get; set; }

        public EditClientModel(IRestHookClientManagementStore restHookClientManagementStore)
        {
            _restHookClientManagementStore = restHookClientManagementStore;
        }
        public async void OnGetAsync(string clientId)
        {
            ClientId = clientId;
            ReturnUrl = "/RestHook";
        }
        public async Task<IActionResult> OnPostAsync(string clientId)
        {
            var hookUserClientsRecord =
                await _restHookClientManagementStore.FindHookUserClientsAsync(User.Claims
                    .FirstOrDefault(x => x.Type == "normailzed_id").Value);

            var form = Request.Form.ToList();
            var result = await _restHookClientManagementStore.UpdateAsync(hookUserClientsRecord.Data);

            List<string> lstString = new List<string>
            {
                "Val 1",
                "Val 2",
                "Val 3"
            };
            return new JsonResult(form);
        }
    }
}