using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.POIFS;
using System.Threading.Tasks;
using System.Collections;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Data;
using System.Reflection;

namespace InvoiceManger.Common
{
   public static class NpoiHelper
    {    
        /// <summary>
        /// 获取制定excel文件的sheet列表
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static ArrayList GetExcelSheetNameList(String Path)
        {
            IWorkbook wb = null;
            ArrayList sheetName = new ArrayList();
            //XSSFSheet sh;
            //ISheet sh = null;
            string Sheet_name;
            try
            {
                using (FileStream fs = new FileStream(Path, FileMode.Open, FileAccess.Read))
                {
                    if (Path.IndexOf(".xlsx") > 0)
                        wb = new XSSFWorkbook(fs);
                    else if (Path.IndexOf(".xls") > 0)
                        wb = new HSSFWorkbook(fs);

                    int noOdSheet = wb.NumberOfSheets;

                    for (int i = 0; i < noOdSheet; i++)
                    {
                        try { Sheet_name = wb.GetSheetAt(i).SheetName.ToString(); sheetName.Add(Sheet_name); }
                        catch { break; }
                    }
                }
            }
            catch { }
            return sheetName;
        }
        public static DataTable ReadDataFromExcel(String Path, int sheetIndex)
        {
            //XSSFWorkbook wb;
            IWorkbook wb = null;

            //XSSFSheet sh;
            ISheet sh = null;
            String Sheet_name;

            using (FileStream fs = new FileStream(Path, FileMode.Open, FileAccess.Read))
            {
                if (Path.IndexOf(".xlsx") > 0)
                    wb = new XSSFWorkbook(fs);
                else if (Path.IndexOf(".xls") > 0)
                    wb = new HSSFWorkbook(fs);
            }

            DataTable DT = new DataTable();
            DT.Rows.Clear();
            DT.Columns.Clear();

            if (sheetIndex < 0)
                sheetIndex = 0;

            sh = wb.GetSheetAt(sheetIndex);
            Sheet_name = wb.GetSheetAt(sheetIndex).SheetName;//get  sheet name

            int rowCount = sh.LastRowNum;
            int i = 0;
            bool isGotRowHit = false;

            for (int r = 0; r <= rowCount; r++)
            {
                i = r;
                isGotRowHit = true;

                int celCount = 0;
                try { celCount = sh.GetRow(i).LastCellNum; }
                catch { }
                if (celCount != 0)
                {
                    // add neccessary columns
                    if (DT.Columns.Count < celCount)
                    {
                        for (int j = 0; j < sh.GetRow(i).Cells.Count; j++)
                        {
                            DT.Columns.Add("", typeof(string));
                        }
                    }

                    // add row
                    DT.Rows.Add();

                    // write row value        
                    for (int j = 0; j <= celCount; j++)
                    {
                        var cell = sh.GetRow(i).GetCell(j);
                        if (cell != null)
                        {
                            string val = "";
                            try { val = cell.ToString(); DT.Rows[i][j] = cell.ToString(); }
                            catch
                            {
                                try { DT.Rows[i][j] = val; }
                                catch { }
                            }
                        }
                    }
                }
                else
                {
                    DT.Rows.Add();
                }
                i++;
                if (isGotRowHit == false)
                {
                    DT.Columns.Add("", typeof(string)); DT.Rows.Add();
                }
            }
            return DT;
        }
        public static List<T> ReadToList<T>(string Path,int sheetIndex)
        {
            List<T> ts = new List<T>();
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();//获得类中的各个属性，添到表单中
            Assembly ass = Assembly.GetAssembly(type);//获取泛型的程序集
            //XSSFWorkbook wb;
            IWorkbook wb = null;

            //XSSFSheet sh;
            ISheet sh = null;
            String Sheet_name;

            using (FileStream fs = new FileStream(Path, FileMode.Open, FileAccess.Read))
            {
                if (Path.IndexOf(".xlsx") > 0)
                    wb = new XSSFWorkbook(fs);
                else if (Path.IndexOf(".xls") > 0)
                    wb = new HSSFWorkbook(fs);
            }

            if (sheetIndex < 0)
                sheetIndex = 0;

            sh = wb.GetSheetAt(sheetIndex);
            Sheet_name = wb.GetSheetAt(sheetIndex).SheetName;//get  sheet name

            int rowCount = sh.LastRowNum;

            for (int r = 0; r <= rowCount; r++)
            {
                Object obj = ass.CreateInstance(type.FullName);//泛型实例化
                int i = 0;
                foreach (var Property in properties)
                {

                    if (Property.PropertyType.Equals(typeof(string)))
                    {
                        Property.SetValue((T)obj, sh.GetRow(r).GetCell(i).ToString());
                    }
                    i++;
                }
                ts.Add((T)obj);
            }
            return ts;

        }

    }
}
