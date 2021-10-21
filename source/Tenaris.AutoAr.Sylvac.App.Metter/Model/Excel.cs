namespace Tenaris.AutoAr.Sylvac.App.Metter.Model
{
    using Microsoft.Office.Interop.Excel;
    using System.Collections.Generic;
    using Tenaris.AutoAr.Sylvac.Library.Metter.Model;
    using _Excel = Microsoft.Office.Interop.Excel;


    public class ExcelConn
    {
        string path = "";
        _Application excel = new _Excel.Application();
        Workbook wb;
        Worksheet ws;

        public ExcelConn(string path, int sheet)
        {
            this.path = path;
            wb = excel.Workbooks.Open(path);
            ws = wb.Worksheets[sheet];
        }

        public void CloseConn()
        {
            wb.Close();
        }

        public List<MetterValue> LoadValues()
        {
            List<MetterValue> Values = new List<MetterValue>();
            bool Loaded = false;
            int x = 1;

            while(!Loaded)
            {
                x++;

                if (ws.Cells[x, 1].Value2 != null || ws.Cells[x, 2].Value2 != null)
                {
                    Values.Add(new MetterValue()
                    {
                        Index = ws.Cells[x, 1].Value2,
                        Value = ws.Cells[x, 2].Value2
                    });
                }
                else
                    Loaded = true;                

            }

            return Values;
        }
    }

}
