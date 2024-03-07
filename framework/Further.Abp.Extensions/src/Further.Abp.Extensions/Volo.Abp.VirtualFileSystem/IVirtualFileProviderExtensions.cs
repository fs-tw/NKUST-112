using Further.Abp.VirtualFileSystem;
using NPOI.SS.UserModel;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Further.Abp.VirtualFileSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MapToExcelAttribute : Attribute
    {
        public string FilePath { get; set; }
        public string SheetName { get; set; }
    }
}
namespace Volo.Abp.VirtualFileSystem
{

    public static class IVirtualFileProviderExtensions
    {
        public static List<T> ReadFromExcel<T>(this IVirtualFileProvider virtualFileProvider, Action<global::Npoi.Mapper.Mapper> action = null, string filePath = null, string sheetName = null)
            where T : class, new()
        {
            var mapToVirtualFileAttribute = TypeDescriptor.GetAttributes(typeof(T)).OfType<MapToExcelAttribute>().FirstOrDefault();
            if (mapToVirtualFileAttribute != null)
            {
                filePath = mapToVirtualFileAttribute.FilePath;
                sheetName = mapToVirtualFileAttribute.SheetName;
            }

            Check.NotNullOrWhiteSpace(filePath, nameof(filePath));

            Check.NotNullOrWhiteSpace(sheetName, nameof(sheetName));


            var file = virtualFileProvider.GetFileInfo(filePath);

            if (!file.Exists)
                return null;

            global::Npoi.Mapper.Mapper mapper = new global::Npoi.Mapper.Mapper(file.CreateReadStream());

            action?.Invoke(mapper);

            filterEmptyRows(mapper, sheetName);

            var datas = mapper.Take<T>(sheetName).ToList();

            var errorDatas = datas.Where(x => !String.IsNullOrEmpty(x.ErrorMessage)).ToList();

            if (errorDatas.Count > 0)
            {
                throw new Exception($"There ara error rows : {string.Join(",", errorDatas.Select(x => $"row at {x.RowNumber} column at {x.ErrorColumnIndex},error message is {x}"))}");
            }

            List<T> sheet = datas.Select(x => x.Value).ToList();

            return sheet;

            static void filterEmptyRows(global::Npoi.Mapper.Mapper mapper, string sheetName)
            {
                var sheet = mapper.Workbook.GetSheet(sheetName);
                var emptyRows = getEmpryRows(sheet);
                foreach (var row in emptyRows) sheet.RemoveRow(row);
            }
            static List<IRow> getEmpryRows(ISheet sheet)
            {
                var emptyRows = new List<IRow>();
                var rowEnumerator = sheet.GetRowEnumerator();

                while (rowEnumerator.MoveNext())
                {
                    var row = rowEnumerator.Current as IRow;

                    if (row.All(cell => cell.CellType == CellType.Blank))
                    {
                        emptyRows.Add(row);
                    }
                }

                return emptyRows;
            }
        }

        public static List<string> GetExcelSheetNames(this IVirtualFileProvider virtualFileProvider, string filePath)
        {
            var file = virtualFileProvider.GetFileInfo(filePath);
            if (!file.Exists)
                return null;
            global::Npoi.Mapper.Mapper mapper = new global::Npoi.Mapper.Mapper(file.CreateReadStream());

            return Enumerable.Range(0, mapper.Workbook.NumberOfSheets).Select(s => mapper.Workbook.GetSheetAt(s).SheetName).ToList();
        }
    }
}
