using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.HSSF.Util;
using NPOI;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.MiniExcels
{
    /// <summary>
    /// Excel帮助类
    /// 功能：
    ///   1、导出数据到Excel文件中
    ///   2、将Excel文件的数据导入到List<T>对象集合中
    /// </summary>
    public static class ExcelHelper
    {
        /// <summary>
        /// 导出列名
        /// </summary>
        public static SortedList ListColumnsName;

        #region 从DataTable导出到excel文件中，支持xls和xlsx格式

        #region 导出为xls文件内部方法

        /// <summary>
        /// 从DataTable 中导出到excel
        /// </summary>
        /// <param name="strFileName">excel文件名</param>
        /// <param name="dtSource">datatabe源数据</param>
        /// <param name="strHeaderText">表名</param>
        /// <param name="sheetnum">sheet的编号</param>
        /// <returns></returns>
        static MemoryStream ExportDT(string strFileName, DataTable dtSource, string strHeaderText, Dictionary<string, string> dir, int sheetnum)
        {
            //创建工作簿和sheet
            IWorkbook workbook = new HSSFWorkbook();
            using (Stream writefile = new FileStream(strFileName, FileMode.OpenOrCreate, FileAccess.Read))
            {
                if (writefile.Length > 0 && sheetnum > 0)
                {
                    workbook = WorkbookFactory.Create(writefile);
                }
            }
            ISheet sheet = null;
            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(Convert.ToString(item.ColumnName)).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(Convert.ToString(dtSource.Rows[i][j])).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 0)
                {
                    string sheetName = strHeaderText + (sheetnum == 0 ? "" : sheetnum.ToString());
                    if (workbook.GetSheetIndex(sheetName) >= 0)
                    {
                        workbook.RemoveSheetAt(workbook.GetSheetIndex(sheetName));
                    }
                    sheet = workbook.CreateSheet(sheetName);
                    #region 表头及样式
                    {
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                        IRow headerRow = sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);
                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;

                        rowIndex = 1;
                    }
                    #endregion

                    #region 列头及样式

                    if (rowIndex == 1)
                    {
                        IRow headerRow = sheet.CreateRow(1);//第二行设置列名
                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        //写入列标题
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(dir[column.ColumnName]);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256 * 2);
                        }
                        rowIndex = 2;
                    }
                    #endregion
                }
                #endregion

                #region 填充内容

                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    NPOI.SS.UserModel.ICell newCell = dataRow.CreateCell(column.Ordinal);
                    string drValue = row[column].ToString();
                    switch (column.DataType.ToString())
                    {
                        case "System.String": //字符串类型
                            double result;
                            if (isNumeric(drValue, out result))
                            {
                                //数字字符串
                                double.TryParse(drValue, out result);
                                newCell.SetCellValue(result);
                                break;
                            }
                            else
                            {
                                newCell.SetCellValue(drValue);
                                break;
                            }

                        case "System.DateTime": //日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle; //格式化显示
                            break;
                        case "System.Boolean": //布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16": //整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal": //浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull": //空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue(drValue.ToString());
                            break;
                    }
                }
                #endregion
                rowIndex++;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms, true);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }

        #endregion

        #region 导出为xlsx文件内部方法

        /// <summary>
        /// 从DataTable 中导出到excel
        /// </summary>
        /// <param name="dtSource">DataTable数据源</param>
        /// <param name="strHeaderText">表名</param>
        /// <param name="fs">文件流</param>
        /// <param name="readfs">内存流</param>
        /// <param name="sheetnum">sheet索引</param>
        static void ExportDTI(DataTable dtSource, string strHeaderText, FileStream fs, MemoryStream readfs, Dictionary<string, string> dir, int sheetnum)
        {
            IWorkbook workbook = new XSSFWorkbook();
            if (readfs.Length > 0 && sheetnum > 0)
            {
                workbook = WorkbookFactory.Create(readfs);
            }
            ISheet sheet = null;
            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(Convert.ToString(item.ColumnName)).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(Convert.ToString(dtSource.Rows[i][j])).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式

                if (rowIndex == 0)
                {
                    #region 表头及样式
                    {
                        string sheetName = strHeaderText + (sheetnum == 0 ? "" : sheetnum.ToString());
                        if (workbook.GetSheetIndex(sheetName) >= 0)
                        {
                            workbook.RemoveSheetAt(workbook.GetSheetIndex(sheetName));
                        }
                        sheet = workbook.CreateSheet(sheetName);
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                        IRow headerRow = sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;
                    }
                    #endregion

                    #region 列头及样式
                    {
                        IRow headerRow = sheet.CreateRow(1);
                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(dir[column.ColumnName]);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256 * 2);
                        }
                    }

                    #endregion

                    rowIndex = 2;
                }
                #endregion

                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    NPOI.SS.UserModel.ICell newCell = dataRow.CreateCell(column.Ordinal);
                    string drValue = row[column].ToString();
                    switch (column.DataType.ToString())
                    {
                        case "System.String": //字符串类型
                            double result;
                            if (isNumeric(drValue, out result))
                            {
                                double.TryParse(drValue, out result);
                                newCell.SetCellValue(result);
                                break;
                            }
                            else
                            {
                                newCell.SetCellValue(drValue);
                                break;
                            }
                        case "System.DateTime": //日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle; //格式化显示
                            break;
                        case "System.Boolean": //布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16": //整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal": //浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull": //空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue(drValue.ToString());
                            break;
                    }
                }
                #endregion
                rowIndex++;
            }
            workbook.Write(fs, true);
            fs.Close();
        }

        #endregion

        #region 导出excel表格

        /// <summary>
        ///  DataTable导出到Excel文件，xls文件
        /// </summary>
        /// <param name="dtSource">数据源</param>
        /// <param name="strHeaderText">表名</param>
        /// <param name="strFileName">excel文件名</param>
        /// <param name="dir">DataTable和excel列名对应字典</param>
        /// <param name="sheetRow">每个sheet存放的行数</param>
        public static void ExportDTtoExcel(DataTable dtSource, string strHeaderText, string strFileName, Dictionary<string, string> dir, bool isNew, int sheetRow = 50000)
        {
            int currentSheetCount = GetSheetNumber(strFileName);//现有的页数sheetnum
            if (sheetRow <= 0)
            {
                sheetRow = dtSource.Rows.Count;
            }
            string[] temp = strFileName.Split('.');
            string fileExtens = temp[temp.Length - 1];
            int sheetCount = (int)Math.Ceiling((double)dtSource.Rows.Count / sheetRow);//sheet数目
            if (temp[temp.Length - 1] == "xls" && dtSource.Columns.Count < 256 && sheetRow < 65536)
            {
                if (isNew)
                {
                    currentSheetCount = 0;
                }
                for (int i = currentSheetCount; i < currentSheetCount + sheetCount; i++)
                {
                    DataTable pageDataTable = dtSource.Clone();
                    int hasRowCount = dtSource.Rows.Count - sheetRow * (i - currentSheetCount) < sheetRow ? dtSource.Rows.Count - sheetRow * (i - currentSheetCount) : sheetRow;
                    for (int j = 0; j < hasRowCount; j++)
                    {
                        pageDataTable.ImportRow(dtSource.Rows[(i - currentSheetCount) * sheetRow + j]);
                    }

                    using (MemoryStream ms = ExportDT(strFileName, pageDataTable, strHeaderText, dir, i))
                    {
                        using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                        {

                            byte[] data = ms.ToArray();
                            fs.Write(data, 0, data.Length);
                            fs.Flush();
                        }
                    }
                }
            }
            else
            {
                if (temp[temp.Length - 1] == "xls")
                    strFileName = strFileName + "x";
                if (isNew)
                {
                    currentSheetCount = 0;
                }
                for (int i = currentSheetCount; i < currentSheetCount + sheetCount; i++)
                {
                    DataTable pageDataTable = dtSource.Clone();
                    int hasRowCount = dtSource.Rows.Count - sheetRow * (i - currentSheetCount) < sheetRow ? dtSource.Rows.Count - sheetRow * (i - currentSheetCount) : sheetRow;
                    for (int j = 0; j < hasRowCount; j++)
                    {
                        pageDataTable.ImportRow(dtSource.Rows[(i - currentSheetCount) * sheetRow + j]);
                    }
                    FileStream readfs = new FileStream(strFileName, FileMode.OpenOrCreate, FileAccess.Read);
                    MemoryStream readfsm = new MemoryStream();
                    readfs.CopyTo(readfsm);
                    readfs.Close();
                    using (FileStream writefs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                    {

                        ExportDTI(pageDataTable, strHeaderText, writefs, readfsm, dir, i);
                    }
                    readfsm.Close();
                }
            }
        }

        /// <summary>
        /// 导出Excel（//超出10000条数据 创建新的工作簿）
        /// </summary>
        /// <param name="dtSource">数据源</param>
        /// <param name="dir">导出Excel表格的字段名和列名的字符串字典实例；例如：dir.Add("IllegalKeywords", "姓名");</param>
        public static XSSFWorkbook ExportExcel(DataTable dtSource, Dictionary<string, string> dir)
        {
            XSSFWorkbook excelWorkbook = new XSSFWorkbook();

            //int columnsCount = columnsNames.GetLength(0);
            int columnsCount = dir.Count;
            if (columnsCount > 0)
            {
                ListColumnsName = new SortedList(new NoSort());
                //for (int i = 0; i < columnsCount; i++)
                //{
                //    ListColumnsName.Add(columnsNames[i, 0], columnsNames[i, 1]);
                //}
                foreach (KeyValuePair<string, string> item in dir)
                {
                    ListColumnsName.Add(item.Key, item.Value);
                }
                if (ListColumnsName == null || ListColumnsName.Count == 0)
                {
                    throw (new Exception("请对ListColumnsName设置要导出的列明！"));
                }
                else
                {
                    excelWorkbook = InsertRow(dtSource);
                }
            }
            else
            {
                throw (new Exception("请对ListColumnsName设置要导出的列明！"));
            }
            return excelWorkbook;
        }

        #endregion

        /// <summary>
        /// 创建Excel文件
        /// </summary>
        /// <param name="filePath"></param>
        private static XSSFWorkbook CreateExcelFile()
        {
            XSSFWorkbook xssfworkbook = new XSSFWorkbook();

            //右击文件“属性”信息
            #region 文件属性信息
            {
                POIXMLProperties props = xssfworkbook.GetProperties();
                props.CoreProperties.Creator = "Joy";//Excel文件的创建作者
                props.CoreProperties.Title = "";//Excel文件标题
                props.CoreProperties.Description = "";//Excel文件备注
                props.CoreProperties.Category = "";//Excel文件类别信息
                props.CoreProperties.Subject = "";//Excel文件主题信息
                props.CoreProperties.Created = DateTime.Now;//Excel文件创建时间
                props.CoreProperties.Modified = DateTime.Now;//Excel文件修改时间
                props.CoreProperties.SetCreated(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                props.CoreProperties.LastModifiedByUser = "Joy";//Excel文件最后一次保存者
                props.CoreProperties.SetModified(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));//Excel文件最后一次保存日期
            }
            #endregion

            return xssfworkbook;
        }

        /// <summary>
        /// 创建excel表头
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="excelSheet"></param>
        private static void CreateHeader(XSSFSheet excelSheet, XSSFWorkbook excelWorkbook, XSSFCellStyle cellStyle)
        {
            int cellIndex = 0;
            XSSFRow newRow = (XSSFRow)excelSheet.CreateRow(0);
            //循环导出列
            foreach (System.Collections.DictionaryEntry de in ListColumnsName)
            {
                XSSFCellStyle? headTopStyle = CreateStyle(excelWorkbook, cellStyle, HorizontalAlignment.Center, VerticalAlignment.Center, 18, true, true, "宋体", true, false, false, true, FillPattern.SolidForeground, HSSFColor.Grey25Percent.Index, HSSFColor.Black.Index, FontUnderlineType.None, FontSuperScript.None, false);
                XSSFCell newCell = (XSSFCell)newRow.CreateCell(cellIndex);
                newCell.SetCellValue(de.Value.ToString());
                newCell.CellStyle = headTopStyle;
                cellIndex++;
            }
        }

        /// <summary>
        /// 插入数据行
        /// </summary>
        private static XSSFWorkbook InsertRow(DataTable dtSource)
        {
            XSSFWorkbook excelWorkbook = CreateExcelFile();
            int rowCount = 0;
            int sheetCount = 1;
            XSSFSheet newsheet = null;

            //循环数据源导出数据集
            newsheet = (XSSFSheet)excelWorkbook.CreateSheet("Sheet" + sheetCount);
            XSSFCellStyle headCellStyle = (XSSFCellStyle)excelWorkbook.CreateCellStyle(); //创建列头单元格实例样式
            CreateHeader(newsheet, excelWorkbook, headCellStyle);
            //单元格内容信息
            foreach (DataRow dr in dtSource.Rows)
            {
                rowCount++;
                //超出10000条数据 创建新的工作簿
                if (rowCount == 10000)
                {
                    rowCount = 1;
                    sheetCount++;
                    newsheet = (XSSFSheet)excelWorkbook.CreateSheet("Sheet" + sheetCount);
                    CreateHeader(newsheet, excelWorkbook, headCellStyle);
                }
                XSSFRow newRow = (XSSFRow)newsheet.CreateRow(rowCount);
                XSSFCellStyle cellStyle = (XSSFCellStyle)excelWorkbook.CreateCellStyle(); //创建单元格实例样式
                XSSFCellStyle? style = CreateStyle(excelWorkbook, cellStyle, HorizontalAlignment.Center, VerticalAlignment.Center, 14, true, false);
                InsertCell(dtSource, dr, newRow, style, excelWorkbook);
            }
            //自动列宽
            //for (int i = 0; i <= dtSource.Columns.Count; i++)
            //{
            //    newsheet.AutoSizeColumn(i, true);
            //}
            return excelWorkbook;
        }

        /// <summary>
        /// 导出数据行
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="drSource"></param>
        /// <param name="currentExcelRow"></param>
        /// <param name="excelSheet"></param>
        /// <param name="excelWorkBook"></param>
        private static void InsertCell(DataTable dtSource, DataRow drSource, XSSFRow currentExcelRow, XSSFCellStyle cellStyle, XSSFWorkbook excelWorkBook)
        {
            for (int cellIndex = 0; cellIndex < ListColumnsName.Count; cellIndex++)
            {
                //列名称
                string columnsName = ListColumnsName.GetKey(cellIndex).ToString();
                XSSFCell newCell = null;
                System.Type rowType = drSource[columnsName].GetType();
                string drValue = drSource[columnsName].ToString().Trim();
                switch (rowType.ToString())
                {
                    case "System.String"://字符串类型
                        drValue = drValue.Replace("&", "&");
                        drValue = drValue.Replace(">", ">");
                        drValue = drValue.Replace("<", "<");
                        newCell = (XSSFCell)currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(drValue);
                        newCell.CellStyle = cellStyle;
                        break;
                    case "System.DateTime"://日期类型
                        DateTime dateV;
                        DateTime.TryParse(drValue, out dateV);
                        newCell = (XSSFCell)currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(dateV);
                        newCell.CellStyle = cellStyle;
                        break;
                    case "System.Boolean"://布尔型
                        bool boolV = false;
                        bool.TryParse(drValue, out boolV);
                        newCell = (XSSFCell)currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(boolV);
                        newCell.CellStyle = cellStyle;
                        break;
                    case "System.Int16"://整型
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Byte":
                        int intV = 0;
                        int.TryParse(drValue, out intV);
                        newCell = (XSSFCell)currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(intV.ToString());
                        newCell.CellStyle = cellStyle;
                        break;
                    case "System.Decimal"://浮点型
                    case "System.Double":
                        double doubV = 0;
                        double.TryParse(drValue, out doubV);
                        newCell = (XSSFCell)currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(doubV);
                        newCell.CellStyle = cellStyle;
                        break;
                    case "System.DBNull"://空值处理
                        newCell = (XSSFCell)currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue("");
                        newCell.CellStyle = cellStyle;
                        break;
                    default:
                        throw (new Exception(rowType.ToString() + "：类型数据无法处理!"));
                }
            }
        }

        /// <summary>
        /// 行内单元格常用样式设置
        /// </summary>
        /// <param name="workbook">Excel文件对象</param>
        /// <param name="cellStyle">Excel文件中XSSFCellStyle对象</param>
        /// <param name="hAlignment">水平布局方式</param>
        /// <param name="vAlignment">垂直布局方式</param>
        /// <param name="fontHeightInPoints">字体大小</param>
        /// <param name="isAddBorder">是否需要边框</param>
        /// <param name="boldWeight">字体加粗 (None = 0,Normal = 400，Bold = 700</param>
        /// <param name="fontName">字体（仿宋，楷体，宋体，微软雅黑...与Excel主题字体相对应）</param>
        /// <param name="isAddBorderColor">是否增加边框颜色</param>
        /// <param name="isItalic">是否将文字变为斜体</param>
        /// <param name="isLineFeed">是否自动换行</param>
        /// <param name="isAddCellBackground">是否增加单元格背景颜色</param>
        /// <param name="fillPattern">填充图案样式(FineDots 细点，SolidForeground立体前景，isAddFillPattern=true时存在)</param>
        /// <param name="cellBackgroundColor">单元格背景颜色（当isAddCellBackground=true时存在）</param>
        /// <param name="fontColor">字体颜色</param>
        /// <param name="underlineStyle">下划线样式（无下划线[None],单下划线[Single],双下划线[Double],会计用单下划线[SingleAccounting],会计用双下划线[DoubleAccounting]）</param>
        /// <param name="typeOffset">字体上标下标(普通默认值[None],上标[Sub],下标[Super]),即字体在单元格内的上下偏移量</param>
        /// <param name="isStrikeout">是否显示删除线</param>
        /// <param name="dataFormat">格式化日期显示</param>
        /// <returns></returns>
        public static XSSFCellStyle CreateStyle(XSSFWorkbook workbook, XSSFCellStyle cellStyle, HorizontalAlignment hAlignment, VerticalAlignment vAlignment, short fontHeightInPoints, bool isAddBorder, bool boldWeight, string fontName = "宋体", bool isAddBorderColor = true, bool isItalic = false, bool isLineFeed = true, bool isAddCellBackground = false, FillPattern fillPattern = FillPattern.NoFill, short cellBackgroundColor = HSSFColor.Yellow.Index, short fontColor = HSSFColor.Black.Index, FontUnderlineType underlineStyle =
            FontUnderlineType.None, FontSuperScript typeOffset = FontSuperScript.None, bool isStrikeout = false, string dataFormat = "yyyy-MM-dd HH:mm:ss")
        {
            cellStyle.Alignment = hAlignment; //水平居中
            cellStyle.VerticalAlignment = vAlignment; //垂直居中
            cellStyle.WrapText = isLineFeed;//自动换行

            //格式化显示
            XSSFDataFormat format = (XSSFDataFormat)workbook.CreateDataFormat();
            cellStyle.DataFormat = format.GetFormat(dataFormat);

            //背景颜色，边框颜色，字体颜色都是使用 HSSFColor属性中的对应调色板索引，关于 HSSFColor 颜色索引对照表，详情参考：https://www.cnblogs.com/Brainpan/p/5804167.html

            //TODO：引用了NPOI后可通过ICellStyle 接口的 FillForegroundColor 属性实现 Excel 单元格的背景色设置，FillPattern 为单元格背景色的填充样式

            //TODO:十分注意，要设置单元格背景色必须是FillForegroundColor和FillPattern两个属性同时设置，否则是不会显示背景颜色
            if (isAddCellBackground)
            {
                cellStyle.FillForegroundColor = cellBackgroundColor;//单元格背景颜色
                cellStyle.FillPattern = fillPattern;//填充图案样式(FineDots 细点，SolidForeground立体前景)
            }
            else
            {
                cellStyle.FillForegroundColor = HSSFColor.White.Index;//单元格背景颜色
            }

            //是否增加边框
            if (isAddBorder)
            {
                //常用的边框样式 None(没有),Thin(细边框，瘦的),Medium(中等),Dashed(虚线),Dotted(星罗棋布的),Thick(厚的),Double(双倍),Hair(头发)[上右下左顺序设置]
                cellStyle.BorderBottom = BorderStyle.Thin;
                cellStyle.BorderRight = BorderStyle.Thin;
                cellStyle.BorderTop = BorderStyle.Thin;
                cellStyle.BorderLeft = BorderStyle.Thin;
            }

            //是否设置边框颜色
            if (isAddBorderColor)
            {
                //边框颜色[上右下左顺序设置]
                cellStyle.TopBorderColor = XSSFFont.DEFAULT_FONT_COLOR;//DarkGreen(黑绿色)
                cellStyle.RightBorderColor = XSSFFont.DEFAULT_FONT_COLOR;
                cellStyle.BottomBorderColor = XSSFFont.DEFAULT_FONT_COLOR;
                cellStyle.LeftBorderColor = XSSFFont.DEFAULT_FONT_COLOR;
            }

            /**
             * 设置相关字体样式
             */
            var cellStyleFont = (XSSFFont)workbook.CreateFont(); //创建字体

            //假如字体大小只需要是粗体的话直接使用下面该属性即可
            //cellStyleFont.IsBold = true;

            cellStyleFont.IsBold = boldWeight; //字体加粗
            cellStyleFont.FontHeightInPoints = fontHeightInPoints; //字体大小
            cellStyleFont.FontName = fontName;//字体（仿宋，楷体，宋体 ）
            cellStyleFont.Color = fontColor;//设置字体颜色
            cellStyleFont.IsItalic = isItalic;//是否将文字变为斜体
            cellStyleFont.Underline = underlineStyle;//字体下划线
            cellStyleFont.TypeOffset = typeOffset;//字体上标下标
            cellStyleFont.IsStrikeout = isStrikeout;//是否有删除线

            cellStyle.SetFont(cellStyleFont); //将字体绑定到样式
            return cellStyle;
        }

        #endregion

        #region 从excel文件中将数据导出到List<T>对象集合

        /// <summary>
        /// 将制定sheet中的数据导出到DataTable中
        /// </summary>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <param name="dir">excel列名和DataTable列名的对应字典</param>
        /// <returns></returns>
        static DataTable ImportDt(ISheet sheet, int HeaderRowIndex, Dictionary<string, string> dir)
        {
            DataTable table = new DataTable();
            IRow headerRow;
            int cellCount;
            try
            {
                //没有标头或者不需要表头用excel列的序号（1,2,3..）作为DataTable的列名
                if (HeaderRowIndex < 0)
                {
                    headerRow = sheet.GetRow(0);
                    cellCount = headerRow.LastCellNum;

                    for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                    {
                        DataColumn column = new DataColumn(Convert.ToString(i));
                        table.Columns.Add(column);
                    }
                }
                //有表头，使用表头做为DataTable的列名
                else
                {
                    headerRow = sheet.GetRow(HeaderRowIndex);
                    cellCount = headerRow.LastCellNum;
                    for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                    {
                        //如果excel某一列列名不存在：以该列的序号作为DataTable的列名，如果DataTable中包含了这个序列为名的列，那么列名为重复列名+序号
                        if (headerRow.GetCell(i) == null)
                        {
                            if (table.Columns.IndexOf(Convert.ToString(i)) > 0)
                            {
                                DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                                table.Columns.Add(column);
                            }
                            else
                            {
                                DataColumn column = new DataColumn(Convert.ToString(i));
                                table.Columns.Add(column);
                            }

                        }
                        //excel中的某一列列名不为空，但是重复了：对应的DataTable列名为“重复列名+序号”
                        else if (table.Columns.IndexOf(headerRow.GetCell(i).ToString()) > 0)
                        {
                            DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                            table.Columns.Add(column);
                        }
                        else
                        //正常情况，列名存在且不重复：用excel中的列名作为DataTable中对应的列名
                        {
                            string aaa = headerRow.GetCell(i).ToString();
                            string colName = dir.Where(s => s.Value == headerRow.GetCell(i).ToString()).First().Key;
                            DataColumn column = new DataColumn(colName);
                            table.Columns.Add(column);
                        }
                    }
                }
                int rowCount = sheet.LastRowNum;
                for (int i = (HeaderRowIndex + 1); i <= sheet.LastRowNum; i++)//excel行遍历
                {
                    try
                    {
                        IRow row;
                        if (sheet.GetRow(i) == null)//如果excel有空行，则添加缺失的行
                        {
                            row = sheet.CreateRow(i);
                        }
                        else
                        {
                            row = sheet.GetRow(i);
                        }

                        DataRow dataRow = table.NewRow();

                        for (int j = row.FirstCellNum; j <= cellCount; j++)//excel列遍历
                        {
                            try
                            {
                                if (row.GetCell(j) != null)
                                {
                                    switch (row.GetCell(j).CellType)
                                    {
                                        case CellType.String://字符串
                                            string str = row.GetCell(j).StringCellValue;
                                            if (str != null && str.Length > 0)
                                            {
                                                dataRow[j] = str.ToString();
                                            }
                                            else
                                            {
                                                dataRow[j] = default(string);
                                            }
                                            break;
                                        case CellType.Numeric://数字
                                            if (DateUtil.IsCellDateFormatted(row.GetCell(j)))//时间戳数字
                                            {
                                                dataRow[j] = DateTime.FromOADate(row.GetCell(j).NumericCellValue);
                                            }
                                            else
                                            {
                                                dataRow[j] = Convert.ToDouble(row.GetCell(j).NumericCellValue);
                                            }
                                            break;
                                        case CellType.Boolean:
                                            dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                            break;
                                        case CellType.Error:
                                            dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                            break;
                                        case CellType.Formula://公式
                                            switch (row.GetCell(j).CachedFormulaResultType)
                                            {
                                                case CellType.String:
                                                    string strFORMULA = row.GetCell(j).StringCellValue;
                                                    if (strFORMULA != null && strFORMULA.Length > 0)
                                                    {
                                                        dataRow[j] = strFORMULA.ToString();
                                                    }
                                                    else
                                                    {
                                                        dataRow[j] = null;
                                                    }
                                                    break;
                                                case CellType.Numeric:
                                                    dataRow[j] = Convert.ToString(row.GetCell(j).NumericCellValue);
                                                    break;
                                                case CellType.Boolean:
                                                    dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                                    break;
                                                case CellType.Error:
                                                    dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                                    break;
                                                default:
                                                    dataRow[j] = "";
                                                    break;
                                            }
                                            break;
                                        default:
                                            dataRow[j] = "";
                                            break;
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                //loger.Error(exception.ToString());
                            }
                        }
                        table.Rows.Add(dataRow);
                    }
                    catch (Exception exception)
                    {
                        //loger.Error(exception.ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                //loger.Error(exception.ToString());
            }
            return table;
        }

        /// <summary>
        /// DataTable 转换为List<T>对象集合
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static List<TResult> DataTableToList<TResult>(this DataTable dt) where TResult : class, new()
        {
            //创建一个属性的列表
            List<PropertyInfo> prlist = new List<PropertyInfo>();
            //获取TResult的类型实例  反射的入口
            Type t = typeof(TResult);
            //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表
            Array.ForEach<PropertyInfo>(t.GetProperties(), p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });
            //创建返回的集合
            List<TResult> oblist = new List<TResult>();
            foreach (DataRow row in dt.Rows)
            {
                //创建TResult的实例
                TResult ob = new TResult();
                //找到对应的数据  并赋值
                prlist.ForEach(p => { if (row[p.Name] != DBNull.Value) p.SetValue(ob, row[p.Name], null); });
                //放入到返回的集合中.
                oblist.Add(ob);
            }
            return oblist;
        }

        /// <summary>
        /// DataTable转化为List集合
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="dt">datatable表</param>
        /// <param name="isStoreDB">是否存入数据库datetime字段，date字段没事，取出不用判断</param>
        /// <returns>返回list集合</returns>
        private static List<T> DataTableToList<T>(DataTable dt, bool isStoreDB = true)
        {
            List<T> list = new List<T>();
            Type type = typeof(T);
            //List<string> listColums = new List<string>();
            PropertyInfo[] pArray = type.GetProperties(); //集合属性数组
            foreach (DataRow row in dt.Rows)
            {
                T entity = Activator.CreateInstance<T>(); //新建对象实例 
                foreach (PropertyInfo p in pArray)
                {
                    if (!dt.Columns.Contains(p.Name) || row[p.Name] == null || row[p.Name] == DBNull.Value)
                    {
                        continue;  //DataTable列中不存在集合属性或者字段内容为空则，跳出循环，进行下个循环   
                    }
                    if (isStoreDB && p.PropertyType == typeof(DateTime) && Convert.ToDateTime(row[p.Name]) < Convert.ToDateTime("1753-01-01"))
                    {
                        continue;
                    }
                    try
                    {
                        var obj = Convert.ChangeType(row[p.Name], p.PropertyType);//类型强转，将table字段类型转为集合字段类型  
                        p.SetValue(entity, obj, null);
                    }
                    catch (Exception)
                    {
                        // throw;
                    }
                }
                list.Add(entity);
            }
            return list;
        }

        /// <summary>
        /// DataSet 转换成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ds"></param>
        /// <param name="tableIndext"></param>
        /// <returns></returns>
        private static List<T> DataTable2List<T>(DataTable dt)
        {
            //确认参数有效 
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }

            IList<T> list = new List<T>(); //实例化一个list 
                                           // 在这里写 获取T类型的所有公有属性。 注意这里仅仅是获取T类型的公有属性，不是公有方法，也不是公有字段，当然也不是私有属性 
            PropertyInfo[] tMembersAll = typeof(T).GetProperties();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //创建泛型对象。为什么这里要创建一个泛型对象呢？是因为目前我不确定泛型的类型。 
                T t = Activator.CreateInstance<T>();

                //获取t对象类型的所有公有属性。但是我不建议吧这条语句写在for循环里，因为没循环一次就要获取一次，占用资源，所以建议写在外面 
                //PropertyInfo[] tMembersAll = t.GetType().GetProperties();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    //遍历tMembersAll 
                    foreach (PropertyInfo tMember in tMembersAll)
                    {
                        //取dt表中j列的名字，并把名字转换成大写的字母。整条代码的意思是：如果列名和属性名称相同时赋值 
                        if (dt.Columns[j].ColumnName.ToUpper().Equals(tMember.Name.ToUpper()))
                        {
                            //dt.Rows[i][j]表示取dt表里的第i行的第j列；DBNull是指数据库中当一个字段没有被设置值的时候的值，相当于数据库中的“空值”。 
                            if (dt.Rows[i][j] != DBNull.Value)
                            {
                                //SetValue是指：将指定属性设置为指定值。 tMember是T泛型对象t的一个公有成员，整条代码的意思就是：将dt.Rows[i][j]赋值给t对象的tMember成员,参数详情请参照http://msdn.microsoft.com/zh-cn/library/3z2t396t(v=vs.100).aspx/html
                                tMember.SetValue(t, Convert.ToString(dt.Rows[i][j]), null);
                            }
                            else
                            {
                                tMember.SetValue(t, null, null);
                            }
                            break;//注意这里的break是写在if语句里面的，意思就是说如果列名和属性名称相同并且已经赋值了，那么我就跳出foreach循环，进行j+1的下次循环 
                        }
                    }
                }
                list.Add(t);
            }
            dt.Dispose();
            return list.ToList();

        }

        /// <summary>
        /// 读取Excel文件特定名字sheet的内容到List<T>对象集合
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="dir">excel列名和DataTable列名的对应字典</param>
        /// <param name="SheetName">excel表名</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static List<T> ImportExceltoDt<T>(string strFileName, Dictionary<string, string> dir, string SheetName, int HeaderRowIndex = 0)
        {
            DataTable table = new DataTable();
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                if (file.Length > 0)
                {
                    IWorkbook wb = WorkbookFactory.Create(file);
                    ISheet isheet = wb.GetSheet(SheetName);
                    table = ImportDt(isheet, HeaderRowIndex, dir);
                    isheet = null;
                }
            }
            List<T> results = DataTableToList<T>(table);
            table.Dispose();
            return results;
        }

        /// <summary>
        /// 读取Excel文件某一索引sheet的内容到DataTable
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet序号</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <param name="dir">excel列名和DataTable列名的对应字典</param>
        /// <returns></returns>
        public static List<T> ImportExceltoDt<T>(string strFileName, Dictionary<string, string> dir, int HeaderRowIndex = 0, int SheetIndex = 0)
        {
            DataTable table = new DataTable();
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                if (file.Length > 0)
                {
                    IWorkbook wb = WorkbookFactory.Create(file);
                    ISheet isheet = wb.GetSheetAt(SheetIndex);
                    table = ImportDt(isheet, HeaderRowIndex, dir);
                    isheet = null;
                }
            }
            List<T> results = DataTableToList<T>(table);
            table.Dispose();
            return results;
        }

        #endregion

        /// <summary>
        /// 获取excel文件的sheet数目
        /// </summary>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        public static int GetSheetNumber(string outputFile)
        {
            int number = 0;
            using (FileStream readfile = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.Read))
            {
                if (readfile.Length > 0)
                {
                    IWorkbook wb = WorkbookFactory.Create(readfile);
                    number = wb.NumberOfSheets;
                }
            }
            return number;
        }

        /// <summary>
        /// 判断内容是否是数字
        /// </summary>
        /// <param name="message"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool isNumeric(String message, out double result)
        {
            Regex rex = new Regex(@"^[-]?\d+[.]?\d*$");
            result = -1;
            if (rex.IsMatch(message))
            {
                result = double.Parse(message);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 验证导入的Excel是否有数据
        /// </summary>
        /// <param name="excelFileStream"></param>
        /// <returns></returns>
        public static bool HasData(Stream excelFileStream)
        {
            using (excelFileStream)
            {
                IWorkbook workBook = new HSSFWorkbook(excelFileStream);
                if (workBook.NumberOfSheets > 0)
                {
                    ISheet sheet = workBook.GetSheetAt(0);
                    return sheet.PhysicalNumberOfRows > 0;
                }
            }
            return false;
        }

    }

    /// <summary>
    /// 排序实现接口 不进行排序 根据添加顺序导出
    /// </summary>
    public class NoSort : IComparer
    {
        public int Compare(object x, object y)
        {
            return -1;
        }
    }

}
