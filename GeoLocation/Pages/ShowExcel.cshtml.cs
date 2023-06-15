using GeoLocation.Models;
using GeoLocation.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;

namespace GeoLocation.Pages
{
    public class ShowExcelModel : PageModel
    {
        private IWebHostEnvironment Environment;
        public static IList<ExcelData> LsExcelData { get; set; }
        public IList<ExcelData> GridData { get; set; }

        public int _current = 0;
        public int Current { get { return _current; } set { _current = value; } }
        public ShowExcelModel(IWebHostEnvironment _enviroment)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            Environment = _enviroment;


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
        public void OnPostFirstData(string current)
        {
          
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
        public IActionResult OnGetProcessExcel([System.Web.Http.FromUri] string request)
        {
            try
            {
                string path = Path.Combine(this.Environment.WebRootPath, "Upload");

                using (var package = new ExcelPackage(new FileInfo(Path.Combine(path, request))))
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets[0];
                    LsExcelData = sheet.ConvertSheetToObjects<ExcelData>().ToList();
                }
                Current = 20;
                GridData = LsExcelData.Take(20).ToList();
                //RedirectToPage("ShowExcel");
                return null;// RedirectToPage("./ShowExcel");


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
