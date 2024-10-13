using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using RoadOfGroping.Common.DependencyInjection;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.MiniExcels
{
    public class MiniExcelManager : IMiniExcelManager, ITransientDependency
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MiniExcelManager(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SaveToExcelAsync<T>(string fileName, List<T> data)
        {
            var path = PathHandler(fileName);
            await MiniExcel.SaveAsAsync(path, data);
            return path;
        }

        public async Task<IEnumerable<T>> LoadFromExcelAsync<T>(string filePath) where T : class, new()
        {
            return await MiniExcel.QueryAsync<T>(filePath);
        }

        public async Task InsertIntoExcelAsync<T>(string filePath, List<T> data, string sheetName = "Sheet1", ExcelType type = ExcelType.UNKNOWN)
        {
            MiniExcel.Insert(filePath, data, sheetName, type);
            await Task.CompletedTask;
        }

        public async Task<FileResult> DownloadExcelFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("文件未找到", filePath);
            }

            var fileName = Path.GetFileName(filePath);
            var fileContent = await File.ReadAllBytesAsync(filePath);

            return new FileContentResult(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = fileName
            };
        }

        private string PathHandler(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            var now = DateTime.Now;
            var webrootpath = _webHostEnvironment.WebRootPath;
            string dynamicPath = $"default-host/Excel/{now.Year}-{now.Month:D2}/{Guid.NewGuid().ToString("N")}/";
            string s = Path.Combine(webrootpath, dynamicPath);
            if (!Directory.Exists(s))
            {
                Directory.CreateDirectory(s);
            }
            return $"{s}{fileName}";
        }
    }
}
