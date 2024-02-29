using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

namespace _1BVGoerlitz_Hub.Pages
{
    public class SatzungModel : PageModel
    {
        public string SatzungContent { get; set; }

        public void OnGet()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Satzung.txt");

            SatzungContent = System.IO.File.ReadAllText(filePath);
        }
    }
}





