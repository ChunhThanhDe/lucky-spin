using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Windows;

namespace LuckySpin
{
    public class ExcelHelper
    {
        /// <summary>
        /// Lấy thông tin file Excel và trả ra List<T> với T là kiểu dữ liệu của các cột trong file Excel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="excelPath"></param>
        /// <param name="worksheetName"></param>
        /// /// <param name="headers"></param>
        /// <returns></returns>
        public static List<T> GetData<T>(
            string excelPath,
            string worksheetName,
            ICollection<string> headers = null
        ) where T : new()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Longnx");
            var list = new List<T>();

            var fileInfo = new FileInfo(excelPath);
            using (var package = new ExcelPackage(fileInfo))
            {
                var workbook = package.Workbook;
                if (workbook == null || workbook.Worksheets.Count < 1)
                {
                    MessageBox.Show("File Excel không chứa worksheet nào.");
                    return list;
                }

                // Chọn worksheet
                ExcelWorksheet worksheet;
                if (!string.IsNullOrWhiteSpace(worksheetName) && workbook.Worksheets[worksheetName] != null)
                    worksheet = workbook.Worksheets[worksheetName];
                else
                    worksheet = workbook.Worksheets.First();

                int startRow = worksheet.Dimension.Start.Row;
                int endRow = worksheet.Dimension.End.Row;
                int startCol = worksheet.Dimension.Start.Column;
                int endCol = worksheet.Dimension.End.Column;
                int colCount = endCol - startCol + 1;

                // Đọc header từ tham số hoặc file
                var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                if (headers != null && headers.Count == colCount)
                {
                    int idx = 0;
                    foreach (var h in headers)
                    {
                        var hdr = h != null ? h.Trim() : null;
                        if (!string.IsNullOrEmpty(hdr) && !headerMap.ContainsKey(hdr))
                            headerMap[hdr] = startCol + idx;
                        idx++;
                    }
                }
                else
                {
                    // Ngược lại, đọc header từ file
                    for (int col = startCol; col <= endCol; col++)
                    {
                        var cellText = worksheet.Cells[startRow, col].Text;
                        var hdr = cellText != null ? cellText.Trim() : null;
                        if (!string.IsNullOrEmpty(hdr) && !headerMap.ContainsKey(hdr))
                            headerMap[hdr] = col;
                    }
                }

                int dataStartRow = startRow + 1; // Bỏ qua header row
                var properties = typeof(T).GetProperties();

                for (int row = dataStartRow; row <= endRow; row++)
                {
                    // Bỏ qua dòng trống hoàn toàn
                    bool rowEmpty = headerMap.Values
                        .All(col => string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Text));
                    if (rowEmpty) continue;

                    var obj = new T();
                    foreach (var prop in properties)
                    {
                        // Xác định tên cột: ưu tiên DisplayAttribute
                        var disp = prop.GetCustomAttribute<DisplayAttribute>();
                        string columnName = (disp != null && disp.Name != null) ? disp.Name : prop.Name;

                        int colIndex;
                        if (!headerMap.TryGetValue(columnName, out colIndex))
                            continue; // không tìm thấy header

                        var cellText = worksheet.Cells[row, colIndex].Text;
                        object value = null;
                        try
                        {
                            var type = prop.PropertyType;
                            if (type == typeof(string))
                            {
                                value = cellText;
                            }
                            else if (type == typeof(int) || type == typeof(int?))
                            {
                                int iv;
                                if (int.TryParse(cellText, out iv))
                                    value = iv;
                                else
                                    value = (type == typeof(int?)) ? (object)null : (object)0;
                            }
                            else if (type == typeof(double) || type == typeof(double?))
                            {
                                double dv;
                                if (double.TryParse(cellText, out dv))
                                    value = dv;
                                else
                                    value = (type == typeof(double?)) ? (object)null : (object)0.0;
                            }
                            else if (type == typeof(DateTime) || type == typeof(DateTime?))
                            {
                                DateTime dt;
                                if (DateTime.TryParse(cellText, out dt))
                                    value = dt;
                                else
                                    value = (type == typeof(DateTime?)) ? (object)null : (object)default(DateTime);
                            }
                            else if (type == typeof(bool) || type == typeof(bool?))
                            {
                                bool bv;
                                if (bool.TryParse(cellText, out bv))
                                    value = bv;
                                else
                                    value = (type == typeof(bool?)) ? (object)null : (object)false;
                            }
                            else
                            {
                                value = cellText;
                            }
                        }
                        catch
                        {
                            value = null;
                        }

                        prop.SetValue(obj, value);
                    }

                    list.Add(obj);
                }
            }

            return list;
        }
    }
}
