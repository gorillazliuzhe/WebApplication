using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using WebApplication.Utilities;


namespace WebApplication.Controllers
{
    public class UpLoadFileController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UpLoadFileController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 表单提交上传 通过IFormFile参数获取上传文件信息
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            foreach (var file in files)
            {
                if (file.Length <= 0) continue;
                var fileName = file.FileName;
                var fileDir = Path.Combine(_webHostEnvironment.WebRootPath, "UploadFiles");
                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }
                string filePath = fileDir + $@"/{fileName}";
                size += file.Length;
                FileStream fileStream = System.IO.File.Create(filePath);
                await using FileStream fs = fileStream;
                file.CopyTo(fs);
                fs.Flush();
            }
            await Tools.ToExcelAsync(_webHostEnvironment);
            return Ok(new { count = files.Count, size });
        }

        /// <summary>
        /// AJAX请求上传，通过Request.Form.Files获取上传文件信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AjaxUploadFiles()
        {
            long size = 0;
            var files = Request.Form.Files;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var file in files)
            {
                var fileName = file.FileName;
                var fileDir = Path.Combine(_webHostEnvironment.WebRootPath, "UploadFiles");
                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }
                string filePath = fileDir + $@"\{fileName}";
                size += file.Length;
                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                stringBuilder.AppendLine($"文件\"{fileName}\" /{size}字节上传成功 <br/>");
            }
            stringBuilder.AppendLine($"共{files.Count}个文件 /{size}字节上传成功! <br/>");
            ViewBag.Message = stringBuilder.ToString();
            return Json(new
            {
                msg = stringBuilder.ToString()
            });
        }
    }
}