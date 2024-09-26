using PuppeteerSharp;
using PuppeteerSharp.Media;
namespace Url2Pdf.Services
{
    public class PdfService
    {

        public async Task<byte[]> ConvertUrlToPdfAsync(string url, bool isLandscape)
        {

            await new BrowserFetcher().DownloadAsync();


            var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });


            var page = await browser.NewPageAsync();


            await page.GoToAsync(url, null, [WaitUntilNavigation.Load]);

            var pdfOptions = new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                Landscape = isLandscape,


                MarginOptions = new MarginOptions
                {
                    Top = "0",
                    Right = "0",
                    Bottom = "0",
                    Left = "0"
                }
            };
            var pdfBytes = await page.PdfDataAsync(pdfOptions);

            await browser.CloseAsync();

            return pdfBytes;
        }
    }
}
