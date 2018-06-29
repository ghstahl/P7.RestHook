using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using P7.RestHook.ClientManagement;

namespace RestHookHost.Areas.RestHook.Pages
{
    public class NewCallbackUrlInputModel
    {
        public int Number { get; set; }

        [Required]
        [Display(Name = "Callback Url")]
        [Url]
        public string CallbackUrl { get; set; }

        [Required]
        [Display(Name = "Items")]
        public List<SelectListItem> Items =>
            Enumerable.Range(1, 3).Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = $"Text {x.ToString()}"
            }).ToList();
    }
    public class NewCallbackUrlModel : PageModel
    {
        [BindProperty] public string ClientId { get; set; }
        private IRestHookClientManagementStore _restHookClientManagementStore;
        [BindProperty] public NewCallbackUrlInputModel Input { get; set; }
        [BindProperty] public string ReturnUrl { get; set; }

        public NewCallbackUrlModel(IRestHookClientManagementStore restHookClientManagementStore)
        {
            _restHookClientManagementStore = restHookClientManagementStore;
        }
        public async void OnGetAsync(string newCallbackUrlClientId,
            string newCallbackUrlReturnUrl = null)
        {
            ClientId = newCallbackUrlClientId;
            Input = new NewCallbackUrlInputModel();
            ReturnUrl = newCallbackUrlReturnUrl ?? "/RestHook";
        }

        

        public async Task<IActionResult> OnPostAsync(
            string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {

                return LocalRedirect($"{returnUrl}");
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}