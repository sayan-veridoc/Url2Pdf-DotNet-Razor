using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PuppeteerSharp;
using System.ComponentModel.DataAnnotations;
using Url2Pdf.Services;

namespace Url2Pdf.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly PdfService _pdfService;


        public IndexModel(ILogger<IndexModel> logger, PdfService pdfService)
        {
            _logger = logger;
            _pdfService = pdfService;
        }

        public void OnGet()
        {

        }

        [BindProperty]
        [Required(ErrorMessage = "Please enter a URL.")]
        public string PageUrl { get; set; }

        [BindProperty]
        public bool IsLandscape { get; set; }
        public string DownloadLink { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                
                return Page();
            }

            try
            {
                var uri = new Uri(PageUrl);
                var hostParts = uri.Host.Split('.');
                string fileName = string.Join(".", hostParts, 0, hostParts.Length - 1) + ".pdf";
                var pdfBytes = await _pdfService.ConvertUrlToPdfAsync(PageUrl, IsLandscape);
                string localPath = Path.Combine("wwwroot", "pdfs"); 
                Directory.CreateDirectory(localPath); 
                string filePath = Path.Combine(localPath, fileName);

                // Save the PDF file locally
                await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

                DownloadLink = Url.Page("Index", "Download", new { fileName }); 
                return Page();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return Page();
            }
        }
        
        public IActionResult OnGetDownload(string fileName)
        {
            var filePath = Path.Combine("wwwroot", "pdfs", fileName);

            if (System.IO.File.Exists(filePath))
            {
                var pdfBytes = System.IO.File.ReadAllBytes(filePath);

                // Serve the file for download
                var result = File(pdfBytes, "application/pdf", fileName);

                // Delete the file after serving it
                System.IO.File.Delete(filePath);

                return result;
            }

            return NotFound();
        }
    }
}
