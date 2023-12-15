using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _1BVGoerlitz_Hub.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        public string ImpressumContent { get; set; }

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Impressum.txt");

            ImpressumContent = System.IO.File.ReadAllText(filePath);
        }
    }

}
