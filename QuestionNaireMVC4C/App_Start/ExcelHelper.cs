using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Collections;
using System.IO;
using System.Data.OleDb;

namespace QuestionNaireMVC4C
{
    public static class ExcelHelper
    {
        public static string SplitChar = ",";
        public static string LineBreakStr = "\r\n";
        public static string AreaStr = "\"";
        public static string PrefixStr = "\t";

        public static string GetCSVContent(DataTable dataTable)
        {
            StringBuilder sbContent = new StringBuilder();

            //列头
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                sbContent.Append(dataTable.Columns[i].ColumnName);
                sbContent.Append(SplitChar);
            }
            sbContent.Append("\r\n");

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                appendCSVRow(dataTable, dataTable.Rows[i], sbContent);
            }

            return sbContent.ToString();
        }

        public static void appendCSVRow(DataTable dataTable, DataRow row, StringBuilder sbContent)
        {
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                DataColumn column = dataTable.Columns[i];
                object value = row[column];
                string strValue = string.IsNullOrEmpty(value.ToString()) ? "" : value.ToString();
                sbContent.Append(AreaStr);
                sbContent.Append(PrefixStr);
                sbContent.Append(strValue.Replace(PrefixStr, PrefixStr + PrefixStr));
                sbContent.Append(AreaStr);
                sbContent.Append(SplitChar);
            }
            sbContent.Append(LineBreakStr);
        }


        /// <summary>
        /// 判断上传文件的后缀是否合法
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool isAllow(string filename)
        {
            string[] allowExtensions = { ".xls", ".xlsx" };
            string fileExtension = System.IO.Path.GetExtension(filename).ToLower();
            for (int i = 0; i < allowExtensions.Length; i++)
            {
                if (fileExtension == allowExtensions[i])
                {
                    return true;
                }
            }
            return false;
        }

        public static DataSet ExcelDataSource(string filepath, string sheetname)
        {
            string strConn;
            if (Path.GetExtension(filepath).ToLower() == ".xlsx")
            {
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=Excel 12.0";
            }
            else
            {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties=Excel 8.0;";
            }
            OleDbConnection conn = new OleDbConnection(strConn);
            OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + sheetname + "]", strConn);
            DataSet ds = new DataSet();
            oada.Fill(ds);
            conn.Close();
            return ds;
        }

        /// <summary>
        /// 获得Excel中的所有sheetname。
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static ArrayList ExcelSheetName(string filepath)
        {
            ArrayList al = new ArrayList();
            string strConn;
            if (Path.GetExtension(filepath).ToLower() == ".xlsx")
            {
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=Excel 12.0";
            }
            else
            {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties=Excel 8.0;";
            }
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            System.Data.DataTable sheetNames = conn.GetOleDbSchemaTable
            (System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            conn.Close();
            foreach (DataRow dr in sheetNames.Rows)
            {
                al.Add(dr[2]);
            }
            return al;
        }
    }
}