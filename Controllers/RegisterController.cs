
using LoginRegistration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace LoginRegistration.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        LoginDbEntities db = new LoginDbEntities();
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult SaveData(SiteUser model)
        {
            model.IsValid = false;
            db.SiteUsers.Add(model);
            db.SaveChanges();
            BuildEmailTemplate(model.Id);
            return Json("Registration Successful", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Confirm(int regId)
        {
            ViewBag.RegId = regId;
            return View();
        }
        public JsonResult RegisterConfirm(int regId)
        {
            SiteUser data = db.SiteUsers.Where(x => x.Id == regId).FirstOrDefault();
            data.IsValid = true;
            db.SaveChanges();
            var msg = "Your email is Verified!";
            return Json(msg, JsonRequestBehavior.AllowGet);

        }

        private void BuildEmailTemplate(int RegId)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Text" + ".cshtml");
            var regInfo = db.SiteUsers.Where(x => x.Id == RegId).FirstOrDefault();
            var url = "https://localhost:44351/" + "Register/Confirm?RegId=" + RegId;
            body = body.Replace("@ViewBag.ConfirmationLink", url);
            body = body.ToString();
            BuildEmailTemplate("Your Account is Successfully Created", body, regInfo.Email);
        }

        public static void BuildEmailTemplate(string subjText, string bodyText, string sendTo)
        {
            string from, to, bcc, cc, subject, body;
            from = "ansususygeorge@gmail.com";
            to = sendTo.Trim();
            bcc = "";
            cc = "";
            subject = subjText;
            StringBuilder sb = new StringBuilder();
            sb.Append(bodyText);
            body = sb.ToString();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(new MailAddress(to));
            if (!string.IsNullOrEmpty(bcc))
            {
                mail.Bcc.Add(new MailAddress(bcc));
            }
            if (!string.IsNullOrEmpty(cc))
            {
                mail.CC.Add(new MailAddress(cc));
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SendEmail(mail);
        }

        public static void SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 465;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("ansususygeorge@gmail.com", "9999999999");
            try
            {
                client.Send(mail);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}