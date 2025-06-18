using System.Reflection.Metadata.Ecma335;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ACCI_Center.Helper
{
    public static class ExcelReaderHelper
    {   
        public static List<T> ReadExcelFileFromFilePath<T>(string filePath, Func<IRow, T> objectMapper)
        {
            try
            {
                var result = new List<T>();

                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook = new XSSFWorkbook(fs);
                    ISheet sheet = workbook.GetSheetAt(0);

                    for (int row = 0; row <= sheet.LastRowNum; row++)
                    {
                        IRow currentRow = sheet.GetRow(row);
                        if (currentRow == null) continue;

                        var item = objectMapper(currentRow);
                        if (item != null)
                        {
                            result.Add((T)(object)item);
                        }

                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading Excel file: {ex.Message}");
                return new List<T>();
            }
        }
        public static List<T> ReadExcelFileFormIFormFile<T> (IFormFile formFile, Func<IRow, T> objectMapper)
        {
            try
            {
                var result = new List<T>();
                using (var stream = formFile.OpenReadStream())
                {
                    IWorkbook workbook = new XSSFWorkbook(stream);
                    ISheet sheet = workbook.GetSheetAt(0);
                    for (int row = 0; row <= sheet.LastRowNum; row++)
                    {
                        IRow currentRow = sheet.GetRow(row);
                        if (currentRow == null) continue;

                        var item = objectMapper(currentRow);
                        if (item != null)
                        {
                            result.Add((T)(object)item);
                        }

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading Excel file: {ex.Message}");
                return new List<T>();
            }
        }
    }
}
