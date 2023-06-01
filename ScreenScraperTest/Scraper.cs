using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
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
            string fullUrl = "https://mtd.mitsui.com/en/TelephoneDirectory/DivisionPage?dp=DP20160513113";
            List<string> programmerLinks = new List<string>();

            var options = new EdgeOptions()
            {
                BinaryLocation = "C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe"
            };
            Console.WriteLine("Options created");

            options.AddArguments(new List<string>() { "headless", "disable-gpu" });
            Console.WriteLine("Added options to arguments");

            var service = EdgeDriverService.CreateDefaultService();
            Console.WriteLine("Service Created");
            service.Port = 40000;
            service.Start();
            Console.WriteLine("Service started");

            var browser = new EdgeDriver(service, options);
            Console.WriteLine("Browser Initialized");
            browser.Navigate().GoToUrl(fullUrl);
            Console.WriteLine("Browser navigated to URL");

            var links = browser.FindElements(By.XPath("//li[not(contains(@class, 'tocsection'))]/a[1]"));
            foreach (var url in links)
            {
                programmerLinks.Add(url.GetAttribute("href"));
            }

            Console.WriteLine("Links collected. About to save to CSV");
            WriteToCsv(programmerLinks);
            Console.WriteLine("Saved to CSV");
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
