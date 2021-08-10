﻿using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UserManagement.Domain;
using UserManagement.Contract.Utility;

namespace UserManagement.Infrastructure.Files
{
    public class EPPlusExcelUtility<T> : IExcelFileUtility<T> where T : new()
    {
        protected ExcelConfiguration _configuration = new ExcelConfiguration();
        public virtual IEnumerable<T> Read(Stream stream)
        {
            var returnList = Enumerable.Empty<T>();
            var obj = new T();
            var properties = obj.GetType().GetProperties();
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPack = new ExcelPackage())
            {
                excelPack.Load(stream);

                ExcelWorksheet ws = excelPack.Workbook.Worksheets[0];

                List<string> excelColumnHeaders = GetExcelColumnHeadersByPassingWS(ws);
                if(_configuration.ColumnPropertyMapping.Count()> 0)
                {
                    returnList = GetDataForMappedColumns(ws, excelColumnHeaders, properties);
                }
                else
                {
                    returnList = GetData(ws, excelColumnHeaders, properties);
                }
            }
            return returnList;
        }

        public virtual Stream Write(IEnumerable<T> data)
        {
            throw new NotImplementedException();
        }
        private List<string> GetExcelColumnHeadersByPassingWS(ExcelWorksheet ws)
        {
            List<string> excelColumnHeaders = new List<string>();
            int actualColumns = ws?.Dimension?.End?.Column ?? 0;
            int consideredColumns = 0;

            if (actualColumns == 0)
                return null;

            foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
            {
                string firstColumn = firstRowCell.Text;
                excelColumnHeaders.Add(firstColumn);
                consideredColumns++;
            }
            return excelColumnHeaders;
        }
        public IExcelFileUtility<T> Configure(ExcelConfiguration excelConfiguration)
        {
            if (excelConfiguration != null)
            {
                _configuration = excelConfiguration;
            }
            return this;
        }
        private IEnumerable<T> GetData(ExcelWorksheet ws , List<string> excelColumnHeaders, IEnumerable<System.Reflection.PropertyInfo> properties)
        {
            List<T> returnList = new List<T>();
            for (int rowNum = 2; rowNum <= ws.Dimension.End.Row; rowNum++)
            {
                var returnObject = new T();

                foreach (var columnHeader in excelColumnHeaders)
                {
                    var excelHeaderColumnIndex = excelColumnHeaders.IndexOf(columnHeader);
                    var property = properties.FirstOrDefault(x => x.Name == columnHeader);
                    var excelCellText = ws.Cells[rowNum, excelHeaderColumnIndex + 1].Value.ToString();
                    if (property != null)
                    {
                        if (property.PropertyType == typeof(DateTime))
                        {
                            var dateTime = DateTime.ParseExact(excelCellText, _configuration.DateTimeFormat, CultureInfo.InvariantCulture);
                            property.SetValue(returnObject, dateTime);
                        }
                        else
                        {
                            var value = Convert.ChangeType(excelCellText, property.PropertyType);

                            property.SetValue(returnObject, value);
                        }
                    }
                }

                returnList.Add(returnObject);
            }
            return returnList;
        }
        private IEnumerable<T> GetDataForMappedColumns(ExcelWorksheet ws, List<string> excelColumnHeaders, IEnumerable<System.Reflection.PropertyInfo> properties)
        {
            List<T> returnList = new List<T>();
            for (int rowNum = 2; rowNum <= ws.Dimension.End.Row; rowNum++)
            {
                var returnObject = new T();

                foreach (var mappedPair in _configuration.ColumnPropertyMapping)
                {
                    if (excelColumnHeaders.Contains(mappedPair.Value))
                    {
                        var excelHeaderColumnIndex = excelColumnHeaders.IndexOf(mappedPair.Value);
                        var property = properties.FirstOrDefault(x => x.Name == mappedPair.Key);
                        var excelCellText = ws.Cells[rowNum, excelHeaderColumnIndex + 1].Value.ToString();
                        if (property != null)
                        {
                            if (property.PropertyType == typeof(DateTime))
                            {
                                var dateTime = DateTime.ParseExact(excelCellText, _configuration.DateTimeFormat, CultureInfo.InvariantCulture);
                                property.SetValue(returnObject, dateTime);
                            }
                            else
                            {
                                var value = Convert.ChangeType(excelCellText, property.PropertyType);

                                property.SetValue(returnObject, value);
                            }
                        }
                    }
                }

                returnList.Add(returnObject);
            }
            return returnList;
        }
    }

}
