using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RestHookHost.Areas.RestHook.Pages
{
    public class EditClientModel : PageModel
    {
        public string ReturnUrl { get; set; }
        public string ClientId { get; set; }
        public async void OnGetAsync(string clientId)
        {
            ClientId = clientId;
            ReturnUrl = "/RestHook";
        }
        public async Task<IActionResult> OnPostAsync(string clientId)
        {
            var form = Request.Form.ToList();
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