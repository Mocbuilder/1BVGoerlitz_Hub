using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _1BVGoerlitz_Hub.Pages
{
    public class KalenderModel : PageModel
    {
        public List<string> PdfPaths { get; set; }

        public void OnGet()
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
    }
}
