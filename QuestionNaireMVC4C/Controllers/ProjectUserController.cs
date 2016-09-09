using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Collections;
using System.Xml;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

namespace QuestionNaireMVC4C.Controllers
{
    

    public class ProjectUserController : Controller
    {

        /// <summary>
        /// 对datatble的内容进行预处理，以便正常显示。
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataTable FormatDatatable(DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    dt.Rows[i][j] = dt.Rows[i][j].ToString()
                        .Replace("\"", "“")
                        .Replace(",", "，")
                        .Replace("\t", "")
                        .Replace("\n", " ")
                        .Replace("\r", " ");
                }
            }
            return dt;
        }
        
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 统计教师的完成课程数。更新数据库的存储状态
        /// </summary>
        public static void updateTeacherComplete()
        {
            string sql = "UPDATE K SET Completed=(SELECT count(id) from Class_Teacher_Info WHERE Teachername =K.Teachername) FROM Class_Teacher_Info K";
            DB.ExecuteSql(sql);
        }

        public ActionResult CheckClassInfo()
        {
            updateTeacherComplete();
            string sql = "SELECT id,classnumber,classname,classtime,major,projectname,projectkey,teachername,classeva FROM class_teacher_info";
            var info = DB.GetResult(sql).Rows;
            ViewBag.info = info;

            return View();
        }



        public ActionResult CheckClassInfoDetail(string id = "")
        {
            if (!string.IsNullOrEmpty(id))
            {
                // 编辑数据
                try
                {
                    int info_id = Int32.Parse(id);
                    string sql = string.Format("SELECT * FROM class_teacher_info WHERE id='{0}'", info_id);
                    var info = DB.GetResult(sql);
                    info = FormatDatatable(info);
                    ViewBag.info = info.Rows[0];
                }
                catch
                {

                }
            }
            return View();
        }

        public ActionResult DownloadClassInfo()
        {
            string sql = "select classnumber as '课程编号', classname as '课程名称', classperiod as '课程学时', classtime as '开课时间', major as '所属专业', projectname as '项目名称', projectkey as '项目关键词', projecttype as '项目属性', studenttype as '学院类别', number as '学员人数', teachername as '教师姓名',teacherposition as '教师职称', workplace as '教师所在单位', email as 'Email', phone as '办公电话', telephone as '手机电话', other as '其他联系方式', teacherinfo as '教师简介', completed as '完成授课次数', state as '授课状态', teachereva as '教师综合评价', classinfo as '课程简介', classeva as '课程综合评价'  from class_teacher_info";
            var info = DB.GetResult(sql);
            info = FormatDatatable(info);
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + "课程信息导出.csv");
            Response.Charset = "gb2312";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            Response.ContentType = "application/ms-excel";
            Response.Write(ExcelHelper.GetCSVContent(info));
            Response.End();
            return Content("");
        }

    }
}
