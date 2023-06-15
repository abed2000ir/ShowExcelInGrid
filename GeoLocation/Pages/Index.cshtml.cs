using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using GeoLocation.Models;
using GeoLocation.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace GeoLocation.Pages
{
    public class IndexModel : PageModel
    {
        private IWebHostEnvironment Environment;
        public string Message { get; set; }

        public IList<FileModel> ListOfFiles { get; set; }

        public IndexModel(IWebHostEnvironment _enviroment)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            Environment = _enviroment;
            OnGet();
        }
        public IActionResult OnPostProcessExcel([System.Web.Http.FromUri] string request)
        {
            try
            {
                //string path = Path.Combine(this.Environment.WebRootPath, "Upload");

                //using (var package = new ExcelPackage(new FileInfo(Path.Combine(path, request))))
                //{
                //    ExcelWorksheet sheet = package.Workbook.Worksheets[0];
                //     var ThatList = sheet.ConvertSheetToObjects<ExcelData>().ToList();
                //}
                //OnGet();
               return RedirectToPage("./ShowExcel","ProcessExcel",new { request = request });
               

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult OnPostDeleteFile([System.Web.Http.FromUri] string request)
        {
            try
            {
                string Name = request;//.FileName;

                string path = Path.Combine(this.Environment.WebRootPath, "Upload");
                System.IO.File.Delete(path + "\\" + request);

                OnGet();
                return RedirectToPage();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult OnPostUpload(IList<IFormFile> postedFiles)
        {
            try
            {
                string wwwpath = this.Environment.WebRootPath;
                string contentPath = this.Environment.ContentRootPath;
                string path = Path.Combine(this.Environment.WebRootPath, "Upload");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                List<string> uploadFiles = new List<string>();
                foreach (IFormFile postedFile in postedFiles)
                {
                    string filename = Path.GetFileName(postedFile.FileName);
                    using (FileStream stream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                        uploadFiles.Add(filename);
                        this.Message += string.Format("<b>{0}</b> uploaded. </br>", filename);
                    }

                }
                OnGet();
                return RedirectToPage();


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void OnGet()
        {
            try
            {
                string path = Path.Combine(this.Environment.WebRootPath, "Upload");
                IList<FileModel> LsfileModel = new List<FileModel>();
                var source = Directory.GetFiles(path);
                int count = 1;
                if (source == null || source.Count() > 0)
                {
                    foreach (var item in source)
                    {
                        FileInfo fileInfo = new FileInfo(item);

                        LsfileModel.Add(new()
                        {
                            Id = count++,
                            Name = fileInfo.Name
                            ,
                            Extension = fileInfo.Extension
                            ,
                            Url = fileInfo.FullName
                            ,
                            Size = (fileInfo.Length > 0 ? (

                            (fileInfo.Length / 1024 > 1024 ?
                                    (fileInfo.Length / (1024 * 1024)).ToString() + " Mb"
                                                : (fileInfo.Length / 1024).ToString() + " Kb")) : "0")
                            ,
                            Description = fileInfo.CreationTime.ToString()
                        });
                    }
                }
                ListOfFiles = LsfileModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}