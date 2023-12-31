﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using GeoLocation.Models;
using GeoLocation.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Web;

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
               return RedirectToPage("./ShowExcel","ProcessExcel",new { request = request });
            }
            catch (Exception ex)
            {
                return RedirectToPage("./Error", "ShowError", new { message = ex.Message });

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
                return RedirectToPage("./Error", "ShowError", new { message = ex.Message });

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
                return RedirectToPage("./Error", "ShowError", new { message = ex.Message });

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
                 RedirectToPage("./Error", "ShowError", new { message = ex.Message });

            }
        }


        public IActionResult OnPostDownloadFile([System.Web.Http.FromUri] string request)
        {

            string wwwpath = this.Environment.WebRootPath;
            string contentPath = this.Environment.ContentRootPath;
            string path = Path.Combine(this.Environment.WebRootPath, "Upload");
            string Filepath = Path.Combine(path, request);
            
            return File(
                Environment
                    .WebRootFileProvider
                    .GetFileInfo(@"\Upload\" + request)
                    .CreateReadStream(),
                GetContentType(Filepath));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if(types.ContainsKey(ext))
            {
                return types[ext];

            }
            else
                // RedirectToPage("./Error", "ShowError", new { message = "" });
            return "*/*";
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/txt"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}