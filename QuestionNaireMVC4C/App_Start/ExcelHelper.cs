using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;

namespace QuestionNaireMVC4C
{
    public class ExcelHelper
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
    }
}