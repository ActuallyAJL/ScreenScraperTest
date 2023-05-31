using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;

namespace ScreenScraperTest
{
    public class Scraper
    {
        private readonly ILogger _logger;

        public Scraper(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Scraper>();
        }

        [Function("Scraper")]
        public void Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] Microsoft.Azure.Functions.Worker.Http.HttpRequestData req)
        {
            string fullUrl = "https://mtd.mitsui.com/en";
            List<string> programmerLinks = new List<string>();

            var options = new ChromeOptions()
            {
                BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            };

            options.AddArguments(new List<string>() { "headless", "disable-gpu" });

            var browser = new ChromeDriver(options);
            browser.Navigate().GoToUrl(fullUrl);

            var links = browser.FindElements(By.XPath("//li[not(contains(@class, 'tocsection'))]/a[1]"));
            foreach (var url in links)
            {
                programmerLinks.Add(url.GetAttribute("href"));
            }

            WriteToCsv(programmerLinks);

            //// Specify the URL of the webpage you want to scrape
            //string url = "https://mtd.mitsui.com/en";

            //// Create a new HtmlWeb instance
            //HtmlWeb web = new HtmlWeb();

            //// Load the HTML document from the specified URL
            //HtmlDocument document = web.Load(url);

            //// Select the elements you want to scrape using XPath
            //HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//*/html/body//a");

            //if (nodes != null)
            //{
            //    // Iterate over the selected nodes and print their inner text
            //    foreach (HtmlNode node in nodes)
            //    {
            //        _logger.LogInformation(node.InnerText);
            //    }
            //}
            //else
            //{
            //    _logger.LogInformation("No matching elements found.");
            //}
            //_logger.LogInformation("Scraper function has completed");
        }

        private void WriteToCsv(List<string> links)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var link in links)
            {
                sb.AppendLine(link);
            }

            System.IO.File.WriteAllText("links.csv", sb.ToString());
        }
    }

    public class Employee
    {
        public string MbkLogonId { get; set; } //Employee ID. Must use combination of this and email for back end table
        public string Telex { get; set; } //aka address code
        public string ProfitCenter { get; set; } 
        public string Office { get; set; } //office, division, or both
        public string FullName { get; set; }
        public string Title { get; set; } //job title
        public string Email { get; set; } //email address
        public string ExtensionNo { get; set; } //VOIP phone number
        public string ExternalNo { get; set; } //Office phone number
    }
}
