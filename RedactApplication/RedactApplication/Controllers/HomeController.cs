
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using RedactApplication.Models;
using Microsoft.Security.Application;

namespace RedactApplication.Controllers
{
    /// <summary>
    /// Class implémentant le Controlleur de la section User.
    /// </summary>
    [HandleError]
    [MvcApplication.AuthorizeRole]
    public class HomeController : Controller
    {
        /// <summary>
        /// Représente l'identifiant de l'utilisateur courant.
        /// </summary>
        public static Guid _userId;

       
        /// <summary>
        /// Retourne la vue d'Index de la page Utilisateur.
        /// </summary>
        /// <returns>View</returns>
        [MvcApplication.CheckSessionOut]
        public ActionResult Index()
        {
           
            return View();
        }

        [HttpPost]
        public JsonResult ReInitPagination(string hash)
        {
            var val = new { Page = true };
            if (Session["pagination"] == null)
            {
                Session["pagination"] = 1;
            }
            else
            {
                Session["pagination"] = 1;
            }
            return Json(val, JsonRequestBehavior.AllowGet);
        }

        

        /// <summary>
        /// Retourne une liste de réference d'utilisateur.
        /// </summary>
        /// <param name="prefix">chaine à rechercher</param>
        /// <returns>Json</returns>
        [HttpPost]
        public JsonResult IndexUser(string prefix)
        {
            var use = (new SearchData()).UserSearch(prefix);
            try
            {
                var UserIdentity = (from N in use
                                    select new { Value = (N.userNom + " " + N.userPrenom) }).Distinct().ToList();
                return Json(UserIdentity, JsonRequestBehavior.AllowGet);
            }
            catch
            {
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
       

        /// <summary>
        /// Retourne la vue à propos de la page Utilisateur.
        /// </summary>
        /// <returns>View</returns>
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        /// <summary>
        /// Retourne la vue d'une liste d'Utilisateur.
        /// </summary>
        /// <param name="nbrow">nombre de ligne</param>
        /// <param name="numpage">numero de page</param>
        /// <returns>View</returns>       
        [Authorize]
        public ActionResult ListeUser()
        {
          
            // Exécute le traitement de la pagination
            Utilisateurs val = new Utilisateurs();
           
            // Récupère la liste des utilisateurs
            var listeDataUser = val.GetListUtilisateur();

            ViewBag.listeUserVm = listeDataUser.Distinct().ToList();

            // Exécute le suivi de session utilisateur
            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                _userId = Guid.Parse(Request.QueryString["currentid"]);
                Session["currentid"] = Request.QueryString["currentid"];
            }
            else
                _userId = Guid.Parse(HttpContext.User.Identity.Name);

            if (_userId != Guid.Empty)
            {
                ViewBag.CurrentUser = val.GetUtilisateur(_userId);
                if (val.GetUtilisateur(_userId) != null)
                {
                    UTILISATEURViewModel userVm = new UTILISATEURViewModel();
                    var currentuser = val.GetUtilisateur(_userId);
                    userVm.userNom = currentuser.userNom;
                    userVm.userPrenom = currentuser.userPrenom;
                    userVm.userId = currentuser.userId;

                    ViewBag.userRole = val.GetUtilisateurRoleToString(userVm.userId);
                    userVm.redactSkype = currentuser.redactSkype;
                    userVm.redactModePaiement= currentuser.redactModePaiement;
                    userVm.redactNiveau = currentuser.redactNiveau;
                    userVm.redactPhone = currentuser.redactPhone;
                    userVm.redactReferenceur = currentuser.redactReferenceur;
                    userVm.redactThemes = currentuser.redactThemes;
                    userVm.redactVolume = currentuser.redactVolume;
                    
                    return View(userVm);
                }

                
            }
            return View();
        }

        /// <summary>
        /// Redirige vers la vue d'affichage d'une liste d'Utilisateur.
        /// </summary>
        /// <returns>View</returns>        
        [Authorize]
        public ActionResult gotoListUser()
        {
            return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "ListeUser" }
                    });
        }

        /// <summary>
        /// Gère la vue de création d'Utilisateur.
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult GererUtilisateur()
        {
           
            redactapplicationEntities db = new Models.redactapplicationEntities();
            

            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                _userId = Guid.Parse(Request.QueryString["currentid"]);
                Session["currentid"] = Request.QueryString["currentid"];
            }
            else
                _userId = Guid.Parse(HttpContext.User.Identity.Name);

            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(_userId);

            if (!string.IsNullOrEmpty(_userId.ToString()))
            {

                UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == _userId);
                UTILISATEURViewModel userVm = new UTILISATEURViewModel();

                if (utilisateur != null)
                {
                    userVm.userNom = utilisateur.userNom;
                    userVm.userPrenom = utilisateur.userPrenom;
                    userVm.userId = utilisateur.userId;

                    userVm.redactSkype = utilisateur.redactSkype;
                    userVm.redactModePaiement = utilisateur.redactModePaiement;
                    userVm.redactNiveau = utilisateur.redactNiveau;
                    userVm.redactPhone = utilisateur.redactPhone;
                    userVm.redactReferenceur = utilisateur.redactReferenceur;
                    userVm.redactThemes = utilisateur.redactThemes;
                    userVm.redactVolume = utilisateur.redactVolume;
                }

                return View("EditUserConfirmation", userVm);
            }
            return View();
        }

        /// <summary>
        /// Retourne la vue de création d'Utilisateur.
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult GotoCreateUser()
        {

            _userId = Guid.Parse(HttpContext.User.Identity.Name);

            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(_userId);

            Guid editUserId;

            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                editUserId = Guid.Parse(Request.QueryString["currentid"]);
            }
            else
            {
                editUserId = Guid.NewGuid();
            }

            if (_userId != Guid.Empty)
            {
                redactapplicationEntities db = new Models.redactapplicationEntities();
                UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == _userId);
                ViewBag.CurrentUser = utilisateur;

                if (utilisateur != null)
                {
                    UTILISATEURViewModel userVm = new UTILISATEURViewModel();
                    userVm.userNom = utilisateur.userNom;
                    userVm.userPrenom = utilisateur.userPrenom;
                    userVm.userId = editUserId;
                    userVm.redactSkype = utilisateur.redactSkype;
                    userVm.redactModePaiement = utilisateur.redactModePaiement;
                    userVm.redactNiveau = utilisateur.redactNiveau;
                    userVm.redactPhone = utilisateur.redactPhone;
                    userVm.redactReferenceur = utilisateur.redactReferenceur;
                    userVm.redactThemes = utilisateur.redactThemes;
                    userVm.redactVolume = utilisateur.redactVolume;

                    return View("GererUtilisateur", userVm);
                }
            }
            return RedirectToAction("GererUtilisateur", "Home");
        }

        /// <summary>
        /// Retourne la vue de confirmation de création d'Utilisateur.
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult CreatedUserConfirmation()
        {
            if (StatePageSingleton.nullInstance())
            {
                StatePageSingleton.getInstance(1, 10);
                ViewBag.numpage = 1;
                ViewBag.nbrow = 10;
            }
            else
            {
                ViewBag.numpage = StatePageSingleton.getInstance().Numpage;
                ViewBag.nbrow = StatePageSingleton.getInstance().Nbrow;
            }

            UTILISATEURViewModel currentUser = GetCurrentUserModel();
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(user);
            return View(currentUser);
        }

        /// <summary>
        /// Retourne la vue d'édition d'Utilisateur.
        /// </summary>
        /// <param name="hash">id de l'Utilisateur</param>
        /// <param name="error">message d'erreur</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult EditUser(Guid? hash, string error)
        {
            switch (error)
            {
                case "ErrorMessage":
                    ViewBag.ErrorMessage = "role null";
                    break;
                case "ErrorMail":
                    ViewBag.ErrorMail = "mail invalid";
                    break;
                case "ErrorRole":
                    ViewBag.ErrorRole = "role null";
                    break;
                case "ErrorUserMailValidation":
                    ViewBag.ErrorUserValidation = "mail is not valid";
                    break;
            }

            Guid userId = (Guid)hash;
            ViewBag.currentid = hash;

            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRoleEdit = (new Utilisateurs()).GetUtilisateurRoleToString(user);

            if (userId != Guid.Empty)
            {
                redactapplicationEntities db = new Models.redactapplicationEntities();
                UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == userId);
                UTILISATEURViewModel userVm = new UTILISATEURViewModel();
                if (utilisateur != null)
                {
                    userVm.userId = utilisateur.userId;
                    userVm.userMail = utilisateur.userMail;
                    userVm.userNom = utilisateur.userNom;
                    userVm.userPrenom = utilisateur.userPrenom;
                    userVm.redactSkype = utilisateur.redactSkype;
                    userVm.redactModePaiement = utilisateur.redactModePaiement;
                    userVm.redactNiveau = utilisateur.redactNiveau;
                    userVm.redactPhone = utilisateur.redactPhone;
                    userVm.redactReferenceur = utilisateur.redactReferenceur;
                    userVm.redactThemes = utilisateur.redactThemes;
                    userVm.redactVolume = utilisateur.redactVolume;
                    userVm.redactTarif = utilisateur.redactTarif;

                    if (Session["userEditModif"] != null)
                    {
                        UTILISATEURViewModel model = (UTILISATEURViewModel) Session["userEditModif"];
                        userVm.userMail = model.userMail;
                        userVm.userNom = model.userNom;
                        userVm.userPrenom = model.userPrenom;
                        userVm.redactSkype = model.redactSkype;
                        userVm.redactModePaiement = model.redactModePaiement;
                        userVm.redactNiveau = model.redactNiveau;
                        userVm.redactPhone = model.redactPhone;
                        userVm.redactReferenceur = model.redactReferenceur;
                        userVm.redactThemes = model.redactThemes;
                        userVm.redactVolume = model.redactVolume;
                        userVm.redactTarif = model.redactTarif;
                        Session["userEditModif"] = null;
                    }

                    userVm.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(utilisateur.userId);
                    List<UserRole> listeUserRole = db.UserRoles.Where(x => x.idUser == utilisateur.userId).ToList();
                    if (listeUserRole.Any())
                    {
                        switch (listeUserRole[0].idRole)
                        {
                            case 1:
                                ViewBag.userRole = 1; /*referenceur manager*/
                                break;
                            case 2:
                                ViewBag.userRole = 2; /*redacteur manager*/
                                break;
                            case 3:
                                ViewBag.userRole = 3; /*manager utilisateur*/
                                break;
                            case 4:
                                ViewBag.userRole = 4; /*administrateur*/
                                break;
                            case 5:
                                ViewBag.userRole = 5; /*super administrateur*/
                                break;
                            default:
                                ViewBag.userRole = 0;
                                break;
                        }

                        ViewBag.listeUserRole = listeUserRole;
                    }
                }

                return View(userVm);
            }
            return View();
        }


        /// <summary>
        /// Retourne la vue d'édition d'Utilisateur.
        /// </summary>
        /// <param name="hash">id de l'Utilisateur</param>
        /// <param name="error">message d'erreur</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult DetailsRedacteurs(Guid? hash, string error)
        {
            switch (error)
            {
                case "ErrorMessage":
                    ViewBag.ErrorMessage = "role null";
                    break;
                case "ErrorMail":
                    ViewBag.ErrorMail = "mail invalid";
                    break;
                case "ErrorRole":
                    ViewBag.ErrorRole = "role null";
                    break;
                case "ErrorUserMailValidation":
                    ViewBag.ErrorUserValidation = "mail is not valid";
                    break;
            }

            Guid userId = (Guid)hash;
            ViewBag.currentid = hash;

            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRoleEdit = (new Utilisateurs()).GetUtilisateurRoleToString(user);

            if (userId != Guid.Empty)
            {
                redactapplicationEntities db = new Models.redactapplicationEntities();
                UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == userId);
                UTILISATEURViewModel userVm = new UTILISATEURViewModel();
                if (utilisateur != null)
                {
                    userVm.userId = utilisateur.userId;
                    userVm.userMail = utilisateur.userMail;
                    userVm.userNom = utilisateur.userNom;
                    userVm.userPrenom = utilisateur.userPrenom;
                    userVm.redactSkype = utilisateur.redactSkype;
                    userVm.redactModePaiement = utilisateur.redactModePaiement;
                    userVm.redactNiveau = utilisateur.redactNiveau;
                    userVm.redactPhone = utilisateur.redactPhone;
                    userVm.redactReferenceur = utilisateur.redactReferenceur;
                    userVm.redactThemes = utilisateur.redactThemes;
                    userVm.redactVolume = utilisateur.redactVolume;
                    userVm.redactTarif = utilisateur.redactTarif;

                    if (Session["userEditModif"] != null)
                    {
                        UTILISATEURViewModel model = (UTILISATEURViewModel)Session["userEditModif"];
                        userVm.userMail = model.userMail;
                        userVm.userNom = model.userNom;
                        userVm.userPrenom = model.userPrenom;
                        userVm.redactSkype = model.redactSkype;
                        userVm.redactModePaiement = model.redactModePaiement;
                        userVm.redactNiveau = model.redactNiveau;
                        userVm.redactPhone = model.redactPhone;
                        userVm.redactReferenceur = model.redactReferenceur;
                        userVm.redactThemes = model.redactThemes;
                        userVm.redactVolume = model.redactVolume;
                        userVm.redactTarif = model.redactTarif;
                        Session["userEditModif"] = null;
                    }

                    userVm.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(utilisateur.userId);
                    List<UserRole> listeUserRole = db.UserRoles.Where(x => x.idUser == utilisateur.userId).ToList();
                    if (listeUserRole.Any())
                    {
                        switch (listeUserRole[0].idRole)
                        {
                            case 1:
                                ViewBag.userRole = 1; /*referenceur manager*/
                                break;
                            case 2:
                                ViewBag.userRole = 2; /*redacteur manager*/
                                break;
                            case 3:
                                ViewBag.userRole = 3; /*manager utilisateur*/
                                break;
                            case 4:
                                ViewBag.userRole = 4; /*administrateur*/
                                break;
                            case 5:
                                ViewBag.userRole = 5; /*super administrateur*/
                                break;
                            default:
                                ViewBag.userRole = 0;
                                break;
                        }

                        ViewBag.listeUserRole = listeUserRole;
                    }
                }

                return View(userVm);
            }
            return View();
        }


        /// <summary>
        /// Retourne la vue de validation de suppression d'Utilisateur.
        /// </summary>
        /// <param name="hash">id de l'Utilisateur</param>
        /// <returns>View</returns>
        public ActionResult DeletedUserWaitting(Guid hash)
        {
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(user);

            ViewBag.hashUser = hash;
            return View("DeletedUserWaitting");
        }

        /// <summary>
        /// Retourne la vue de confirmation de suppression d'un Utilisateur.
        /// </summary>
        /// <param name="hash">id de l'Utilisateur</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult DeletedUserConfirmation(Guid hash)
        {
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(user);

            if (hash != Guid.Empty)
            {
                try
                {
                    redactapplicationEntities db = new Models.redactapplicationEntities();
                    UTILISATEUR users = db.UTILISATEURs.SingleOrDefault(x => x.userId == hash);
                    if (users != null)
                    {
                       
                        List<UserRole> listeUserRole = db.UserRoles.Where(x => x.idUser == users.userId).ToList();
                        for (int indice = 0; indice < listeUserRole.Count(); indice++)
                        {
                            db.UserRoles.Remove(listeUserRole[indice]);
                        }
                        db.UTILISATEURs.Remove(users);
                        db.SaveChanges();
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "ListeUser" }
                    });
        }

        /// <summary>
        /// Retourne la vue de confirmation de suppression d'une liste d'Utilisateur.
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult DeleteAllUserConfirmation()
        {
            Guid userSession = new Guid(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(userSession);

            try
            {
                return View("DeletedAllUserWaitting");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "ListeUser" }
                    });
        }

        [HttpPost]
        public JsonResult InfoSearch()
        {
            var val = new { value = "" };
            if (Session["Infosearch"] != null)
            {
                val = new { value = Session["Infosearch"].ToString() };
                Session["Infosearch"] = null;
            }
            return Json(val, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Retourne la vue contenant la recherche d'Utilisateur.
        /// </summary>
        /// <param name="searchValue">Chaine de recherche</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UserSearch(string searchValue)
        {
            if (searchValue != null && searchValue != "")
            {
                Session["Infosearch"] = searchValue;
            }
            else
            {
                return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "ListeUser" }
                    });
            }

            redactapplicationEntities bds = new Models.redactapplicationEntities();
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(user);

            Utilisateurs db = new Utilisateurs();
            var answer = db.SearchUtilisateur(searchValue);
            if (answer == null || answer.Count == 0)
            {                
                List<UTILISATEURViewModel> listeUser = new List<UTILISATEURViewModel>();
                answer = listeUser;
                ViewBag.SearchUserNoResultat = 1;
            }

            ViewBag.Search = true;
            redactapplicationEntities e = new redactapplicationEntities();
            var req = e.UserRoles.Where(x => x.idRole == 1 || x.idRole == 2 || x.idRole == 3 || x.idRole == 4);
            List<UTILISATEURViewModel> listeDataUserFiltered = new List<UTILISATEURViewModel>();
            foreach (var userModel in answer)
            {
                foreach (var q in req)
                {
                    if (userModel.userId == q.idUser)
                    {
                        listeDataUserFiltered.Add(userModel);
                    }
                }
            }
            ViewBag.listeUserVm = listeDataUserFiltered.Distinct().ToList();
            ViewBag.listeUserRole = e.UserRoles.ToList();
            if (user != Guid.Empty)
            {
                ViewBag.CurrentUser = db.GetUtilisateur(user);
                var utilisateur = db.GetUtilisateur(user);
                if (utilisateur != null)
                {
                    UTILISATEURViewModel userVm = new UTILISATEURViewModel();
                   
                    userVm.userNom = utilisateur.userNom;
                    userVm.userPrenom = utilisateur.userPrenom;
                    userVm.userId = utilisateur.userId;

                    userVm.redactSkype = utilisateur.redactSkype;
                    userVm.redactModePaiement = utilisateur.redactModePaiement;
                    userVm.redactNiveau = utilisateur.redactNiveau;
                    userVm.redactPhone = utilisateur.redactPhone;
                    userVm.redactReferenceur = utilisateur.redactReferenceur;
                    userVm.redactThemes = utilisateur.redactThemes;
                    userVm.redactVolume = utilisateur.redactVolume;

                    ViewBag.userRole = db.GetUtilisateurRoleToString(userVm.userId);
                    return View("ListeUser", userVm);
                }
            }
            return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "ListeUser" }
                    });
        }   

        /// <summary>
        /// Ajoute un nouvel Utilisateur dans la base de données.
        /// </summary>
        /// <param name="model">information Utilisateur</param>
        /// <param name="selectedDiv">liste des divisions de l'utilisateur à créer</param>
        /// <param name="selectedRole">liste des roles de l'utilisateur à créer</param>
        /// <returns>View</returns>
        [Authorize]
        [ValidateInput(false)]
        public ActionResult EnregistrerUtilisateur(UTILISATEURViewModel model, int[] selectedDiv, string[] selectedRole)
        {
            model.userNom = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userNom));
            model.userPrenom = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userPrenom));
            model.userMail = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userMail));

            if (string.IsNullOrEmpty(model.userNom) || string.IsNullOrEmpty(model.userPrenom) || string.IsNullOrEmpty(model.userMail))
            {
                ViewBag.succes = 3;
                return View("GererUtilisateur");
            }            

            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(user);

            redactapplicationEntities db = new Models.redactapplicationEntities();

            bool isRoleValid = true;
            if (selectedRole == null) isRoleValid = false;
            else if (selectedRole.Length == 0) isRoleValid = false;
            if (!isRoleValid)
            {
                ViewBag.ErrorMessage = "role is null";
                return View("GererUtilisateur");
            }
            if (Regex.IsMatch(model.userMail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase) == false)
            {             
                ViewBag.ErrorUserValidation = "This email is not valid.";
                return View("GererUtilisateur");
            }            
            
            try
            {
                UTILISATEUR utilisateur = new UTILISATEUR();
                var users = db.UTILISATEURs.Count(x => x.userMail == model.userMail);
                if (users <= 0)
                {
                    utilisateur.userNom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.userNom.ToLower());
                    utilisateur.userPrenom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.userPrenom.ToLower());
                    utilisateur.userMail = model.userMail;

                    utilisateur.redactSkype = model.redactSkype;
                    utilisateur.redactModePaiement = model.redactModePaiement;
                    utilisateur.redactNiveau = model.redactNiveau;
                    utilisateur.redactPhone = model.redactPhone;
                    utilisateur.redactReferenceur = model.redactReferenceur;
                    utilisateur.redactThemes = model.redactThemes;
                    utilisateur.redactVolume = model.redactVolume;
                    utilisateur.redactTarif = model.redactTarif;

                    utilisateur.userId = Guid.NewGuid();
                    db.UTILISATEURs.Add(utilisateur);

                    
                    if (selectedRole.Count() == 1)
                    {
                        UserRole userRole = new UserRole();
                        userRole.idRole = int.Parse(selectedRole[0]);
                        userRole.idUser = utilisateur.userId;
                        db.UserRoles.Add(userRole);
                    }
                    else
                    {
                        bool test = false;
                        foreach (var val in selectedRole)
                        {
                            if (val.Contains("1") || val.Contains("2"))
                            {
                                test = true;
                            }
                            else
                            {
                                test = false;
                            }
                        }
                        if (test == true)
                        {
                            UserRole userRole = new UserRole();
                            userRole.idRole = 4;
                            userRole.idUser = utilisateur.userId;
                            db.UserRoles.Add(userRole);
                        }
                    }
                    db.SaveChanges();

                    return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "sendMailRecovery" },
                        { "hash", utilisateur.userId }
                    });
                }
                else
                {
                    ViewBag.ErrorUserCreation = true;
                    return View("GererUtilisateur");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                ViewBag.ErrorMessage = "role null";
                return View("GererUtilisateur");
            }
        }

        /// <summary>
        /// Enregistre les modifications d'un Utilisateur dans la base de données.
        /// </summary>
        /// <param name="model">information Utilisateur</param>
        /// <param name="selectedDiv">liste des divisions de l'utilisateur à créer</param>
        /// <param name="selectedRole">liste des roles de l'utilisateur à créer</param>
        /// <param name="idUser">id de l'Utilisateur</param>
        /// <returns>View</returns>
        [Authorize]
        [ValidateInput(false)]
        public ActionResult ModifierUtilisateur(UTILISATEURViewModel model, int[] selectedDiv, string[] selectedRole, Guid idUser)
        {
            model.userNom = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userNom));
            model.userPrenom = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userPrenom));
            model.userMail = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userMail));

            model.redactSkype = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactSkype));
            model.redactModePaiement = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactModePaiement));
            model.redactNiveau = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactNiveau));
            model.redactPhone = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactPhone));
            model.redactReferenceur = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactReferenceur));
            model.redactThemes = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactThemes));
            model.redactVolume = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactVolume));
            model.redactTarif = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactTarif));

            if (string.IsNullOrEmpty(model.userNom) || string.IsNullOrEmpty(model.userPrenom) || string.IsNullOrEmpty(model.userMail))
            {
                return View("ErrorEditUser");
            }
            if (StatePageSingleton.nullInstance())
            {
                StatePageSingleton.getInstance(1, 10);
                ViewBag.numpage = 1;
                ViewBag.nbrow = 10;
            }
            else
            {
                ViewBag.numpage = StatePageSingleton.getInstance().Numpage;
                ViewBag.nbrow = StatePageSingleton.getInstance().Nbrow;
            }
            _userId = idUser;

            Guid userID = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(userID);

            redactapplicationEntities db = new Models.redactapplicationEntities();
            //Recuperation de l'utilisateur
            UTILISATEUR user = db.UTILISATEURs.SingleOrDefault(x => x.userId == idUser);

            if (selectedRole == null)
            {
                Session["userEditModif"] = model;
                return RedirectToAction("EditUser", new { hash = idUser, error = "ErrorRole" });
            }
            if (Regex.IsMatch(model.userMail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase) == false)
            {
                Session["userEditModif"] = model;
                return RedirectToAction("EditUser", new { hash = idUser, error = "ErrorUserMailValidation" });
            }
            //Verify the user email
            bool userMailValid = true;
            UTILISATEUR userByMail = db.UTILISATEURs.SingleOrDefault(x => x.userMail == model.userMail);
            if (userByMail != null)
            {
                if (userByMail.userId != user.userId) userMailValid = false;
            }
            if (userMailValid)
            {
                try
                {
                    // mise à jour user -> Role
                    List<UserRole> listeUserRole = db.UserRoles.Where(x => x.idUser == user.userId).ToList();
                    for (int i = 0; i < listeUserRole.Count; i++)
                    {
                        db.UserRoles.Remove(listeUserRole[i]);
                    }
                    if (selectedRole.Count() == 1)
                    {
                        UserRole userRole = new UserRole();
                        userRole.idRole = int.Parse(selectedRole[0]);
                        userRole.idUser = user.userId;
                        db.UserRoles.Add(userRole);
                    }
                    else
                    {
                        bool test = false;
                        foreach (var val in selectedRole)
                        {
                            if (val.Contains("1") || val.Contains("2"))
                            {
                                test = true;
                            }
                            else
                            {
                                test = false;
                            }
                        }
                        if (test == true)
                        {
                            UserRole userRole = new UserRole();
                            userRole.idRole = 4;
                            if (user != null) userRole.idUser = user.userId;
                            db.UserRoles.Add(userRole);
                        }
                    }
                   
                    // mise à jour de user
                    user.userNom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.userNom.ToLower());
                    user.userPrenom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.userPrenom.ToLower());
                    user.userMail = model.userMail;

                    user.redactSkype = model.redactSkype;
                    user.redactModePaiement = model.redactModePaiement;
                    user.redactNiveau = model.redactNiveau;
                    user.redactPhone = model.redactPhone;
                    user.redactReferenceur = model.redactReferenceur;
                    user.redactThemes = model.redactThemes;
                    user.redactVolume = model.redactVolume;
                    user.redactTarif = model.redactTarif;
                    
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    Session["userEditModif"] = model;
                    return RedirectToAction("EditUser", new { hash = idUser, error = "ErrorMessage" });
                }
            }
            else
            {
                Session["userEditModif"] = model;
                return RedirectToAction("EditUser", new { hash = idUser, error = "ErrorMail" });
            }
            return View("EditUserConfirmation");
        }


        /// <summary>
        /// Enregistre les modifications d'un Utilisateur dans la base de données.
        /// </summary>
        /// <param name="model">information Utilisateur</param>
        /// <param name="selectedDiv">liste des divisions de l'utilisateur à créer</param>
        /// <param name="selectedRole">liste des roles de l'utilisateur à créer</param>
        /// <param name="idUser">id de l'Utilisateur</param>
        /// <returns>View</returns>
        [Authorize]
        [ValidateInput(false)]
        public ActionResult ModifierRedacteur(UTILISATEURViewModel model, int[] selectedDiv, string[] selectedRole, Guid idUser)
        {
            model.userNom = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userNom));
            model.userPrenom = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userPrenom));
            model.userMail = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userMail));

            model.redactSkype = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactSkype));
            model.redactModePaiement = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactModePaiement));
            model.redactNiveau = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactNiveau));
            model.redactPhone = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactPhone));
            model.redactReferenceur = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactReferenceur));
            model.redactThemes = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactThemes));
            model.redactVolume = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactVolume));

            if (string.IsNullOrEmpty(model.userNom) || string.IsNullOrEmpty(model.userPrenom) || string.IsNullOrEmpty(model.userMail))
            {
                return View("ErrorEditUser");
            }
            if (StatePageSingleton.nullInstance())
            {
                StatePageSingleton.getInstance(1, 10);
                ViewBag.numpage = 1;
                ViewBag.nbrow = 10;
            }
            else
            {
                ViewBag.numpage = StatePageSingleton.getInstance().Numpage;
                ViewBag.nbrow = StatePageSingleton.getInstance().Nbrow;
            }
            _userId = idUser;

            Guid userID = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(userID);

            redactapplicationEntities db = new Models.redactapplicationEntities();
            //Recuperation de l'utilisateur
            UTILISATEUR user = db.UTILISATEURs.SingleOrDefault(x => x.userId == idUser);

            if (selectedRole == null)
            {
                Session["userEditModif"] = model;
                return RedirectToAction("EditUser", new { hash = idUser, error = "ErrorRole" });
            }
            if (Regex.IsMatch(model.userMail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase) == false)
            {
                Session["userEditModif"] = model;
                return RedirectToAction("EditUser", new { hash = idUser, error = "ErrorUserMailValidation" });
            }
            //Verify the user email
            bool userMailValid = true;
            UTILISATEUR userByMail = db.UTILISATEURs.SingleOrDefault(x => x.userMail == model.userMail);
            if (userByMail != null)
            {
                if (userByMail.userId != user.userId) userMailValid = false;
            }
            if (userMailValid)
            {
                try
                {
                    // mise à jour user -> Role
                    List<UserRole> listeUserRole = db.UserRoles.Where(x => x.idUser == user.userId).ToList();
                    for (int i = 0; i < listeUserRole.Count; i++)
                    {
                        db.UserRoles.Remove(listeUserRole[i]);
                    }
                    if (selectedRole.Count() == 1)
                    {
                        UserRole userRole = new UserRole();
                        userRole.idRole = int.Parse(selectedRole[0]);
                        userRole.idUser = user.userId;
                        db.UserRoles.Add(userRole);
                    }
                    else
                    {
                        bool test = false;
                        foreach (var val in selectedRole)
                        {
                            if (val.Contains("1") || val.Contains("2"))
                            {
                                test = true;
                            }
                            else
                            {
                                test = false;
                            }
                        }
                        if (test == true)
                        {
                            UserRole userRole = new UserRole();
                            userRole.idRole = 4;
                            if (user != null) userRole.idUser = user.userId;
                            db.UserRoles.Add(userRole);
                        }
                    }

                    // mise à jour de user
                    user.userNom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.userNom.ToLower());
                    user.userPrenom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.userPrenom.ToLower());
                    user.userMail = model.userMail;

                    user.redactSkype = model.redactSkype;
                    user.redactModePaiement = model.redactModePaiement;
                    user.redactNiveau = model.redactNiveau;
                    user.redactPhone = model.redactPhone;
                    user.redactReferenceur = model.redactReferenceur;
                    user.redactThemes = model.redactThemes;
                    user.redactVolume = model.redactVolume;
                    user.redactTarif = model.redactTarif;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    Session["userEditModif"] = model;
                    return RedirectToAction("EditUser", new { hash = idUser, error = "ErrorMessage" });
                }
            }
            else
            {
                Session["userEditModif"] = model;
                return RedirectToAction("EditUser", new { hash = idUser, error = "ErrorMail" });
            }
            return View("EditUserConfirmation");
        }




        /// <summary>
        /// Annule l'enregistrement d'un Utilisateur dans la base de données.
        /// </summary>
        /// <param name="model">information Utilisateur</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult annulerEnregUtilisateur(UTILISATEURViewModel model)
        {
            return RedirectToAction("GererUtilisateur");
        }

        /// <summary>
        /// Supprime un Utilisateur dans la base de données.
        /// </summary>
        /// <param name="userId">id de l'Utilisateur</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult supprUtilisateur(Guid userId)
        {
            try
            {
                redactapplicationEntities db = new Models.redactapplicationEntities();
                UTILISATEUR utilisateur = db.UTILISATEURs.Where(x => x.userId == userId).FirstOrDefault();

                if (utilisateur != null)
                {
                    db.UTILISATEURs.Remove(utilisateur);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return RedirectToAction("GererUtilisateur");
        }

        /// <summary>
        /// Envoi un mail d'édition de mot de passe Utilisateur et retourne une vue de confirmation.
        /// </summary>
        /// <param name="hash">id de l'Utilisateur</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult sendMailRecovery(Guid? hash)
        {
           
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(user);

            Guid userId = (Guid)hash;
            if (userId != Guid.Empty)
            {
                try
                {
                    Guid userIds = userId;
                    redactapplicationEntities db = new Models.redactapplicationEntities();
                    UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == userIds);
                    Guid TemporaryIdUser = Guid.NewGuid();
                    if (utilisateur != null)
                    {
                        ViewBag.mail = utilisateur.userMail;
                        var url = Request.Url.Scheme;
                        if (Request.Url != null)
                        {

                            string callbackurl = Request.Url.Host != "localhost"
                                ? Request.Url.Host : Request.Url.Authority;
                            var port = Request.Url.Port;
                            if (!string.IsNullOrEmpty(port.ToString()) && Request.Url.Host != "localhost")
                                callbackurl += ":" + port;

                            url += "://" + callbackurl;
                        }

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

                        bool isSendMail = MailClient.SendResetPasswordMail(utilisateur.userMail, mailBody.ToString(), "Redact application - password recovery instructions.");
                        if (isSendMail)
                        {
                            utilisateur.token = TemporaryIdUser;
                            utilisateur.dateToken = DateTime.Now;
                            int result = db.SaveChanges();
                            if (result > 0)
                            {
                                return View();
                            }
                        }
                    }
                    else return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "ListeUser" }
                    });
                }
                catch
                {
                    return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "ListeUser" }
                    });
                }
            }
            return View();
        }

        /// <summary>
        /// Charge une liste d'utilisateur à supprimer dans la base de données.
        /// </summary>
        /// <param name="hash">List d'id d'utilisateur</param>
        /// <returns>bool</returns>
        [Authorize]
        [HttpPost]
        public bool SelecteAllUserToDelete(string hash)
        {
            try
            {
                // Récupère la liste des id d'utilisateur                
                Session["ListUserToDelete"] = hash;
                if (Session["ListUserToDelete"] != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        /// <summary>
        /// Supprime une liste d'Utilisateur dans la base de données.
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult DeleteAllUserSelected()
        {
            Guid userSession = new Guid(HttpContext.User.Identity.Name);
            
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(userSession);
            try
            {
                bool unique = true;
                if (Session["ListUserToDelete"] != null)
                {
                    string hash = Session["ListUserToDelete"].ToString();
                    List<Guid> listIdUser = new List<Guid>();
                    if (!string.IsNullOrEmpty(hash))
                    {
                        if (!hash.Contains(','))
                        {
                            listIdUser.Add(Guid.Parse(hash));
                        }
                        else
                        {
                            foreach (var id in (hash).Split(','))
                            {
                                listIdUser.Add(Guid.Parse(id));
                            }
                            unique = false;
                        }                        
                    }
                    if (listIdUser.Count != 0)
                    {
                        redactapplicationEntities db = new Models.redactapplicationEntities();
                        foreach (var userId in listIdUser)
                        {
                           
                            //suppression des roles
                            var userRoleDeleted = db.UserRoles.Where(x => x.idUser == userId);
                            foreach (var role in userRoleDeleted)
                            {
                                db.UserRoles.Remove(role);
                            }
                            //suppression des utilisateurs
                            UTILISATEUR user = db.UTILISATEURs.SingleOrDefault(x => x.userId == userId);
                            db.UTILISATEURs.Remove(user);
                        }
                        db.SaveChanges();
                       
                        if (unique)
                        {
                            return View("DeletedUserConfirmation");
                        }
                        return View("DeletedAllUserConfirmation");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "ListeUser" }
                    });
        }

        /// <summary>
        /// Retourne le model de l'Utilisateur courant.
        /// </summary>
        /// <returns>UTILISATEURViewModel</returns>
        private UTILISATEURViewModel GetCurrentUserModel()
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            _userId = Guid.Parse(HttpContext.User.Identity.Name);
            UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == _userId);
            UTILISATEURViewModel userVm = new UTILISATEURViewModel();

            if (utilisateur != null)
            {
                userVm.userNom = utilisateur.userNom;
                userVm.userPrenom = utilisateur.userPrenom;
                userVm.userId = utilisateur.userId;
            }
            return userVm;
        }

      }
    
}