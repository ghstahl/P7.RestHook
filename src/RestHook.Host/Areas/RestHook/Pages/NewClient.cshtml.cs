using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using P7.RestHook;
using P7.RestHook.ClientManagement;
using P7.RestHook.ClientManagement.Models;

namespace RestHookHost.Areas.RestHook.Pages
{
    public class NewClientInputModel
    {
        [Required]
        [Display(Name = "ClientId")]
        public string ClientId { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }

    public class NewClientModel : PageModel
    {
        private IRestHookClientManagementStore _restHookClientManagementStore;
        [BindProperty] public NewClientInputModel Input { get; set; }
        public string ReturnUrl { get; set; }

        public NewClientModel(IRestHookClientManagementStore restHookClientManagementStore)
        {
            _restHookClientManagementStore = restHookClientManagementStore;
        }
        public async void OnGetAsync(string newClientReturnUrl)
        {
            var clientId = Unique.G;
            Input = new NewClientInputModel()
            {
                ClientId = clientId,
                Description = $"Description for {clientId}"
            };
            ReturnUrl = newClientReturnUrl??"/RestHook";
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var userId = User.Claims
                    .FirstOrDefault(x => x.Type == "normailzed_id").Value;
                var result =
                    await _restHookClientManagementStore.FindHookUserAsync(userId);
                var record = result.Data;
                if (record == null)
                {
                    var result2 = await _restHookClientManagementStore.UpsertHookUserAsync(userId);
                    record = result2.Data;
                }

                var clientRecordResult = await _restHookClientManagementStore.CreateProducerHookClientAsync(record.UserId);
                var clientRecord = clientRecordResult.Data;
                var index = returnUrl.IndexOf("?", StringComparison.Ordinal);
                var separator = index < 0 ? "?" : "&";
                return LocalRedirect($"{returnUrl}{separator}clientId={clientRecord.ClientId}");
            } 
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}