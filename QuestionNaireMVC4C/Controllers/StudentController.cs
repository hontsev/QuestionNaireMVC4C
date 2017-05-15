using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuestionNaireMVC4C.Controllers
{
    public class AnswerQn
    {
        public string id { get; set; }
        public string title { get; set; }
        public string introduction { get; set; }
        public string is_open { get; set; }
        public string is_answer { get; set; }
        public string publish_code { get; set; }
    }

    public class Answer
    {
        public string answer { get; set; }
        public string qid { get; set; }
    }



    public class StudentController : Controller
    {


        //public ActionResult Index()
        //{
        //    return View();
        //}

        //public ActionResult CheckQn()
        //{
        //    string uid = CheckIP.GetNetIP();
        //    string sql = "SELECT id,title,introduction,is_open,publish_code FROM questionnaire ORDER BY id DESC";
        //    var tb = DB.GetResult(sql);
        //    List<AnswerQn> qn = new List<AnswerQn>();
        //    for (int i = 0; i < tb.Rows.Count;i++ )
        //    {
        //        AnswerQn aqn = new AnswerQn();
        //        aqn.id = tb.Rows[i][0].ToString();
        //        aqn.title = tb.Rows[i][1].ToString();
        //        aqn.introduction = StringTruncat(tb.Rows[i][2].ToString());
        //        aqn.is_open = tb.Rows[i][3].ToString();
        //        aqn.publish_code = tb.Rows[i][4].ToString();
        //        sql = string.Format("SELECT answer.id FROM answer LEFT JOIN question  ON answer.belong_q=question.id WHERE belong_user='{0}' and question.belong_qn='{1}'", uid, aqn.id);
        //        if (DB.Exists(sql))
        //        {
        //            //is answer
        //            aqn.is_answer = "1";
        //        }
        //        else
        //        {
        //            aqn.is_answer = "0";
        //        }
        //        qn.Add(aqn);
        //    }
        //    ViewBag.questions = qn;
        //    return View();
        //}

        public ActionResult ShowQnByCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                string sql = string.Format("SELECT id FROM questionnaire WHERE publish_code='{0}'", code);
                var res = DB.GetResult(sql);
                if (res.Rows.Count > 0)
                {
                    string id = res.Rows[0][0].ToString();
                    try
                    {
                        int qnid = Int32.Parse(id);

                        ////限制每个ip限答一次
                        //string uid = CheckIP.GetIP();
                        //sql = string.Format("SELECT answer.id FROM answer LEFT JOIN question  ON answer.belong_q=question.id WHERE belong_user='{0}' and question.belong_qn='{1}'", uid, qnid);
                        //if (DB.Exists(sql))
                        //{
                        //    //此ip已经回答过此问题了。
                        //    return RedirectToAction("Message", "Home", new { id = "您已回答了本问卷，感谢您的参与！" });
                        //}


                        sql = string.Format("SELECT id,title,introduction FROM questionnaire WHERE id='{0}'", qnid);
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

                        return View("ShowQn");
                    }
                    catch
                    {

                    }
                }
            }
            return RedirectToAction("Message", "Home", new { id = "不存在此问卷。" });
        }

        //public ActionResult ShowQn(string id)
        //{
        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        try
        //        {
        //            int qnid = Int32.Parse(id);

        //            string uid = CheckIP.GetIP();
        //            string sql = string.Format("SELECT answer.id FROM answer LEFT JOIN question  ON answer.belong_q=question.id WHERE belong_user='{0}' and question.belong_qn='{1}'", uid, qnid);
        //            if (DB.Exists(sql))
        //            {
        //                //此ip已经回答过此问题了。
        //                return RedirectToAction("Message", "Home", new { id = "您已回答了本问卷，感谢您的参与！" });
        //            }


        //            sql = string.Format("SELECT id,title,introduction FROM questionnaire WHERE id='{0}'", qnid);
        //            var qnr = DB.GetResult(sql).Rows[0];
        //            QuestionNaire qn = new QuestionNaire();
        //            qn.id = qnid.ToString();
        //            qn.title = qnr[1].ToString();
        //            qn.introduction = qnr[2].ToString();
        //            List<Question> qs = new List<Question>();
        //            sql = string.Format("SELECT type,introduction,options,id FROM question WHERE belong_qn='{0}' ORDER BY id", qnid);
        //            var qst = DB.GetResult(sql);
        //            for (int i = 0; i < qst.Rows.Count; i++)
        //            {
        //                Question nq = new Question();
        //                nq.type = qst.Rows[i][0].ToString();
        //                nq.title = qst.Rows[i][1].ToString();
        //                nq.options = qst.Rows[i][2].ToString();
        //                nq.id = Int32.Parse(qst.Rows[i][3].ToString());
        //                qs.Add(nq);
        //            }
        //            qn.questions = qs;
        //            ViewBag.qn = qn;

        //            return View();
        //        }
        //        catch
        //        {

        //        }
        //    }
        //    return RedirectToAction("Message", "Home", new { id = "不存在此问卷。" });
        //}

        [HttpPost]
        public ActionResult SubmitQn(List<Answer> answers)
        {
            string uid = DateTime.Now.ToString("yyyy/MM/dd/hh:mm:ss")+"-"+ CheckIP.GetIP() ;

            string sql = "";

            foreach (var a in answers)
            {
                ////删除此ip之前的回答记录
                //sql = string.Format("DELETE FROM answer WHERE belong_user='{0}' AND belong_q='{1}'", uid, a.qid);
                //DB.ExecuteSql(sql);
                a.answer = a.answer.Replace("'", "\"");
                sql = string.Format("INSERT INTO answer(answer,belong_user,belong_q) VALUES('{0}','{1}','{2}')", a.answer, uid, a.qid);
                DB.ExecuteSql(sql);
            }

            return Content("Success");
        }
    }
}
