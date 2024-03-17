using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _1BVGoerlitz_Hub.Pages
{
    public class ManageFilesModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ManageFilesModel(IConfiguration configuration)
        {
            _configuration = configuration;
            LoadSecureConfig();
        }

        [BindProperty(SupportsGet = true)]
        public string Password { get; set; }

        public bool IsPasswordCorrect { get; private set; }
        public List<string> PdfPaths { get; set; }

        public void OnGet()
        {
            if (!IsPasswordCorrect) return;
            UpdatePdfPaths();
        }

        public IActionResult OnPostCheckPassword()
        {
            var correctPassword = _configuration["AppSettings:AdminPassword"].ToLower();
            IsPasswordCorrect = Password == correctPassword;
            if (Password == correctPassword) IsPasswordCorrect = true;
            if (!IsPasswordCorrect) return RedirectToPage("/Index");
            UpdatePdfPaths();
            return Page();
        }

        public IActionResult OnPostDelete(string path)
        {
            if (!IsPasswordCorrect) return Forbid();
            try
            {
                if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    UpdatePdfPaths();
                    return RedirectToPage();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the file: {ex.Message}");
            }
        }

        private void UpdatePdfPaths()
        {
            var pdfFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EventPDF");
            PdfPaths = Directory.GetFiles(pdfFolderPath, "*.pdf")
                .Select(filePath => GetRelativePath(filePath))
                .ToList();
        }

        private string GetRelativePath(string filePath)
        {
            var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            return filePath.Replace(wwwRootPath, "").Replace("\\", "/");
        }

        private void LoadSecureConfig()
        {
            var contentRoot = (_configuration as Microsoft.AspNetCore.Hosting.IWebHostEnvironment)?.ContentRootPath ?? Directory.GetCurrentDirectory();
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
        public IActionResult OnPostUpload(Microsoft.AspNetCore.Http.IFormFile file)
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
