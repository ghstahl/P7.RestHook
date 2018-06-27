using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        public async void OnGetAsync()
        {
            var clientId = Unique.G;
            Input = new NewClientInputModel()
            {
                ClientId = clientId,
                Description = $"Description for {clientId}"
            };
            ReturnUrl = "/RestHook";
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var userId = User.Claims
                    .FirstOrDefault(x => x.Type == "normailzed_id").Value;
                var record =
                    await _restHookClientManagementStore.FindHookUserClientAsync(userId);
                if (record == null)
                {
                    record = await _restHookClientManagementStore.CreateHookUserClientAsync(userId);
                }

                await _restHookClientManagementStore.AddClientAsync(record, new ClientRecord(){ClientId = Input.ClientId,Description = Input.Description});
                return LocalRedirect(returnUrl);
            } 
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}