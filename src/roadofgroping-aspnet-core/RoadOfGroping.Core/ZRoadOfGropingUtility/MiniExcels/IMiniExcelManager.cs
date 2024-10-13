using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniExcelLibs;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.MiniExcels
{
    public interface IMiniExcelManager
    {
        Task<string> SaveToExcelAsync<T>(string filePath, List<T> data);

        Task<IEnumerable<T>> LoadFromExcelAsync<T>(string filePath) where T : class, new();

        Task InsertIntoExcelAsync<T>(string filePath, List<T> data, string sheetName = "Sheet1", ExcelType type = ExcelType.UNKNOWN);


    }
}
