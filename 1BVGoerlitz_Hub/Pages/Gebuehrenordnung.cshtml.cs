using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _1BVGoerlitz_Hub.Pages
{
    public class GebuehrenordnungModel : PageModel
    {
        public string GebuehrenordnungContent { get; set; }
        public void OnGet()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Content", "GebuehrenOrdnung.txt");

            GebuehrenordnungContent = System.IO.File.ReadAllText(filePath);
        }
    }
}
