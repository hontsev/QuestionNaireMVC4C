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
    public class Question
    {
        public int id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string options { get; set; }

        public string star_target { get; set; }

    }

    public class QuestionNaire
    {
        public string id { get; set; }
        public string title { get; set; }
        public string introduction { get; set; }
        public List<Question> questions { get; set; }
    }


    public class QResult
    {
        public string type { get; set; }
        public string title { get; set; }
        public string star_target { get; set; }

        public string[,] options { get; set; }
    }
    public class Result
    {
        //参与人数
        public int number { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public List<QResult> answers { get; set; }
    }

    public class AdminController : Controller
    {
        ///   <summary>
        ///   将指定字符串按指定长度进行剪切，
        ///   </summary>
        ///   <param   name="oldStr "> 需要截断的字符串 </param>
        ///   <param   name="maxLength "> 字符串的最大长度 </param>
        ///   <param   name="endWith "> 超过长度的后缀 </param>
        ///   <returns> 如果超过长度，返回截断后的新字符串加上后缀，否则，返回原字符串 </returns>
        public string StringTruncat(string oldStr, int maxLength = 25, string endWith = "...")
        {
            if (string.IsNullOrEmpty(oldStr))
                //   throw   new   NullReferenceException( "原字符串不能为空 ");
                return oldStr + endWith;
            if (maxLength < 1)
                return "";
            if (oldStr.Length > maxLength)
            {
                string strTmp = oldStr.Substring(0, maxLength);
                if (string.IsNullOrEmpty(endWith))
                    return strTmp;
                else
                    return strTmp + endWith;
            }
            return oldStr;
        }

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

        public ActionResult CheckQn()
        {
            string sql = "SELECT id,title,introduction,is_open FROM questionnaire ORDER BY id DESC";
            var questions = DB.GetResult(sql).Rows;
            //防止简介过长，截断
            for (int i = 0; i < questions.Count; i++)
            {
                questions[i][2] = StringTruncat(questions[i][2].ToString());
            }
            ViewBag.questions = questions;

            return View();
        }

        /// <summary>
        /// 获取问卷回答数目。
        /// </summary>
        /// <returns></returns>
        public int getAnswerNumber(string qnid)
        {
            int num = 0;

            string sql = string.Format("SELECT id FROM question WHERE belong_qn='{0}'", qnid);
            var q = DB.GetResult(sql).Rows;
            if (q.Count <= 0)
            {
                //这个问卷没有题。
                num = 0;
            }
            else
            {
                string qid = q[0][0].ToString();
                sql = string.Format("SELECT count(id) FROM answer WHERE belong_q='{0}'", qid);
                string numstr = DB.GetResult(sql).Rows[0][0].ToString();
                num = Int32.Parse(numstr);
            }

            return num;
        }

        /// <summary>
        /// 统计问卷结果
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CheckQnResult(string id)
        {
            updateStarLevel(id);

            string sql = string.Format("SELECT id,title,introduction,is_open FROM questionnaire WHERE id='{0}'", id);
            var qn = DB.GetResult(sql).Rows[0];
            Result res = new Result();
            res.answers = new List<QResult>();
            res.id = qn[0].ToString();
            res.title = qn[1].ToString();
            res.number = getAnswerNumber(res.id);
            sql = string.Format("SELECT id,introduction,type,options,star_target FROM question WHERE belong_qn='{0}'", res.id);
            var qs = DB.GetResult(sql).Rows;

            for (int i = 0; i < qs.Count; i++)
            {
                QResult qres = new QResult();
                qres.title = qs[i][1].ToString();
                qres.type = qs[i][2].ToString();
                qres.star_target = qs[i][4].ToString();
                string[] qoptions = qs[i][3].ToString().Split(',');
                int onum = qoptions.Length;
                string qid = qs[i][0].ToString();
                switch (qres.type)
                {
                    case "1":
                        //单选
                        qres.options = new string[onum, 2];
                        for (int j = 0; j < onum; j++) qres.options[j, 0] = qoptions[j];
                        int[] times1 = new int[onum];
                        sql = string.Format("SELECT answer FROM answer WHERE belong_q='{0}'", qid);
                        var a1 = DB.GetResult(sql).Rows;
                        for (int k = 0; k < a1.Count; k++)
                        {
                            times1[Int32.Parse(a1[k][0].ToString()) - 1]++;
                        }
                        for (int j = 0; j < onum; j++) qres.options[j, 1] = times1[j].ToString();
                        break;
                    case "2":
                        //多选
                        qres.options = new string[onum, 2];
                        for (int j = 0; j < onum; j++) qres.options[j, 0] = qoptions[j];
                        int[] times2 = new int[onum];
                        sql = string.Format("SELECT answer FROM answer WHERE belong_q='{0}'", qid);
                        var a2 = DB.GetResult(sql).Rows;
                        for (int k = 0; k < a2.Count; k++)
                        {
                            string[] answerstr = a2[k][0].ToString().Split(',');
                            foreach (var ans in answerstr)
                            {
                                int ansint = Int32.Parse(ans);
                                times2[ansint - 1]++;
                            }
                        }
                        for (int j = 0; j < onum; j++) qres.options[j, 1] = times2[j].ToString();
                        break;
                    case "3":
                        //简答题
                        sql = string.Format("SELECT answer FROM answer WHERE belong_q='{0}'", qid);
                        var a4 = DB.GetResult(sql).Rows;
                        qres.options = new string[a4.Count, 1];
                        for (int k = 0; k < a4.Count; k++)
                        {
                            qres.options[k, 0] = a4[k][0].ToString().Replace("\r", "").Replace("\n", "");
                        }

                        break;
                    case "4":
                    case "5":
                        //矩阵单选
                        qres.options = new string[onum, 5];
                        for (int j = 0; j < onum; j++) qres.options[j, 0] = qoptions[j];
                        int[,] times3 = new int[onum, 4];
                        sql = string.Format("SELECT answer FROM answer WHERE belong_q='{0}'", qid);
                        var a = DB.GetResult(sql).Rows;
                        for (int k = 0; k < a.Count; k++)
                        {
                            string[] answerstr = a[k][0].ToString().Split(',');
                            for (int l = 0; l < onum; l++)
                            {
                                int ansint = Int32.Parse(answerstr[l]);
                                times3[l, ansint - 1]++;
                            }
                        }
                        for (int k = 0; k < 4; k++)
                        {
                            for (int j = 0; j < onum; j++) qres.options[j, k + 1] = times3[j, k].ToString();
                        }

                        break;
                    
                    default:
                        break;
                }
                res.answers.Add(qres);
            }
            ViewBag.res = res;
            return View();
        }

        public ActionResult CheckQnPublish(string id)
        {
            string sql = string.Format("SELECT publish_code,title FROM questionnaire WHERE id='{0}'", id);
            var res = DB.GetResult(sql).Rows;
            if (res.Count > 0)
            {
                string code = res[0][0].ToString();
                string name = res[0][1].ToString();
                string url = Url.Action("ShowQnByCode", "Student", new { code = code });
                ViewBag.qnurl = url;
                ViewBag.qnname = name;
            }

            return View();
        }


        public ActionResult CopyQn(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    int qnid = Int32.Parse(id);
                    string sql = string.Format("SELECT * FROM questionnaire WHERE id='{0}'", qnid);
                    var qnr = DB.GetResult(sql).Rows[0];
                    QuestionNaire qn = new QuestionNaire();
                    //qn.id = qnid.ToString();
                    qn.title = qnr[1].ToString() + "（复制）";
                    qn.introduction = qnr[2].ToString();
                    List<Question> qs = new List<Question>();
                    sql = string.Format("SELECT type,introduction,options,star_target FROM question WHERE belong_qn='{0}' ORDER BY id", qnid);
                    var qst = DB.GetResult(sql);
                    for (int i = 0; i < qst.Rows.Count; i++)
                    {
                        Question nq = new Question();
                        nq.type = qst.Rows[i][0].ToString();
                        nq.title = qst.Rows[i][1].ToString();
                        nq.options = qst.Rows[i][2].ToString();
                        nq.star_target = qst.Rows[i][3].ToString();
                        qs.Add(nq);
                    }
                    qn.questions = qs;
                    ViewBag.qn = qn;
                }
                catch
                {

                }
            }

            return View("AddQn");
        }

        public ActionResult DeleteQn(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string sql = string.Format("DELETE FROM questionnaire WHERE id='{0}'", id);
                DB.ExecuteSql(sql);
                sql = string.Format("DELETE FROM answer WHERE belong_q in (SELECT id FROM question WHERE belong_qn='{0}')", id);
                DB.ExecuteSql(sql);
                sql = string.Format("DELETE FROM question WHERE belong_qn='{0}'", id);
                DB.ExecuteSql(sql);
            }
            return RedirectToAction("CheckQn");
        }

        public ActionResult AddQn(string id = "")
        {
            if (!string.IsNullOrEmpty(id))
            {
                // 编辑问卷
                try
                {
                    int qnid = Int32.Parse(id);
                    string sql = string.Format("SELECT * FROM questionnaire WHERE id='{0}'", qnid);
                    var qnr = DB.GetResult(sql).Rows[0];
                    QuestionNaire qn = new QuestionNaire();
                    qn.id = qnid.ToString();
                    qn.title = qnr[1].ToString();
                    qn.introduction = qnr[2].ToString();
                    List<Question> qs = new List<Question>();
                    sql = string.Format("SELECT type,introduction,options,star_target FROM question WHERE belong_qn='{0}' ORDER BY id", qnid);
                    var qst = DB.GetResult(sql);
                    for (int i = 0; i < qst.Rows.Count; i++)
                    {
                        Question nq = new Question();
                        nq.type = qst.Rows[i][0].ToString();
                        nq.title = qst.Rows[i][1].ToString();
                        nq.options = qst.Rows[i][2].ToString();
                        nq.star_target = qst.Rows[i][3].ToString();
                        qs.Add(nq);
                    }
                    qn.questions = qs;
                    ViewBag.qn = qn;
                }
                catch
                {

                }
            }

            return View();
        }


        /// <summary>
        /// 生成发布用的问卷专属编码
        /// (与调用时的服务器时刻有关)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string getPublishCode(string str)
        {
            string code = "";

            string timestr = DateTime.Now.ToLongTimeString();
            string beforestr = str + timestr;

            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            code = BitConverter.ToString(MD5.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(beforestr))).Replace("-", "");
            //md5太长，取前7位
            code = code.Substring(0, 7);

            return code;
        }

        [HttpPost]
        public ActionResult SaveQn(QuestionNaire qn)
        {

            if (string.IsNullOrEmpty(qn.id))
            {
                //new 
                string sql = String.Format("INSERT INTO questionnaire(title,introduction,is_open) VALUES('{0}','{1}',0)", qn.title, qn.introduction);
                DB.ExecuteSql(sql);
                sql = String.Format("SELECT max(id) FROM questionnaire");
                qn.id = DB.GetResult(sql).Rows[0][0].ToString();
                foreach (var q in qn.questions)
                {
                    sql = string.Format("INSERT INTO question(introduction,type,options,star_target,belong_qn) VALUES('{0}','{1}','{2}','{3}','{4}')", q.title, q.type, q.options, q.star_target, qn.id);
                    DB.ExecuteSql(sql);
                }
            }
            else
            {
                //update

                //开启状态下不能修改
                string sql = string.Format("SELECT is_open FROM questionnaire WHERE id='{0}' AND is_open=1", qn.id);
                if (DB.Exists(sql)) return Content("Failed");

                sql = String.Format("UPDATE questionnaire SET title='{0}',introduction='{1}' WHERE id='{2}'", qn.title, qn.introduction, qn.id);
                DB.ExecuteSql(sql);

                //删除以前的题目记录和答题结果
                sql = string.Format("DELETE FROM answer WHERE belong_q in (SELECT id FROM question WHERE belong_qn='{0}')", qn.id);
                DB.ExecuteSql(sql);
                sql = String.Format("DELETE FROM question WHERE belong_qn='{0}'", qn.id);
                DB.ExecuteSql(sql);

                foreach (var q in qn.questions)
                {
                    sql = string.Format("INSERT INTO question(introduction,type,options,star_target,belong_qn) VALUES('{0}','{1}','{2}','{3}','{4}')", q.title, q.type, q.options, q.star_target, qn.id);
                    DB.ExecuteSql(sql);
                }
            }

            return Content("Success");
        }

        /// <summary>
        /// 预览问卷
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ShowQn(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    int qnid = Int32.Parse(id);
                    string sql = string.Format("SELECT id,title,introduction FROM questionnaire WHERE id='{0}'", qnid);
                    var qnr = DB.GetResult(sql).Rows[0];
                    QuestionNaire qn = new QuestionNaire();
                    qn.id = qnid.ToString();
                    qn.title = qnr[1].ToString();
                    qn.introduction = qnr[2].ToString();
                    List<Question> qs = new List<Question>();
                    sql = string.Format("SELECT type,introduction,options,id FROM question WHERE belong_qn='{0}' ORDER BY id", qnid);
                    var qst = DB.GetResult(sql);
                    for (int i = 0; i < qst.Rows.Count; i++)
                    {
                        Question nq = new Question();
                        nq.type = qst.Rows[i][0].ToString();
                        nq.title = qst.Rows[i][1].ToString();
                        nq.options = qst.Rows[i][2].ToString();
                        nq.id = Int32.Parse(qst.Rows[i][3].ToString());
                        qs.Add(nq);
                    }
                    qn.questions = qs;
                    ViewBag.qn = qn;

                    return View();
                }
                catch
                {

                }
            }
            return RedirectToAction("Message", "Home", new { id = "不存在此问卷。" });
        }

        public ActionResult ChangeState(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    int qnid = Int32.Parse(id);
                    string sql = string.Format("SELECT id FROM questionnaire WHERE id='{0}' AND is_open=0", qnid);
                    if (DB.Exists(sql))
                    {
                        //open
                        string publishCode = getPublishCode(id);
                        sql = string.Format("UPDATE questionnaire SET is_open=1 , publish_code='{1}' WHERE id='{0}'", qnid, publishCode);
                        DB.ExecuteSql(sql);
                    }
                    else
                    {
                        //close
                        sql = string.Format("UPDATE questionnaire SET is_open=0 WHERE id='{0}'", qnid);
                        DB.ExecuteSql(sql);
                        updateStarLevel(qnid.ToString());
                    }
                }
                catch
                {

                }
            }
            return RedirectToAction("CheckQn");
        }










        /// <summary>
        /// 根据统计好的四个选项的结果，给出星级。
        /// </summary>
        /// <param name="times"></param>
        private int getStarLevel(double[] times)
        {
            if (times.Length < 4) return 0;
            double sum = times[0] + times[1] + times[2] + times[3];
            if (sum <= 0) return 0;
            double ab = (times[0] + times[1]) / sum;
            double d = times[3] / sum;
            int starLevel = 0;
            if (d >= 0.3)
            {
                starLevel = 1;
            }
            else if (ab >= 0.9)
            {
                if (d > 0.05)
                {
                    starLevel = 4;
                }
                else
                {
                    starLevel = 5;
                }
            }
            else if (ab >= 0.7)
            {
                if (d > 0.1)
                {
                    starLevel = 3;
                }
                else
                {
                    starLevel = 4;
                }
            }
            else if (ab >= 0.5)
            {
                if (d > 0.1)
                {
                    starLevel = 2;
                }
                else
                {
                    starLevel = 3;
                }
            }
            else if (ab >= 0.3)
            {
                if (d > 0.2)
                {
                    starLevel = 1;
                }
                else
                {
                    starLevel = 2;
                }
            }
            else
            {
                starLevel = 1;
            }
            return starLevel;
        }

        /// <summary>
        /// 检查一道题的选项里是不是有指定老师
        /// 这是为了判断这道题哪个选项用来统计指定老师的星级评价
        /// 返回所在位置。没有返回-1
        /// </summary>
        /// <param name="optionsString"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public int checkTeacherIndexInQuestion(string optionsString, string name)
        {
            string[] tmp = optionsString.Split(',');
            for (int i = 0; i < tmp.Length; i++)
            {
                if (tmp[i] == name)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 获取单个教师的星级评价
        /// </summary>
        /// <param name="teacher_name"></param>
        public void setTeacherStarLevel(string teacher_name)
        {
            double[] times = new double[4];

            string sql = string.Format("SELECT id,options FROM question WHERE type='4'");
            var target_qs = DB.GetResult(sql).Rows;
            for (int j = 0; j < target_qs.Count; j++)
            {
                //每道题都要统计。
                int target_index = checkTeacherIndexInQuestion(target_qs[j][1].ToString(), teacher_name);
                if (target_index == -1) continue;
                string target_qid = target_qs[j][0].ToString();
                sql = string.Format("SELECT answer FROM answer WHERE belong_q='{0}'", target_qid);
                var a = DB.GetResult(sql).Rows;
                for (int k = 0; k < a.Count; k++)
                {
                    string[] answerstr = a[k][0].ToString().Split(',');
                    int ansint = Int32.Parse(answerstr[target_index]);
                    times[ansint - 1]++;
                }
            }
            int starLevel = getStarLevel(times);
            if (starLevel > 0)
            {
                sql = string.Format("UPDATE Class_Teacher_Info SET Teachereva='{0}' WHERE Teachername='{1}'", starLevel.ToString() + "星", teacher_name);
                DB.ExecuteSql(sql);
            }
                    
        }

        /// <summary>
        /// 更新课程和教师的星级，当一个问卷被关闭时触发结算。
        /// </summary>
        /// <param name="qnid"></param>
        public void updateStarLevel(string qnid)
        {
            string sql = string.Format("SELECT id,title,introduction,is_open FROM questionnaire WHERE id='{0}'", qnid);
            var qn = DB.GetResult(sql).Rows[0];
            sql = string.Format("SELECT id,introduction,type,options,star_target FROM question WHERE belong_qn='{0}'", qn[0].ToString());
            var qs = DB.GetResult(sql).Rows;

            //选择本问卷里那些与星级相关的题目的答案，计算星级。
            for (int i = 0; i < qs.Count; i++)
            {
                string type = qs[i][2].ToString();
                string[] options=qs[i][3].ToString().Split(',');
                string star_target = qs[i][4].ToString();
                string qid = qs[i][0].ToString();
                if(type=="4")
                {
                    // 教师评级
                    // 得把数据库里跟这个老师有关的所有内容一起拿出来然后评级。
                    
                    // 针对本题涉及的所有老师，每个老师分别统计整个系统内与他相关的问卷的调查结果，然后综合这些问卷结果，计算星级。
                    //找出本题涉及的老师名字们
                    string[] target_names=options;
                    foreach (string tname in target_names)
                    {
                        setTeacherStarLevel(tname);
                    }
                }
                else if (type == "5")
                {
                    //课程评级
                    double[] times = new double[4];
                    for (int s = 0; s < times.Length; s++) times[s] = 0;
                    sql = string.Format("SELECT answer FROM answer WHERE belong_q='{0}'", qid);
                    var a = DB.GetResult(sql).Rows;
                    for (int k = 0; k < a.Count; k++)
                    {
                        string[] answerstr = a[k][0].ToString().Split(',');

                        for (int l = 0; l < answerstr.Length; l++)
                        {
                            int ansint = Int32.Parse(answerstr[l]);
                            times[ansint - 1]++;
                        }
                    }

                    int starLevel = getStarLevel(times);
                    if (starLevel > 0)
                    {
                        sql = string.Format("UPDATE Class_Teacher_Info SET Classeva='{0}' WHERE ClassNumber='{1}'", starLevel.ToString() + "星", star_target.Trim());
                        DB.ExecuteSql(sql);
                    }
                    
                }
            }
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
            string sql = "SELECT id,classnumber,classname,classtime,major,projectname,projectkey,teachername,classeva,state FROM class_teacher_info";
            var info = DB.GetResult(sql).Rows;
            ViewBag.info = info;

            return View();
        }

        public ActionResult DeleteClassInfo(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string sql = "DELETE FROM Class_Teacher_Info WHERE id='" + id + "'";
                DB.ExecuteSql(sql);
            }
            return RedirectToAction("CheckClassInfo");
        }

        

        public ActionResult AddClassInfo(string id="")
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

        public ActionResult CheckClassInfoDetail(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
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
                return View();
            }
            else
            {
                return Redirect("CheckClassInfo");
            }
            
        }

        public ActionResult SaveClassInfo()
        {
            var id = Request["id"].ToString();
            if (string.IsNullOrEmpty(id))
            {
                //新建
                string sql = string.Format(
                    "INSERT INTO Class_Teacher_Info("
                    + "classnumber,classname,classperiod,classtime,major,"
                    + "projectname,projectkey,projecttype,studenttype,number,classinfo,[Classeva_xianxia],"
                    + "[Teachername],[Teacherposition] ,[Workplace],[Email],[Phone],[Telephone],[Other],[Teacherinfo],[State],[Teacherbirthday],[Teacheridentity],[Bank],[Bankcard],[Thirdpay]"
                      + ") VALUES('{0}','{1}','{2}','{3}','{4}',"
                      + "'{5}','{6}','{7}','{8}','{9}','{10}','{11}',"
                      + "'{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}')",
                      Request["c_id"].ToString(),
                      Request["c_name"].ToString(),
                      Request["c_hours"].ToString(),
                      Request["c_date"].ToString(),
                      Request["c_major"].ToString(),

                      Request["p_name"].ToString(),
                      Request["p_key"].ToString(),
                      Request["p_type"].ToString(),
                      Request["s_type"].ToString(),
                      Request["s_num"].ToString(),
                      Request["c_info"].ToString(),
                      Request["c_offlineeva"].ToString(),

                      Request["t_name"].ToString(),
                      Request["t_position"].ToString(),
                      Request["t_place"].ToString(),
                      Request["t_email"].ToString(),
                      Request["t_phone"].ToString(),
                      Request["t_mobilephone"].ToString(),
                      Request["t_other"].ToString(),
                      Request["t_info"].ToString(),
                      Request["t_state"].ToString(),
                      Request["t_birthday"].ToString(),
                      Request["t_idnum"].ToString(),
                      Request["t_bank"].ToString(),
                      Request["t_bankid"].ToString(),
                      Request["t_thirdpay"].ToString()
                      );
                DB.ExecuteSql(sql);
            }
            else
            {
                //编辑已有
                string sql = string.Format(
                    "UPDATE Class_Teacher_Info SET "
                    + "classnumber='{1}',classname='{2}',classperiod='{3}',classtime='{4}',major='{5}',"
                    + "projectname='{6}',projectkey='{7}',projecttype='{8}',studenttype='{9}',number='{10}',classinfo='{11}',[Classeva_xianxia]='{12}',"
                    + "[Teachername]='{13}',[Teacherposition]='{14}' ,[Workplace]='{15}',[Email]='{16}',[Phone]='{17}',[Telephone]='{18}',[Other]='{19}',[Teacherinfo]='{20}',[State]='{21}',[Teacherbirthday]='{22}',[Teacheridentity]='{23}',[Bank]='{24}',[Bankcard]='{25}',[Thirdpay]='{26}'"
                      + " WHERE ID='{0}'",
                      id,
                      Request["c_id"].ToString(),
                      Request["c_name"].ToString(),
                      Request["c_hours"].ToString(),
                      Request["c_date"].ToString(),
                      Request["c_major"].ToString(),

                      Request["p_name"].ToString(),
                      Request["p_key"].ToString(),
                      Request["p_type"].ToString(),
                      Request["s_type"].ToString(),
                      Request["s_num"].ToString(),
                      Request["c_info"].ToString(),
                      Request["c_offlineeva"].ToString(),

                      Request["t_name"].ToString(),
                      Request["t_position"].ToString(),
                      Request["t_place"].ToString(),
                      Request["t_email"].ToString(),
                      Request["t_phone"].ToString(),
                      Request["t_mobilephone"].ToString(),
                      Request["t_other"].ToString(),
                      Request["t_info"].ToString(),
                      Request["t_state"].ToString(),
                      Request["t_birthday"].ToString(),
                      Request["t_idnum"].ToString(),
                      Request["t_bank"].ToString(),
                      Request["t_bankid"].ToString(),
                      Request["t_thirdpay"].ToString()
                      );
                DB.ExecuteSql(sql);
            }
            return RedirectToAction("CheckClassInfo");
        }

        public ActionResult DownloadClassInfo()
        {
            string sql = "select classnumber as '课程编号', classname as '课程名称', classperiod as '课程学时', classtime as '开课时间', major as '所属专业', projectname as '项目名称', projectkey as '项目关键词', projecttype as '项目属性', studenttype as '学院类别', number as '学员人数', teachername as '教师姓名',teacherposition as '教师职称', workplace as '教师所在单位', email as 'Email', phone as '办公电话', telephone as '手机电话', other as '其他联系方式', teacherinfo as '教师简介', completed as '完成授课次数', state as '授课状态', teachereva as '教师综合评价', classinfo as '课程简介', classeva as '课程综合评价', teacherbirthday as '教师出生年月', teacheridentity as '身份证号码', bank as '开户行', bankcard as '银行卡号', thirdpay as '第三方支付' from class_teacher_info";
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

        public DataSet ExcelDataSource(string filepath, string sheetname)
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
        public ArrayList ExcelSheetName(string filepath)
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

        public ActionResult UploadClassInfo()
        {
            HttpPostedFileBase file = Request.Files["infofile"];
            if (file != null && isAllow(file.FileName))
            {
                //存储文件
                string root = HttpContext.Server.MapPath("../Uploads");
                if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                string filepath = Path.Combine(root, Path.GetFileName(file.FileName));
                file.SaveAs(filepath);
                //读取文件
                DataSet ds = ExcelDataSource(filepath, ExcelSheetName(filepath)[0].ToString());
                var info = ds.Tables[0].Rows;
                for (int i = 0; i < info.Count; i++)
                {
                    string c_id = info[i][1].ToString().Replace("\t", "");
                    string sql = "SELECT id FROM Class_Teacher_Info WHERE ClassNumber='" + c_id + "'";
                    if (!DB.Exists(sql) && !string.IsNullOrEmpty(c_id))
                    {
                        //如果课程编号重复，就不导入
                        sql = string.Format(
                        "INSERT INTO Class_Teacher_Info("
                        + "classnumber,classname,classperiod,classtime,major,"
                        + "projectname,projectkey,projecttype,studenttype,number,classinfo,[Classeva_xianxia],"
                        + "[Teachername],[Teacherposition] ,[Workplace],[Email],[Phone],[Telephone],[Other],[Teacherinfo],[Completed],[State],[Teacherbirthday],[Teacheridentity],[Bank],[Bankcard],[Thirdpay]"
                        + ") VALUES('{0}','{1}','{2}','{3}','{4}',"
                        + "'{5}','{6}','{7}','{8}','{9}','{10}','{11}',"
                        + "'{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}')",
                        info[i][1].ToString().Replace("\t", ""),
                        info[i][2].ToString().Replace("\t", ""),
                        info[i][3].ToString().Replace("\t", ""),
                        info[i][4].ToString().Replace("\t", ""),
                        info[i][5].ToString().Replace("\t", ""),

                        info[i][6].ToString().Replace("\t", ""),
                        info[i][7].ToString().Replace("\t", ""),
                        info[i][8].ToString().Replace("\t", ""),
                        info[i][9].ToString().Replace("\t", ""),
                        info[i][10].ToString().Replace("\t", ""),
                        info[i][22].ToString().Replace("\t", ""),
                        info[i][29].ToString().Replace("\t", ""),

                        info[i][11].ToString().Replace("\t", ""),
                        info[i][12].ToString().Replace("\t", ""),
                        info[i][13].ToString().Replace("\t", ""),
                        info[i][14].ToString().Replace("\t", ""),
                        info[i][15].ToString().Replace("\t", ""),
                        info[i][16].ToString().Replace("\t", ""),
                        info[i][17].ToString().Replace("\t", ""),
                        info[i][18].ToString().Replace("\t", ""),
                        info[i][19].ToString().Replace("\t", ""),
                        info[i][20].ToString().Replace("\t", ""),

                        info[i][24].ToString().Replace("\t", ""),
                        info[i][25].ToString().Replace("\t", ""),
                        info[i][26].ToString().Replace("\t", ""),
                        info[i][27].ToString().Replace("\t", ""),
                        info[i][28].ToString().Replace("\t", "")
                    );
                        DB.ExecuteSql(sql);
                    }
                }
                //删除文件
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
                


            }
            return RedirectToAction("CheckClassInfo");
        }

        public ActionResult CheckUser()
        {
            string sql = "SELECT id,name,type FROM [user]";
            var info = DB.GetResult(sql);
            ViewBag.info = info.Rows;
            return View();
        }

        public ActionResult AddUser()
        {
            string u_id = Request["u_id"].ToString();
            string u_pw = "123";
            string u_type = Request["u_type"].ToString();
            if (!string.IsNullOrEmpty(u_id)
                && !string.IsNullOrEmpty(u_type)
                && !DB.Exists(string.Format("SELECT id FROM [user] WHERE name='{0}'", u_id)))
            {
                string sql = string.Format("INSERT INTO [user](name,pw,type) VALUES('{0}','{1}','{2}')", u_id, u_pw, u_type);
                DB.ExecuteSql(sql);
            }
            return RedirectToAction("CheckUser");
        }

        public ActionResult DeleteUser(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string sql = string.Format("DELETE FROM [user] WHERE id='{0}'", id);
                DB.ExecuteSql(sql);
            }
            return RedirectToAction("CheckUser");
        }

        public ActionResult ResetPw(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string sql = string.Format("UPDATE [user] SET pw='123' WHERE id='{0}'", id);
                DB.ExecuteSql(sql);
            }
            return RedirectToAction("CheckUser");
        }
    }
}
