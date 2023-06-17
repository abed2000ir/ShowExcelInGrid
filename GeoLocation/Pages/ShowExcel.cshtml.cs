using GeoLocation.Models;
using GeoLocation.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using System.IO;
using System.Net.Http;

namespace GeoLocation.Pages
{
    public class ShowExcelModel : PageModel
    {
        private IWebHostEnvironment Environment;
        public CancellationToken cancellationToken { get; set; }
        public static CancellationTokenSource cancellationTokenSource { get; set; }
        public static IList<ExcelData> LsExcelData { get; set; }
        public IList<ExcelData> GridData { get; set; }

        public int _current = 0;
        public int Current { get { return _current; } set { _current = value; } }
        public ShowExcelModel(IWebHostEnvironment _enviroment)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Environment = _enviroment;

            if(cancellationTokenSource==null)
            {
                cancellationTokenSource = new CancellationTokenSource();
            }

        }

        public void OnGet()
        {
        }

        public void OnPostNextData(string current)
        {
            int localcurrent = int.Parse(current);
            if (localcurrent < LsExcelData.Count)
            {
                Current = localcurrent + 20;
                GridData = LsExcelData.Skip(Current).Take(20).ToList();

            }
            RedirectToPage();
        }
        public void OnPostEndData(string current)
        {

            Current = 20;
            if (LsExcelData.Count > 20)
            {
                Current = LsExcelData.Count - 20;
                GridData = LsExcelData.Skip(LsExcelData.Count - 20).Take(20).ToList();
            }
            else
                GridData = LsExcelData;
            RedirectToPage();

        }
        public void OnGetFirstData(string filename)
        {
            ViewData["Sended_File"] = filename;

            Current = 20;
            GridData = LsExcelData.Take(20).ToList();
            RedirectToPage();

        }

        public void OnPostPreviousData(string current)
        {
            int localcurrent = int.Parse(current);
            if (localcurrent > 20)
            {
                Current = localcurrent - 20;
                GridData = LsExcelData.Skip(localcurrent - 20).Take(20).ToList();
            }
            RedirectToPage();
        }

        public void OnPostSortData(string sort, string data)
        {
            try
            {
                int localcurrent = int.Parse(data);

                GridData = LsExcelData.Skip((localcurrent - 20 > 20 ? localcurrent - 20 : 0)).Take(20).ToList();

                Current = localcurrent;
                if (sort == "asc")
                    GridData = GridData.ToList().OrderBy(c => c.Postalcode).ToList();
                else
                    GridData = GridData.ToList().OrderByDescending(c => c.Postalcode).ToList();
            }
            catch (Exception ex)
            {
                RedirectToPage("./Error", "ShowError", new { message = ex.Message });
            }
        }

        private IList<ExcelData> GetExcelContent(string FileFullpath)
        {
            using (var package = new ExcelPackage(new FileInfo(Path.Combine(FileFullpath))))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets[0];
                return sheet.ConvertSheetToObjects<ExcelData>().ToList();
            }

        }

        public IActionResult OnGetProcessExcel([System.Web.Http.FromUri] string request)
        {
            try
            {
                ViewData["Sended_File"] = request;
                string path = Path.Combine(this.Environment.WebRootPath, "Upload");

                if (new string[] { ".xlsx", ".csv" }.Any(new FileInfo(Path.Combine(path, request)).Extension.Equals))
                {

                    LsExcelData = GetExcelContent(Path.Combine(path, request));
                    Current = 20;
                    GridData = LsExcelData.Take(20).ToList();
                    return RedirectToPage("./ShowExcel", "FirstData", new { filename = request });// RedirectToPage("./ShowExcel");
                }
                else
                {
                    return RedirectToPage("./Error", "ShowError", new { message = "File Format is not Excel." });
                }
            }
            catch (Exception ex)
            {
                return RedirectToPage("./Error", "ShowError", new { message = ex.Message });
            }
        }

        private string GetUniqueFileNameInPath(string path,string filename,string extensions )
        {
            if (new FileInfo(Path.Combine(path, filename) + ".log").Exists)
            {
                IList<FileInfo> lsFile = new DirectoryInfo(Path.Combine(path)).GetFiles("*"+ extensions + "*.*.log").OrderBy(c => c.Name).ToList();

                if (lsFile.Count > 0)
                {
                    filename = lsFile[lsFile.Count - 1].Name.Substring(0, lsFile[lsFile.Count - 1].Name.LastIndexOf(".log"));

                    return filename.Split(extensions).First() + extensions + (int.Parse(filename.Split('.').First().Split(extensions).Last()) + 1) + "." + filename.Split('.').Last();
                }
                else
                    return filename.Split('.').First() + extensions+"1." + filename.Split('.').Last();

            }
            return filename;

        }

        public void OnPostCancelExcelFileProcessed()
        {
            cancellationTokenSource.Cancel();
        }

        public async Task<IActionResult> OnPostGetLocations(string filename)
        {
            try
            {
                string path = Path.Combine(this.Environment.WebRootPath, "Upload");

                LsExcelData = GetExcelContent(Path.Combine(path, filename));
                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 5, 0);
                var code = "";
                var responseBody = "";

                filename=GetUniqueFileNameInPath(path, filename, "_Geo");

                foreach (var item in LsExcelData)
                {
                    code = item.Postalcode;

                    try
                    {
                        using HttpResponseMessage response = await client.GetAsync("******" + code, cancellationTokenSource.Token);
                        response.EnsureSuccessStatusCode();
                        responseBody = response.Content.ReadAsStringAsync().Result;

                        
                    }
                    catch
                    {
                        responseBody = "";
                    }
                    if(cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }

                    using (System.IO.StreamWriter sw = new StreamWriter(Path.Combine(path, filename) + ".log", true))
                    {
                        sw.WriteLine($"{code},{responseBody}");
                    }

                }

                return RedirectToPage("./ShowExcel", "FirstData");// RedirectToPage("./ShowExcel");

            }
            catch (Exception ex)
            {

                return RedirectToPage("./Error", "ShowError", new { message = ex.Message });

            }

        }

    }
}
