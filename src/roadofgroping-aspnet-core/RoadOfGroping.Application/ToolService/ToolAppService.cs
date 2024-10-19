using Microsoft.AspNetCore.Mvc;
using NPOI.XSSF.UserModel;
using System.Data;
using RoadOfGroping.Common.Consts;
using RoadOfGroping.Core.Permissions.DomainService;
using RoadOfGroping.Core.Permissions.Entity;
using RoadOfGroping.Core.ZRoadOfGropingUtility.MiniExcels;
using RoadOfGroping.Core.ZRoadOfGropingUtility.MiniExcels.Npoi;
using RoadOfGroping.Repository.DomainService;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace RoadOfGroping.Application.ToolService
{
    public class ToolAppService : ApplicationService
    {
        private readonly IPermissionRoleRelationManager permissionRoleRelationManager;
        private readonly IPermissionsManager permissionManager;

        private readonly IMiniExcelManager miniExcelManager;

        private readonly IWebHostEnvironment _hostingEnv;

        public class Student
        {
            public string? Name { get; set; }
            public string? Home { get; set; }
            public int? Age { get; set; }
        }

        public ToolAppService(IServiceProvider serviceProvider, IPermissionRoleRelationManager permissionRoleRelationManager, IPermissionsManager permissionManager, IMiniExcelManager miniExcelManager, IWebHostEnvironment hostingEnv) : base(serviceProvider)
        {
            this.permissionRoleRelationManager = permissionRoleRelationManager;
            this.permissionManager = permissionManager;
            this.miniExcelManager = miniExcelManager;
            _hostingEnv = hostingEnv;
        }

        public async Task<string> ExcelSaveTest()
        {

            List<Student> studentList = new List<Student>()
              {
                  new Student { Name = "小东", Home = "New York", Age = 25 },
                  new Student { Name = "小西", Home = "London", Age = 22 },
                  new Student { Name = "小南", Home = "Paris", Age = 28 },
                  new Student { Name = "小北", Home = "Tokyo", Age = 24 },
                  new Student { Name = "小王", Home = "Berlin", Age = 26 }
             };
            var fileName = $"{Guid.NewGuid()}.xlsx";
            var filepath = await miniExcelManager.SaveToExcelAsync(fileName, studentList);
            return filepath;
        }

        public async Task GetExcelData(string filepath)
        {
            var data =await miniExcelManager.LoadFromExcelAsync<Student>(filepath);
        }


        /// <summary>
        /// Excel导入的具体实现
        /// </summary>
        /// <returns></returns>
        public async Task import_excel()
        {
            string filepath = _hostingEnv.WebRootPath + "/在线用户20230324.xlsx";//导入的文件地址路径，可动态传入
            Dictionary<string, string> dir = new Dictionary<string, string>();//申明excel列名和DataTable列名的对应字典
            dir.Add("Name", "姓名");
            List<Student> keyWordsList = ExcelHelper.ImportExceltoDt<Student>(filepath, dir, "Sheet1", 0);
        }

        /// <summary>
        /// Excel导出的具体实现
        /// </summary>
        /// <returns></returns>
        public async Task<FileResult> export_excel()
        {
            List<Student> keys = new List<Student>();
            for (int i = 0; i < 6; i++)
            {
                keys.Add(new Student { Name = "测试_" + i });
            }

            DataTable dt = ListToDataTable(keys);
            string filename = DateTime.Now.ToString("在线用户yyyyMMdd") + ".xlsx";
            Dictionary<string, string> dir = new Dictionary<string, string>
            {
                { "Name", "姓名" }
            };

            XSSFWorkbook book = ExcelHelper.ExportExcel(dt, dir);
            dt.Dispose();

            using (NpoiMemoryStream ms = new NpoiMemoryStream())
            {
                ms.AllowClose = false;
                book.Write(ms, true);
                ms.Flush();
                ms.Position = 0; // 重置位置以便读取

                // 直接从内存流中读取内容
                var fileContent = ms.ToArray(); // 将内存流转换为字节数组
                ms.Seek(0, SeekOrigin.Begin); // 重置位置以便写入
                ms.AllowClose = true; // 释放内存流
                book.Dispose();
                // 返回文件内容
                return new FileContentResult(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = filename // 使用文件名
                };
            }
        }

        private DataTable ListToDataTable<T>(List<T> items)
        {
            DataTable dt = new DataTable();

            if (items == null || items.Count == 0)
            {
                return dt; // 返回空的 DataTable
            }

            // 使用反射获取类型 T 的所有属性
            var properties = typeof(T).GetProperties();

            // 添加列
            foreach (var property in properties)
            {
                Type propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                dt.Columns.Add(property.Name, propertyType);
            }

            // 添加行
            foreach (var item in items)
            {
                DataRow row = dt.NewRow();
                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item) ?? DBNull.Value; // 处理空值
                }
                dt.Rows.Add(row);
            }

            return dt;
        }

    }
}