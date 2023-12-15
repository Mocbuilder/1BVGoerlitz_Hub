using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _1BVGoerlitz_Hub.Pages
{
    public class MitgliedschaftModel : PageModel
    {
        public string TrainingsTimeContent { get; set; }
        public void OnGet()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Trainingszeiten.txt");

            TrainingsTimeContent = System.IO.File.ReadAllText(filePath);
        }
    }
}
