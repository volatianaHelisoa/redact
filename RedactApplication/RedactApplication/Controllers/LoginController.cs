
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using RedactApplication.Models;

namespace RedactApplication.Controllers
{
    [HandleError]
    public class LoginController : Controller
    {
        /*pour s'authentifier*/
        public ActionResult AuthentificationUser(UTILISATEURViewModel model)
        {
            try
            {
                string pwdCrypte = model.userMotdepasse.Trim();
                redactapplicationEntities db = new Models.redactapplicationEntities();
                UTILISATEUR utilisateur = null;
                if (model.saveOnComputer)
                {
                    utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userMail == model.userMail.Trim() && x.userMotdepasse == pwdCrypte);
                    if (utilisateur == null)
                        pwdCrypte = Encryptor.EncryptPass(pwdCrypte);
                }
                else
                    pwdCrypte = Encryptor.EncryptPass(pwdCrypte);


                utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userMail == model.userMail.Trim() && x.userMotdepasse == pwdCrypte);

                if (utilisateur != null)
                {
                    FormsAuthentication.SetAuthCookie(utilisateur.userId.ToString(), model.saveOnComputer);/*CREATION COOKIES*/
                    //Session["mail"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(utilisateur.userMail);
                    //Session["pass"] = pwdCrypte;
                    Session["logoUrl"] = utilisateur.logoUrl;
                    Session["name"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(utilisateur.userNom);
                    Session["surname"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(utilisateur.userPrenom);
                    Session["role"] = (new Utilisateurs()).GetUtilisateurRoleToString(utilisateur.userId);
                    if (model.saveOnComputer)
                    {
                        HttpCookie trigerAuths = new HttpCookie("trigerAuths");
                        trigerAuths.Values["username"] = utilisateur.userMail;
                        trigerAuths.Values["password"] = Encryptor.Decrypt(utilisateur.userMotdepasse);
                        trigerAuths.Expires = DateTime.Now.AddDays(Convert.ToInt32(ConfigurationManager.AppSettings["cookiesValidity"]));
                        Response.Cookies.Add(trigerAuths);
                       
                    }
                }
                else
                {
                    return View("ErrorInvalidAccountOrPassword");
                }
                var data = (new Utilisateurs()).GetUtilisateurRole(utilisateur.userId).ToList();
                {
                    if (data.Count == 1)
                    {
                        if (data[0] == 1)
                        {  return RedirectToRoute("Home", new RouteValueDictionary
                            {
                                {"controller", "Contact"},
                                {"action", "ListeContact"}
                            });
                        }
                        if (data[0] == 2)
                        {
                            return RedirectToRoute("Home", new RouteValueDictionary {
                                { "controller", "Commandes" },
                                { "action", "ListCommandes" }
                            });
                        }

                        if (data[0] == 4)
                        {
                            return RedirectToRoute("Home", new RouteValueDictionary
                            {
                                {"controller", "Contact"},
                                {"action", "ListeContact"}
                            });
                        }

                        if (data[0] == 3 || data[0] == 5)
                        {
                            return RedirectToRoute("Home", new RouteValueDictionary {
                                    { "controller", "Home" },
                                    { "action", "ListeUser" },
                                    { "numpage",1 },
                                    { "nbrow", 10 }
                                });
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Debug.WriteLine("passe exception");
                return View("ErrorException");
            }
            Debug.WriteLine("passe error final");
            return View("ErrorInvalidAccountOrPassword");
        }

      

        /*pour aller a la page de traitement des mots de passe oublie*/
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /*pour envoyer un mail de reset password a un utilisateur*/
        public ActionResult SendMail(UTILISATEURViewModel model)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            UTILISATEUR utilisateur = db.UTILISATEURs.FirstOrDefault(x => x.userMail == model.userMail);
            Guid TemporaryIdUser = Guid.NewGuid();
            if (utilisateur == null)
            {
                return View("ErrorUserNotExist");
            }

            if (Request.Url != null)
            {
                var url = Request.Url.Scheme;
                if (Request.Url != null)
                {

                    string callbackurl = Request.Url.Host != "localhost"
                        ? Request.Url.Host
                        : Request.Url.Authority;
                    var port = Request.Url.Port;
                    if (!string.IsNullOrEmpty(port.ToString()) && Request.Url.Host != "localhost")
                        callbackurl += ":" + port;

                    url += "://" + callbackurl;
                }

                ViewBag.mailRecepteur = model.userMail;
                //var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                StringBuilder mailBody = new StringBuilder();
                mailBody.AppendFormat("Dear " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(utilisateur.userNom.ToLower()));
                mailBody.AppendFormat("<br />");
                mailBody.AppendFormat("<p>Your recently requested to reset your password for your " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(utilisateur.userNom.ToLower()) + " account. Click the link bellow to reset it.</p>");
                mailBody.AppendFormat("<br />");
                mailBody.AppendFormat(url + "/Login/UpdatePassword?token=" + TemporaryIdUser);
                mailBody.AppendFormat("<br />");
                mailBody.AppendFormat("<p>If you did not request a password reset, please ignore this email. </p>");
                mailBody.AppendFormat("<br />");
                mailBody.AppendFormat("Thanks.");
                mailBody.AppendFormat("<br />");
                mailBody.AppendFormat("Mediaclick Company.");

                bool isSendMail = MailClient.SendResetPasswordMail(model.userMail, mailBody.ToString(), "Redact application - forgotten password.");
                if (isSendMail)
                {
                    utilisateur.token = TemporaryIdUser;
                    utilisateur.dateToken = DateTime.Now;
                    int result = db.SaveChanges();
                    if (result <= 0)
                    {
                        return View("ErrorConfiguration");
                    }
                    return View("SendMailSuccess", model);
                }
            }

            return View("ErrorConfiguration");
        }


        /*pour rediriger a la page concernant la reussite d'envoi du mail*/
        public ActionResult SendMailSuccess()
        {
            return View();
        }

        /*pour terminer l'envoi du mail et retourner a la page d'authentification*/
        public ActionResult DoSendMail(UTILISATEURViewModel model)
        {
            return RedirectToAction("Accueil", "Login");
        }

        /*pour aller a la page de modification du mot de passe*/
        public ActionResult UpdatePassword(Guid? token)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.token == token);

            /*l'utilisateur est null si le token n'existe pas/plus dans la base de donnees*/
            if (utilisateur == null)
            {
                return RedirectToAction("ExpiredLink", "Login");
            }
            else
            {
                DateTime now = DateTime.Now;
                if (utilisateur.dateToken != null)
                {
                    DateTime dateToken = (DateTime)utilisateur.dateToken;
                    double nbrTime = (now - dateToken).TotalMinutes;
                    if (nbrTime > 60.0)
                    {
                        return RedirectToAction("ExpiredLink", "Login");
                    }
                }
            }
            Session["tokenPass"] = token;
            return View();
        }

        /*pour aller a la page qui indique qu'un lien de recuperation de mot de passe est expire*/
        public ActionResult ExpiredLink()
        {
            return View();
        }

        /*pour modifier le mot de passe*/
        public ActionResult ConfirmUpdatePassword(Guid? token, UTILISATEURViewModel model)
        {
            if (Session["tokenPass"] != null)
            {
                token = (Guid)Session["tokenPass"];
                Session["tokenPass"] = null;
            }
            string patternNoAplha = "[\\W]";
            string patternDigit = "[0-9]";
            string patternAlphaUpper = "[A-Z]";
            string patternAlphaLower = "[a-z]";
            List<string> Error = new List<string>();
            ViewBag.ErrorPassWord = "";

            if (model.userMotdepasse == "")
            {
                Error.Add("The password entered is empty.");
            }
            if (model.userMotdepasseConfirme == "")
            {
                Error.Add("The confirmation password is empty.");
            }
            if (model.userMotdepasse != model.userMotdepasseConfirme)
            {
                Error.Add("The password entered and the confirmation password are not the same.");
            }
            if ((model.userMotdepasse.ToString().Length >= 8) == false)
            {
                Error.Add("The password must contain at least 8 characters.");
            }
            if ((Regex.IsMatch(model.userMotdepasse.ToString(), patternNoAplha)) == false)
            {
                Error.Add("The password must contain at least 1 non-alphanumeric character.");
            }
            if ((Regex.IsMatch(model.userMotdepasse.ToString(), patternDigit)) == false)
            {
                Error.Add("The password must contain at least 1 digit character.");
            }
            if ((Regex.IsMatch(model.userMotdepasse.ToString(), patternAlphaUpper)) == false)
            {
                Error.Add("The password must contain at least 1 uppercase character.");
            }
            if ((Regex.IsMatch(model.userMotdepasse.ToString(), patternAlphaLower)) == false)
            {
                Error.Add("The password must contain at least 1 lowercase character.");
            }
            if (Error.Count != 0)
            {
                Session["tokenPass"] = token;
                ViewBag.userId = token;
                ViewBag.ErrorPassWord = Error;
                return View("ErrrorForgotPassword");
            }
            redactapplicationEntities db = new Models.redactapplicationEntities();
            UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.token == token);
            if (utilisateur == null)
            {
                Error = new List<string> {"You are no longer allowed to change your password."};
                Session["tokenPass"] = token;
                ViewBag.userId = token;
                ViewBag.ErrorPassWord = Error;
                return View("ErrrorForgotPassword");
            }
            utilisateur.userMotdepasse = Encryptor.EncryptPass(model.userMotdepasse);
            utilisateur.token = null;
            utilisateur.dateToken = null;
            db.SaveChanges();
            return RedirectToAction("UpdatePasswordSuccess", "Login");
        }

        /*pour aller a la page qui indique qu'un lien de recuperation de mot de passe est expire*/
        public ActionResult UpdatePasswordSuccess()
        {
            return View();
        }

        /*pour aller a la page d'accueil de notre application*/
        public ActionResult Accueil()
        {
            HttpCookie trigerAuths = Request.Cookies["trigerAuths"];
            UTILISATEURViewModel user = new UTILISATEURViewModel();
            if (trigerAuths != null)
            {
                user.userMail = trigerAuths["username"];
                user.userMotdepasse = trigerAuths["password"];
                user.saveOnComputer = true;
                user.logoUrl = (string) Session["logoUrl"];
                return View(user);
            }

            //if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            //{
            //    Guid userId = new Guid(HttpContext.User.Identity.Name);
            //    var userobj = new redactapplicationEntities().UTILISATEURs.Find(userId);
            //    user.userMail = userobj.userMail;
            //    user.userMotdepasse = userobj.userMotdepasse;
            //    user.logoUrl = userobj.logoUrl;
            //    return View(user);
            //}
            //else
            //{
            //    if (Session["mail"] != null)
            //    {
            //        user.userMail = Session["mail"].ToString().ToLower();
            //        user.userMotdepasse = Session["pass"].ToString();
            //        user.logoUrl = Session["logoUrl"] != null ? Session["logoUrl"].ToString():"";
            //        return View(user);
            //    }
            //}

            return View();
        }

        public ActionResult LogoutPage()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Accueil", "Login");
        }

    }
}