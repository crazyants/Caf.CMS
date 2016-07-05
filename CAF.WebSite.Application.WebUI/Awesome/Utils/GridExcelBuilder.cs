using System;
using System.Collections.Generic;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using CAF.Infrastructure.MvcHtml;



namespace AwesomeMvcDemo.Utils
{
    public static class GridExcelBuilder
    {
        public static HSSFWorkbook Build<T>(GridModel<T> gridModel, string[] columns)
        {
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("sheet1");
            var headerRow = sheet.CreateRow(0);

            //create header
            for (int i = 0; i < columns.Length; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(columns[i]);
            }

            var currentRow = 0;

            if (gridModel.Data.Groups != null)
            {
                foreach (var groupView in gridModel.Data.Groups)
                {
                    BuildGroup(sheet, columns, ref currentRow, groupView);
                }
            }
            else if (gridModel.Data.Items != null)
            {
                BuildItems(sheet, columns, ref currentRow, gridModel.Data.Items);
            }

            if (gridModel.Data.Footer != null)
            {
                BuildFooter(sheet, columns, ref currentRow, gridModel.Data.Footer);
            }

            return workbook;
        }

        private static void BuildGroup<T>(ISheet sheet, string[] columns, ref int currentRow, GroupView<T> groupView)
        {
            if (groupView.Header != null)
            {
                currentRow++;
                var row = sheet.CreateRow(currentRow);
                var cell = row.CreateCell(0);
                cell.SetCellValue(groupView.Header.Content);
            }

            if (groupView.Groups != null)
            {
                foreach (var groupViewx in groupView.Groups)
                {
                    BuildGroup(sheet, columns, ref currentRow, groupViewx);
                }
            }
            else if (groupView.Items != null)
            {
                BuildItems(sheet, columns, ref currentRow, groupView.Items);
            }

            if (groupView.Footer != null)
            {
                BuildFooter(sheet, columns, ref currentRow, groupView.Footer);
            }
        }

        private static void BuildItems(ISheet sheet, string[] columns, ref int currentRow, IList<object> items)
        {
            //fill content 
            foreach (var item in items)
            {
                currentRow++;
                var row = sheet.CreateRow(currentRow);

                for (int columnIndex = 0; columnIndex < columns.Length; columnIndex++)
                {
                    var cell = row.CreateCell(columnIndex);
                    CellSetValue(cell, columns[columnIndex], item);
                }
            }
        }

        private static void BuildFooter(ISheet sheet, string[] columns, ref int currentRow, object footer)
        {
            currentRow++;
            var row = sheet.CreateRow(currentRow);
            for (int columnIndex = 0; columnIndex < columns.Length; columnIndex++)
            {
                var cell = row.CreateCell(columnIndex);
                CellSetValue(cell, columns[columnIndex], footer);
            }
        }

        private static void CellSetValue(ICell cell, string column, object item)
        {
            var prop = item.GetType().GetProperty(column);

            if (prop != null)
            {
                var value = prop.GetValue(item, null);
                if (prop.PropertyType == typeof(DateTime))
                {
                    cell.SetCellValue(((DateTime)value).ToShortDateString());
                }
                else if (prop.PropertyType == typeof(int))
                {
                    cell.SetCellValue(Convert.ToDouble(value));
                }
                else
                {
                    cell.SetCellValue(value.ToString());
                }
            }
        }
    }
}