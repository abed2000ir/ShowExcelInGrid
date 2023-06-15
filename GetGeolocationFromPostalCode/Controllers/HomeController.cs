using GetGeolocationFromPostalCode.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Xml.Linq;

namespace GetGeolocationFromPostalCode.Controllers
{



    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [HttpPost]
        public IActionResult FileUpload(IFormFile postedFile)

        {

            if (postedFile == null || postedFile.Length == 0)

                return BadRequest("No file selected for upload...");



            string fileName = Path.GetFileName(postedFile.FileName);

            string contentType = postedFile.ContentType;



            return View();

        }


        [HttpGet]
        public IActionResult GetListOFGeolocation()
        {
            //var package = new ExcelPackage(new FileInfo(@"C:\Samples Project\PostalCodeGeolocation\GetGeolocationFromPostalCode\Uploaded\schoolsData.xlsx"));
            //ExcelWorksheet sheet = package.Workbook.Worksheets[1];
            //var table = sheet.Tables.FirstOrDefault();

            using (System.IO.StreamReader sr = new StreamReader(@"C:\Users\a_rajabian\Documents\PostalCode.txt"))
            {
                while (!sr.EndOfStream)
                {
                    var code = sr.ReadLine();
                    GetAllListGeolocation(code);

                }
            }


            return View();

        }

        public async Task GetAllListGeolocation(string code)
        {
            HttpClient client = new HttpClient();
            client.Timeout = new TimeSpan(0, 5, 0);

            using HttpResponseMessage response = await client.GetAsync("https://map.shatel.ir/geocodes/" + code);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();


            using (System.IO.StreamWriter sw = new StreamWriter(@"C:\Users\a_rajabian\Documents\PostalCode_result.txt", true))
            {
                sw.WriteLine($"{code},{responseBody}");
            }

            // Above three lines can be replaced with new helper method below
            // string responseBody = await client.GetStringAsync(uri);


        }

        [HttpPost]
        public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = "~/Uploaded/";// Path.GetTempFileName();

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }



  
}