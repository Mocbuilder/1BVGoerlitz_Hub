using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;


namespace _1BVGoerlitz_Hub.Pages
{
    public class ManageFilesModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ManageFilesModel(IConfiguration configuration)
        {
            _configuration = configuration;

            // Load sensitive information from the secure configuration file
            LoadSecureConfig();
        }

        [BindProperty(SupportsGet = true)]
        public string Password { get; set; }

        public bool IsPasswordCorrect { get; private set; }
        public List<string> PdfPaths { get; set; }

        public void OnGet()
        {
            // If the password is not correct, do not load PdfPaths
            if (!IsPasswordCorrect) return;

            UpdatePdfPaths();
        }

        public IActionResult OnPostCheckPassword()
        {
            // Retrieve the correct password from the configuration
            var correctPassword = _configuration["AppSettings:AdminPassword"].ToLower();

            IsPasswordCorrect = Password == correctPassword;

            if (!IsPasswordCorrect)
            {
                // Redirect to the homepage if the password is incorrect
                return RedirectToPage("/Index");
            }

            // Continue to display the Manage Files content
            UpdatePdfPaths();
            return Page();
        }


        private void UpdatePdfPaths()
        {
            var pdfFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EventPDF");

            PdfPaths = Directory.GetFiles(pdfFolderPath, "*.pdf")
                .Select(filePath => GetRelativePath(filePath))
                .ToList();
        }

        // Helper method to get the relative path of a file
        private string GetRelativePath(string filePath)
        {
            var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            return filePath.Replace(wwwRootPath, "").Replace("\\", "/");
        }

        private void LoadSecureConfig()
        {
            var contentRoot = (_configuration as IWebHostEnvironment)?.ContentRootPath ?? Directory.GetCurrentDirectory();
            var secureConfigPath = Path.Combine(contentRoot, "SecureConfig.json");

            try
            {
                if (System.IO.File.Exists(secureConfigPath))
                {
                    var secureConfig = System.IO.File.ReadAllText(secureConfigPath);
                    _configuration.GetSection("AppSettings").Bind(JsonConvert.DeserializeObject<Dictionary<string, string>>(secureConfig));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OnPostUpload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                if (file.ContentType == "application/pdf" && file.Length <= 10485760)
                {
                    var pdfFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EventPDF");
                    var fileName = Path.Combine(pdfFolderPath, file.FileName);

                    using (var stream = new FileStream(fileName, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    UpdatePdfPaths();
                }
                else
                {
                    ModelState.AddModelError("File", "Invalid file format or size");
                    return RedirectToPage("/Index");
                }
            }

            return RedirectToPage("/Kalender");
        }

    }
}
