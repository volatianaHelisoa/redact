using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Security.Application;
using RedactApplication.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace RedactApplication.Controllers
{
    public class CommandesController : Controller
    {
        private redactapplicationEntities db = new redactapplicationEntities();
        /// <summary>
        /// Représente l'identifiant de l'utilisateur courant.
        /// </summary>
        public static Guid _userId;

        // GET: COMMANDEs
        public ActionResult Index()
        {
           
            return View();
        }

        public ActionResult Chat()
        {
            return View("ListCommandes");
        }

        public ActionResult ListCommandes()
        {
            // Exécute le suivi de session utilisateur
            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                _userId = Guid.Parse(Request.QueryString["currentid"]);
                Session["currentid"] = Request.QueryString["currentid"];
            }
            else if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                _userId = Guid.Parse(HttpContext.User.Identity.Name);

            // Exécute le traitement de la pagination
            Commandes val = new Commandes();

            // Récupère la liste des commandes
            var listeDataCmde = val.GetListCommande();
            ViewBag.listeCommandeVms = listeDataCmde.Distinct().ToList();

            var currentrole = (new Utilisateurs()).GetUtilisateurRoleToString(_userId);
            if (currentrole != null)
            {
                if (currentrole.Contains("2"))
                {
                    ViewBag.listeCommandeVms = listeDataCmde.Where(x => x.commandeRedacteurId == _userId).ToList();
                    GetRedacteurInformations(_userId);
                }
            }

               
                return View();
        }

            return View("ErrorException");
        }


        public ActionResult ListCommandeAValider()
        {
            // Exécute le suivi de session utilisateur
            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                _userId = Guid.Parse(Request.QueryString["currentid"]);
                Session["currentid"] = Request.QueryString["currentid"];
            }
            else if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                _userId = Guid.Parse(HttpContext.User.Identity.Name);

                // Exécute le traitement de la pagination
                Commandes val = new Commandes();

                // Récupère la liste des commandes
                var listeDataCmde = val.GetListCommande();
                ViewBag.listeCommandeVms = listeDataCmde.Distinct().ToList();

                var currentrole = (new Utilisateurs()).GetUtilisateurRoleToString(_userId);
                if (currentrole != null)
                {
                    if (currentrole.Contains("2"))
                    {
                        ViewBag.listeCommandeVms = listeDataCmde
                            .Where(x => x.commandeRedacteurId == _userId && x.commandeStatutId == null).ToList();
                        //GetRedacteurInformations(_userId);
                    }

                    else
                    {
                        ViewBag.listeCommandeVms = listeDataCmde.Where(x => x.commandeStatutId == null).ToList();
                    }

                }

                return View();
            }

            return View("ErrorException");
        }


        [Authorize]
        [HttpGet]
        public JsonResult GetNotifications()
        {
            // Exécute le suivi de session utilisateur
            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                _userId = Guid.Parse(Request.QueryString["currentid"]);
            }
            else if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                _userId = Guid.Parse(HttpContext.User.Identity.Name);
            }

            return Json(new Notifications().GetAllMessages(), JsonRequestBehavior.AllowGet);
            
        }

        private int GetVolumeEnCours(Guid? redactId)
        {
            var redact = db.UTILISATEURs.Find(redactId);
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            var lastDay = new DateTime(now.Year, now.Month, daysInMonth);
            var commandes = db.COMMANDEs.Where(x => x.commandeRedacteurId == redactId && x.date_cmde >= startOfMonth &&
                                                    x.date_cmde <= lastDay).ToList();
            int volume = 0;
            foreach (var commande in commandes)
            {
                volume += Convert.ToInt32(commande.nombre_mots);
            }

            return volume;
        }

        private void GetRedacteurInformations(Guid redactId)
        {
            var redact = db.UTILISATEURs.Find(redactId);
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            var lastDay = new DateTime(now.Year, now.Month, daysInMonth);

            if (redact != null)
            {
                var commandes = db.COMMANDEs.Where(x=>x.commandeRedacteurId == redactId).ToList();

                var commandesEnCours = commandes.Count(x => x.date_cmde >= startOfMonth &&
                                x.date_cmde <= lastDay && 
                               (x.STATUT_COMMANDE != null && x.STATUT_COMMANDE.statut_cmde.Contains("En cours"))
                                                            );

                ViewBag.commandesEnCours = commandesEnCours;

                var commandesEnAttente = db.COMMANDEs.Count(x => x.date_cmde >= startOfMonth &&
                                             x.date_cmde <= lastDay &&
                                                                 (x.STATUT_COMMANDE != null && x.STATUT_COMMANDE.statut_cmde.Contains("En attente")));

                ViewBag.commandesEnAttente = commandesEnAttente;

                var commandesAValider = db.COMMANDEs.Count(x => x.date_cmde >= startOfMonth &&
                                                                 x.date_cmde <= lastDay &&
                                                                 (x.STATUT_COMMANDE == null && x.commandeStatutId == null));

                ViewBag.commandesAValider = commandesAValider;

                var commandesEnRetard = db.COMMANDEs.Count(x => x.date_cmde >= startOfMonth &&
                                                                x.date_cmde <= lastDay &&
                                                                x.date_livraison <= now &&
                                                                (x.STATUT_COMMANDE != null &&
                                                                 x.STATUT_COMMANDE.statut_cmde.Contains("En cours")));
                ViewBag.commandesEnRetard = commandesEnRetard;
                var commandesFacturer = db.COMMANDEs.Count(x => x.date_cmde >= startOfMonth &&
                                                                  x.date_cmde <= lastDay && (x.STATUT_COMMANDE != null &&
                                                                  x.STATUT_COMMANDE.statut_cmde.Contains("Validé")));
                ViewBag.commandesFacturer = commandesFacturer;

                var commandesLivrer = db.COMMANDEs.Count(x => x.date_cmde >= startOfMonth &&
                                                                x.date_cmde <= lastDay && (x.STATUT_COMMANDE != null &&
                                                                                           x.STATUT_COMMANDE.statut_cmde.Contains("Livré")));
                ViewBag.commandesLivrer = commandesLivrer;

            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CommandesSearch(string searchValue)
        {
            if (StatePageSingleton.nullInstance())
            {
                StatePageSingleton.getInstance(1, 10);
            }
            StatePageSingleton q = StatePageSingleton.getInstance();
            ViewBag.numpage = q.Numpage;
            ViewBag.nbrow = q.Nbrow;
            if (!string.IsNullOrEmpty(searchValue))
            {
                Session["Infosearch"] = searchValue;
            }
            else
            {
                return RedirectToRoute("Home", new RouteValueDictionary {
                    { "controller", "Commandes" },
                    { "action", "ListCommandes" }
                });
            }

            redactapplicationEntities bds = new Models.redactapplicationEntities();
            searchValue = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(searchValue));

            Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRole(user);

            Commandes db = new Commandes();
            var answer = db.SearchCommandes(searchValue);

            if (answer == null || answer.Count == 0)
            {
                List<COMMANDEViewModel> listcCommande = new List<COMMANDEViewModel>();
                answer = listcCommande;
                ViewBag.SearchContactNoResultat = 1;
            }
            ViewBag.Search = true;
            ViewBag.listeCommandeVms = answer.Distinct().ToList();

            COMMANDEViewModel cmdeVm = new COMMANDEViewModel();
            if ((new Commandes()).GetCommandeType(userId) != null)
            {
                cmdeVm.commandeType = (new Commandes()).GetCommandeType(userId).Type;
            }

            return View("ListCommandes", cmdeVm);
        }

        /// <summary>
        /// Charge une liste d'utilisateur à supprimer dans la base de données.
        /// </summary>
        /// <param name="hash">List d'id d'utilisateur</param>
        /// <returns>bool</returns>
        [Authorize]
        [HttpPost]
        public bool SelecteAllCommandeToDelete(string hash)
        {
            try
            {
                // Récupère la liste des id d'utilisateur                
                Session["ListCommandeToDelete"] = hash;
                if (Session["ListCommandeToDelete"] != null)
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
        public ActionResult DeleteAllCommandeSelected()
        {
            Guid userSession = new Guid(HttpContext.User.Identity.Name);

           
            try
            {
                bool unique = true;
                if (Session["ListCommandeToDelete"] != null)
                {
                    string hash = Session["ListCommandeToDelete"].ToString();
                    List<Guid> listIdCmde = new List<Guid>();
                    if (!string.IsNullOrEmpty(hash))
                    {
                        if (!hash.Contains(','))
                        {
                            listIdCmde.Add(Guid.Parse(hash));
                        }
                        else
                        {
                            foreach (var id in (hash).Split(','))
                            {
                                listIdCmde.Add(Guid.Parse(id));
                            }
                            unique = false;
                        }
                    }
                    if (listIdCmde.Count != 0)
                    {
                        redactapplicationEntities db = new Models.redactapplicationEntities();
                        foreach (var cmdeId in listIdCmde)
                        {
                            //suppression des commandes
                            COMMANDE cmde = db.COMMANDEs.SingleOrDefault(x => x.commandeId == cmdeId);
                            if (cmde != null) db.COMMANDEs.Remove(cmde);
                        }
                        db.SaveChanges();

                        return View(unique ? "DeletedCommandeConfirmation" : "DeteleAllCommandeConfirmation");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return RedirectToRoute("Home", new RouteValueDictionary {
                { "controller", "Commandes" },
                { "action", "ListCommandes" }
            });
        }

        /// <summary>
        /// Retourne la vue de validation de suppression d'Utilisateur.
        /// </summary>
        /// <param name="hash">id de l'Utilisateur</param>
        /// <returns>View</returns>
        public ActionResult DeletedUserWaitting(Guid hash)
        {
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.hashCmde = hash;
            return View("DeletedCommandeWaitting");
        }

        /// <summary>
        /// Retourne la vue de confirmation de suppression d'un Utilisateur.
        /// </summary>
        /// <param name="hash">id de l'Utilisateur</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult DeletedCommandeConfirmation(Guid hash)
        {
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
        
            if (hash != Guid.Empty)
            {
                try
                {
                    redactapplicationEntities db = new redactapplicationEntities();
                    COMMANDE cmde = db.COMMANDEs.SingleOrDefault(x => x.commandeId == hash);
                    if (cmde != null)
                    {
                        db.COMMANDEs.Remove(cmde);
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
                        { "controller", "Commandes" },
                        { "action", "ListCommandes" }
                    });
        }

        /// <summary>
        /// Retourne la vue de confirmation de suppression d'une liste d'Utilisateur.
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult DeleteAllCommandeConfirmation()
        {
            Guid userSession = new Guid(HttpContext.User.Identity.Name);
           

            try
            {
                return View("DeletedAllCommandeWaitting");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Commandes" },
                        { "action", "ListCommandes" }
                    });
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
        /// Charge une liste d'utilisateur à supprimer dans la base de données.
        /// </summary>
        /// <param name="hash">List d'id d'utilisateur</param>
        /// <returns>bool</returns>
        [Authorize]
        [HttpPost]
        public bool SelectedTheme(string theme)
        {
            try
            {
                // Récupère la liste des id d'utilisateur                
                Session["ThemeSelected"] = theme;
                if (Session["ThemeSelected"] != null)
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

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadRedacteurByTheme(string theme)
        {
            THEME selectedTheme = db.THEMES.SingleOrDefault(x=>x.themeId.ToString() == theme);
            //Your Code For Getting Physicans Goes Here
            var redactList = (selectedTheme != null)?(new Commandes().GetListRedacteurItem(selectedTheme.theme_name)): null;
    
            return Json(redactList, JsonRequestBehavior.AllowGet);
        }

        private COMMANDEViewModel SetCommandeViewModelDetails(COMMANDE commande)
        {
            Commandes val = new Commandes();
            COMMANDEViewModel commandeVm = new COMMANDEViewModel();
            if (commande != null)
            {
                commandeVm.ListProjet = val.GetListProjetItem();
                commandeVm.ListTheme = val.GetListThemeItem();
                commandeVm.ListRedacteur = val.GetListRedacteurItem();
                commandeVm.ListCommandeType = val.GetListCommandeTypeItem();
                commandeVm.ListContenuType = val.GetListContenuTypeItem();

                if (commande.commandeProjetId != null)
                    commandeVm.listprojetId = (Guid)commande.commandeProjetId;

                if (commande.commandeThemeId != null)
                    commandeVm.listThemeId = (Guid)commande.commandeThemeId;

                if (commande.commandeTypeId != null)
                    commandeVm.listCommandeTypeId = (Guid)commande.commandeTypeId;

                if (commande.commandeRedacteurId != null)
                    commandeVm.listRedacteurId = (Guid)commande.commandeRedacteurId;

                if (commande.consigne_type_contenuId != null)
                    commandeVm.listContenuTypeId = (Guid)commande.consigne_type_contenuId;


                string referenceur = val.GetUtilisateurReferenceur(commande.commandeReferenceurId).userNom;
                    string cmdeType = val.GetCommandeType(commande.commandeTypeId).Type;
                    string consigneType = val.GetCommandeContenuType(commande.consigne_type_contenuId).Type;
                    string redacteur = val.GetUtilisateurReferenceur(commande.commandeRedacteurId).userNom;
                    string priorite = commande.ordrePriorite == "0" ? "Moyen" : "Haut";
                    string projet = val.GetProjet(commande.commandeProjetId).projet_name;
                    string theme = val.GetTheme(commande.commandeThemeId).theme_name;
                    string statutcmde = (commande.commandeStatutId != null) ? val.GetStatutCommande(commande.commandeStatutId).statut_cmde:"";

                    commandeVm.commandeId = commande.commandeId;
                    commandeVm.commandeDemandeur = referenceur;
                    commandeVm.date_cmde = commande.date_cmde;
                    commandeVm.date_livraison = commande.date_livraison;
                    commandeVm.commandeType = cmdeType;
                    commandeVm.nombre_mots = commande.nombre_mots;
                    commandeVm.mot_cle_pricipal = commande.mot_cle_pricipal;
                    commandeVm.mot_cle_secondaire = commande.mot_cle_secondaire;
                    commandeVm.consigne_references = commande.consigne_references;
                    commandeVm.consigneType = consigneType;
                    commandeVm.consigne_autres = commande.consigne_autres;
                    commandeVm.etat_paiement = commande.etat_paiement;
                    commandeVm.commandeRedacteur = redacteur;
                    commandeVm.ordrePriorite = priorite;
                    commandeVm.balise_titre = commande.balise_titre;
                    commandeVm.contenu_livre = commande.contenu_livre;
                    commandeVm.projet = projet;
                    commandeVm.thematique = theme;
                    commandeVm.statut_cmde = statutcmde;
                    Session["cmdeEditModif"] = null;
                

                if (commandeVm.contenu_livre != null) ViewBag.ComptMetaContenu = commandeVm.contenu_livre.Length;

                return commandeVm;
            }

            return null;
        }

        private COMMANDEViewModel SetCommandeDetails(Guid? commandeId)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["not"]))
            {
                Guid? notificationId;
                notificationId = Guid.Parse(Request.QueryString["not"]);
                int res = UpdateStatutNotification(notificationId);
            }


            Commandes val = new Commandes();
            COMMANDEViewModel commandeVm = val.GetDetailsCommande(commandeId);

            if (commandeVm != null)
            {
                if (Session["cmdeEditModif"] != null)
                {
                    COMMANDEViewModel commande = (COMMANDEViewModel)Session["cmdeEditModif"];
                    string referenceur = val.GetUtilisateurReferenceur(commande.commandeReferenceurId).userNom;
                    string cmdeType = val.GetCommandeType(commande.commandeTypeId).Type;
                    string consigneType = val.GetCommandeContenuType(commande.consigne_type_contenuId).Type;
                    string redacteur = val.GetUtilisateurReferenceur(commande.commandeRedacteurId).userNom;
                    string priorite = commande.ordrePriorite == "0" ? "Moyen" : "Haut";
                    string projet = val.GetProjet(commande.commandeProjetId).projet_name;
                    string theme = val.GetTheme(commande.commandeThemeId).theme_name;
                    string statutcmde = val.GetStatutCommande(commande.commandeStatutId).statut_cmde;

                    commandeVm.commandeId = commande.commandeId;
                    commandeVm.commandeDemandeur = referenceur;
                    commandeVm.date_cmde = commande.date_cmde;
                    commandeVm.date_livraison = commande.date_livraison;
                    commandeVm.commandeType = cmdeType;
                    commandeVm.nombre_mots = commande.nombre_mots;
                    commandeVm.mot_cle_pricipal = commande.mot_cle_pricipal;
                    commandeVm.mot_cle_secondaire = commande.mot_cle_secondaire;
                    commandeVm.consigne_references = commande.consigne_references;
                    commandeVm.consigneType = consigneType;
                    commandeVm.consigne_autres = commande.consigne_autres;
                    commandeVm.etat_paiement = commande.etat_paiement;
                    commandeVm.commandeRedacteur = redacteur;
                    commandeVm.ordrePriorite = priorite;
                    commandeVm.balise_titre = commande.balise_titre;
                    commandeVm.contenu_livre = commande.contenu_livre;
                    commandeVm.projet = projet;
                    commandeVm.thematique = theme;
                    commandeVm.statut_cmde = statutcmde;
                    Session["cmdeEditModif"] = null;
                }

                if (commandeVm.contenu_livre != null) ViewBag.ComptMetaContenu = commandeVm.contenu_livre.Length;

                return commandeVm;
            }

            return null;
        }

        // GET: COMMANDEs/Details/5
        public ActionResult DetailsCommande(Guid? hash, string not ="")
        {
            COMMANDEViewModel commandeVm = SetCommandeDetails(hash);
            if (commandeVm != null)
            {
              
                return View(commandeVm);
            }
               
            return View("ErrorException");
        }


        // GET: COMMANDEs/Details/5
        public ActionResult DetailsCommandeAValider(Guid? hash, string not = "")
        {
            COMMANDEViewModel commandeVm = SetCommandeDetails(hash);
            if (commandeVm != null)
                return View(commandeVm);
            return View("ErrorException");
        }


        // GET: COMMANDEs/Create
        public ActionResult Create()
        {
           
            Commandes val = new Commandes();
            COMMANDEViewModel commandeVm = new COMMANDEViewModel();
            commandeVm.ListProjet = val.GetListProjetItem();
            commandeVm.ListTheme = val.GetListThemeItem();
            commandeVm.ListRedacteur = val.GetListRedacteurItem();
            commandeVm.ListCommandeType = val.GetListCommandeTypeItem();
            commandeVm.ListContenuType = val.GetListContenuTypeItem();
            return View(commandeVm);
        }

        // POST: COMMANDEs/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCommande(COMMANDEViewModel model, FormCollection collection)
        {
            var selectedProjetId = model.listprojetId;
            var selectedThemeId = model.listThemeId;
            var selectedRedacteurId = model.listRedacteurId;
            var selectedCommandeTypeId = model.listCommandeTypeId;
            var selectedContentTypeId = model.listContenuTypeId;
            var selectedReferenceurId = Guid.Parse(HttpContext.User.Identity.Name);


            model.mot_cle_secondaire =
                StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.mot_cle_secondaire));
            model.texte_ancrage = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.texte_ancrage));
            model.consigne_references =
                StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.consigne_references));
            model.consigne_autres =
                StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.consigne_autres));
            bool notifSms = false;
            if (!string.IsNullOrEmpty(collection["checkResp"]))
            {
                string checkResp = collection["checkResp"];
                notifSms = checkResp == "on";
            }

            COMMANDE newcommande = new COMMANDE();
            var commande = db.COMMANDEs.Count(x => x.commandeProjetId == selectedProjetId && x.commandeThemeId == selectedThemeId && x.commandeTypeId == model.listCommandeTypeId && x.date_livraison == model.date_livraison) ;
            if (commande <= 0)
            {
                newcommande.PROJET = db.PROJETS.Find(selectedProjetId);
                newcommande.commandeProjetId = selectedProjetId;
                newcommande.THEME = db.THEMES.Find(selectedThemeId);
                newcommande.commandeThemeId = selectedThemeId;
                newcommande.REFERENCEUR = db.UTILISATEURs.Find(selectedReferenceurId);
                newcommande.commandeReferenceurId = selectedReferenceurId;
                newcommande.REDACTEUR = db.UTILISATEURs.Find(selectedRedacteurId);
                newcommande.commandeRedacteurId = selectedRedacteurId;
            
                newcommande.COMMANDE_TYPE = db.COMMANDE_TYPE.Find(selectedCommandeTypeId);
                newcommande.commandeTypeId = selectedCommandeTypeId;
                newcommande.CONTENU_TYPE = db.CONTENU_TYPE.Find(selectedContentTypeId);
                newcommande.consigne_type_contenuId = selectedContentTypeId;
                newcommande.mot_cle_pricipal =
                    StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.mot_cle_pricipal));
                newcommande.mot_cle_secondaire =
                    StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.mot_cle_secondaire));
                newcommande.texte_ancrage =
                    StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.texte_ancrage));
                newcommande.nombre_mots = model.nombre_mots;
                newcommande.consigne_references = model.consigne_references;
                newcommande.consigne_type_contenuId = model.listContenuTypeId;
                newcommande.consigne_autres = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.consigne_autres));
              
                newcommande.date_livraison = model.date_livraison;
                newcommande.date_cmde = DateTime.Now;
                int? maxRef = db.COMMANDEs.Max(u => u.commandeREF);
                newcommande.commandeREF = maxRef + 1;
                newcommande.commandeId = Guid.NewGuid();
                int? volume = GetVolumeEnCours(newcommande.commandeRedacteurId);
                if (newcommande.REDACTEUR != null && volume > Convert.ToInt32(newcommande.REDACTEUR.redactVolume) && Session["VolumeInfo"] == null)
                {
                    Session["VolumeInfo"] = "Le volume maximal du mois pour le rédacteur " + newcommande.REDACTEUR.userNom + " est atteint. Vous confirmez l'envoi de la commande ? ";
                    COMMANDEViewModel cmd = SetCommandeViewModelDetails(newcommande);
                    return View("Create", cmd);
                }

                Session["VolumeInfo"] = null;

                db.COMMANDEs.Add(newcommande);
                try
                {
                    db.SaveChanges();
                    if (notifSms)
                    {
                        string msgBody = "Vous avez une nouvelle commande.";
                        var accountSid =
                            System.Configuration.ConfigurationManager.AppSettings["SMSAccountIdentification"];
                        var authToken = System.Configuration.ConfigurationManager.AppSettings["SMSAccountPassword"];
                        var phonenumber = System.Configuration.ConfigurationManager.AppSettings["SMSAccountFrom"];

                        TwilioClient.Init(accountSid, authToken);
                        if (newcommande.REDACTEUR != null)
                        {
                            string redactNumber = newcommande.REDACTEUR.redactPhone;
                            var to = new PhoneNumber(redactNumber);
                            var message = MessageResource.Create(
                                to,
                                @from: new PhoneNumber(phonenumber),
                                body: msgBody);

                            if (!string.IsNullOrEmpty(message.Sid))
                            {
                                Console.WriteLine(message.Sid);
                                //return View("Create");
                            }
                        }
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

                        
                        StringBuilder mailBody = new StringBuilder();
                        if (newcommande.REDACTEUR != null)
                        {
                            mailBody.AppendFormat(
                                "Dear " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(newcommande.REDACTEUR.userNom
                                    .ToLower()));
                            mailBody.AppendFormat("<br />");
                            mailBody.AppendFormat(
                                "<p>Vous avez une nouvelle commande à valider. Cliquez sur le lien suivant pour accepter ou refuser la commande.</p>");
                            mailBody.AppendFormat("<br />");
                            mailBody.AppendFormat(url + "/Commandes/CommandeWaitting?token=" + newcommande.commandeId);
                            mailBody.AppendFormat("<br />");
                            mailBody.AppendFormat("Cordialement,");
                            mailBody.AppendFormat("<br />");
                            mailBody.AppendFormat("Mediaclick Company.");

                            bool isSendMail = MailClient.SendResetPasswordMail(newcommande.REDACTEUR.userMail,
                                mailBody.ToString(), "Redact application - nouvelle commande.");
                            if (isSendMail)
                            {
                                newcommande.commandeToken = newcommande.commandeId;
                                newcommande.dateToken = DateTime.Now;
                                int result = db.SaveChanges();
                                if (result > 0)
                                {
                                    if (SendNotification(newcommande, newcommande.commandeReferenceurId, newcommande.commandeRedacteurId) > 0)
                                        return View("CreateCommandeConfirmation");

                                    return RedirectToRoute("Home", new RouteValueDictionary
                                    {
                                        {"controller", "Commandes"},
                                        {"action", "Create"}
                                    });

                                }
                            }
                        }
                    }
                }
                catch (DbUpdateException ex)
                {

                    return RedirectToRoute("Home", new RouteValueDictionary
                    {
                        {"controller", "Commandes"},
                        {"action", "Create"}
                    });
                }
            }

            return View("ErrorException");
        }

        public ActionResult CommandeConfirmationVolume()
        {
            return View("CreateCommandeConfirmation");
        }


        /*pour aller a la page de modification du mot de passe*/
        public ActionResult CommandeWaitting(Guid? token)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            COMMANDE commande = db.COMMANDEs.SingleOrDefault(x => x.commandeToken == token);
            COMMANDEViewModel commandeVm = new Commandes().GetDetailsCommande(token);
            /*l'utilisateur est null si le token n'existe pas/plus dans la base de donnees*/
            if (commande == null)
            {
                return View("ExpiredLink");
            }

            DateTime now = DateTime.Now;
            if (commande.dateToken != null)
            {
                DateTime dateToken = (DateTime)commande.dateToken;
                double nbrTime = (now - dateToken).TotalMinutes;
                if (nbrTime > 60.0)
                {
                    return View("ExpiredLink");
                }
            }
            Session["tokenCmde"] = token;
            ViewBag.hashCmde = token;
            return View(commandeVm);
        }

      

        /*pour aller a la page de modification du mot de passe*/
        public ActionResult AcceptCommande(Guid? hash)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            var tokenCmde = !string.IsNullOrEmpty(hash.ToString())?hash: Session["tokenCmde"];
            COMMANDE commande;
            if (tokenCmde != null)
            {
                commande = db.COMMANDEs.SingleOrDefault(x =>x.commandeToken == (Guid) tokenCmde);


                /*l'utilisateur est null si le token n'existe pas/plus dans la base de donnees*/
                if (commande == null)
                {
                    return RedirectToAction("ExpiredLink", "Login");
                }

                commande.commandeToken = null;
                commande.dateToken = null;
                var status = db.STATUT_COMMANDE.SingleOrDefault(s => s.statut_cmde.Contains("En cours"));
                commande.STATUT_COMMANDE = status;
                if (status != null) commande.commandeStatutId = status.statutCommandeId;
                db.SaveChanges();
                SendNotification(commande,commande.commandeRedacteurId,commande.commandeReferenceurId);
            }

            return RedirectToRoute("Home", new RouteValueDictionary
            {
                {"controller", "Commandes"},
                {"action", "ListCommandes"}
            });
        }

        public int SendNotification( COMMANDE commande,Guid? fromId, Guid? toId)
        {
            var notif = new  NOTIFICATION();
            notif.commandeId = commande.commandeId;
            notif.fromId = fromId;
            notif.toId = toId;
            notif.statut = true;
            notif.datenotif = DateTime.Now;
            notif.notificationId = Guid.NewGuid();
            db.NOTIFICATIONs.Add(notif);
            var res = db.SaveChanges();

            return res;

        }

        public int UpdateStatutNotification(Guid? notificationId)
        {
            var notif =  db.NOTIFICATIONs.SingleOrDefault(x=>x.notificationId == notificationId);
            
            notif.statut = false;
            
            
            var res = db.SaveChanges();

            return res;

        }


        // GET: COMMANDEs/Edit/5
        public ActionResult Edit(Guid? hash)
        {
            if (hash == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            Commandes val = new Commandes();
            var currentCommande = val.GetDetailsCommande(hash);

            if (Session["cmdeEditModif"] != null)
                currentCommande = (COMMANDEViewModel)Session["cmdeEditModif"];

            currentCommande.ListProjet = val.GetListProjetItem();
            currentCommande.ListProjet = val.GetListProjetItem();
            currentCommande.ListTheme = val.GetListThemeItem();
            currentCommande.ListRedacteur = val.GetListRedacteurItem();
            currentCommande.ListCommandeType = val.GetListCommandeTypeItem();
            currentCommande.ListContenuType = val.GetListContenuTypeItem();
            currentCommande.ListStatut =val.GetListStatutItem();
            if (currentCommande.commandeProjetId != null)
                currentCommande.listprojetId = (Guid) currentCommande.commandeProjetId;

            if (currentCommande.commandeThemeId != null)
                currentCommande.listThemeId = (Guid)currentCommande.commandeThemeId;

            if (currentCommande.commandeTypeId != null)
                currentCommande.listCommandeTypeId = (Guid)currentCommande.commandeTypeId;

            if (currentCommande.commandeRedacteurId != null)
                currentCommande.listRedacteurId = (Guid)currentCommande.commandeRedacteurId;

            if (currentCommande.consigne_type_contenuId != null)
                currentCommande.listContenuTypeId = (Guid)currentCommande.consigne_type_contenuId;

            
               

            Session["cmdeEditModif"] = null;
            return View(currentCommande);
           
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        [MvcApplication.CheckSessionOut]
        public ActionResult EditCommande(Guid idCommande, COMMANDEViewModel model, FormCollection collection)
        {
            var hash = idCommande;
            COMMANDE commande = db.COMMANDEs.Find(hash);
            Guid? fromId =_userId;
            Guid? toId;
            if (commande != null)
            {
                bool notifSms = false;
                if (Session["role"] != null && Session["role"].ToString() == "2")
                {
                    string etat = "Livré";
                    var statut = db.STATUT_COMMANDE.SingleOrDefault(x => x.statut_cmde.Contains(etat));
                    commande.commandeStatutId = statut.statutCommandeId;

                    commande.balise_titre = model.balise_titre;
                    commande.contenu_livre = model.contenu_livre;

                    commande.dateLivraisonReel = DateTime.Now;
                    toId = commande.commandeReferenceurId;
                }

                else
                {
                    toId = commande.commandeRedacteurId;
                    var selectedProjetId = model.listprojetId;
                    var selectedThemeId = model.listThemeId;
                    var selectedRedacteurId = model.listRedacteurId;
                    var selectedCommandeTypeId = model.listCommandeTypeId;
                    var selectedContentTypeId = model.listContenuTypeId;
                    var selectedReferenceurId = Guid.Parse(HttpContext.User.Identity.Name);


                    model.mot_cle_secondaire =
                        StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.mot_cle_secondaire));
                    model.texte_ancrage =
                        StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.texte_ancrage));
                    model.consigne_references =
                        StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.consigne_references));
                    model.consigne_autres =
                        StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.consigne_autres));
                   
                    if (!string.IsNullOrEmpty(collection["checkResp"]))
                    {
                        string checkResp = collection["checkResp"];
                        notifSms = checkResp == "on";
                    }
                    
                    commande.PROJET = db.PROJETS.Find(selectedProjetId);
                    commande.commandeProjetId = selectedProjetId;
                    commande.THEME = db.THEMES.Find(selectedThemeId);
                    commande.commandeThemeId = selectedThemeId;
                    commande.REFERENCEUR = db.UTILISATEURs.Find(selectedReferenceurId);
                    commande.commandeReferenceurId = selectedReferenceurId;
                    commande.REDACTEUR = db.UTILISATEURs.Find(selectedRedacteurId);
                    commande.commandeRedacteurId = selectedRedacteurId;

                    commande.COMMANDE_TYPE = db.COMMANDE_TYPE.Find(selectedCommandeTypeId);
                    commande.commandeTypeId = selectedCommandeTypeId;
                    commande.CONTENU_TYPE = db.CONTENU_TYPE.Find(selectedContentTypeId);
                    commande.consigne_type_contenuId = selectedContentTypeId;
                    commande.mot_cle_pricipal =
                        StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.mot_cle_pricipal));
                    commande.mot_cle_secondaire =
                        StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.mot_cle_secondaire));
                    commande.texte_ancrage =
                        StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.texte_ancrage));
                    commande.nombre_mots = model.nombre_mots;
                    commande.consigne_references = model.consigne_references;
                    commande.consigne_type_contenuId = model.listContenuTypeId;
                    commande.consigne_autres =
                        StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.consigne_autres));

                    commande.date_livraison = model.date_livraison;
                    int? volume = GetVolumeEnCours(commande.commandeRedacteurId);
                    if (commande.REDACTEUR != null && volume > Convert.ToInt32(commande.REDACTEUR.redactVolume) && Session["VolumeInfo"] == null)
                    {
                        Session["VolumeInfo"] = "Le volume maximal du mois pour le rédacteur " + commande.REDACTEUR.userNom + " est atteint. Vous confirmez l'envoi de la commande ? ";
                        COMMANDEViewModel cmd = SetCommandeViewModelDetails(commande);
                        return View("Edit", cmd);
                    }
                }
            
            try
                {
                    Session["VolumeInfo"] = null;
                    int result = db.SaveChanges();
                    if (result > 0)
                    {
                        if (notifSms)
                        {
                            string msgBody = "Vous avez une commande mise à jour.";
                            var accountSid =
                                System.Configuration.ConfigurationManager.AppSettings["SMSAccountIdentification"];
                            var authToken = System.Configuration.ConfigurationManager.AppSettings["SMSAccountPassword"];
                            var phonenumber = System.Configuration.ConfigurationManager.AppSettings["SMSAccountFrom"];

                            TwilioClient.Init(accountSid, authToken);
                            if (commande.REDACTEUR != null)
                            {
                                string redactNumber = commande.REDACTEUR.redactPhone;
                                var to = new PhoneNumber(redactNumber);
                                var message = MessageResource.Create(
                                    to,
                                    @from: new PhoneNumber(phonenumber),
                                    body: msgBody);

                                if (!string.IsNullOrEmpty(message.Sid))
                                {
                                    Console.WriteLine(message.Sid);

                                }
                            }
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


                            StringBuilder mailBody = new StringBuilder();
                            if (commande.REDACTEUR != null)
                            {
                                mailBody.AppendFormat(
                                    "Dear " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(commande.REDACTEUR.userNom
                                        .ToLower()));
                                mailBody.AppendFormat("<br />");
                                mailBody.AppendFormat(
                                    "<p>Vous avez une commande à corriger. Connectez-vous et voir le lien suivant pour voir les détails de la commande.</p>");
                                mailBody.AppendFormat("<br />");
                                mailBody.AppendFormat(url + "/Commandes/DetailsCommande?hash=" + commande.commandeId);
                                mailBody.AppendFormat("<br />");
                                mailBody.AppendFormat("Cordialement,");
                                mailBody.AppendFormat("<br />");
                                mailBody.AppendFormat("Mediaclick Company.");

                                //bool isSendMail = MailClient.SendResetPasswordMail(commande.REDACTEUR.userMail,
                                //    mailBody.ToString(), "Redact application - nouvelle commande.");
                                //if (isSendMail)
                                //{

                                    if (SendNotification(commande,fromId,toId) > 0)
                                        return View("EditCommandeConfirmation");

                                    return RedirectToRoute("Home", new RouteValueDictionary
                                    {
                                        {"controller", "Commandes"},
                                        {"action", "Edit"}
                                    });

                                //}
                            }
                        }
                    }
                }
                catch (DbUpdateException ex)
                {
                    Session["cmdeEditModif"] = model;
                    return RedirectToRoute("Home", new RouteValueDictionary
                    {
                        {"controller", "Commandes"},
                        {"action", "Edit"}
                    });
                }
            }

            return View("ErrorException");

        }

        // GET: COMMANDEs/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            COMMANDE cOMMANDE = db.COMMANDEs.Find(id);
            if (cOMMANDE == null)
            {
                return HttpNotFound();
            }
            return View(cOMMANDE);
        }

        // POST: COMMANDEs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            COMMANDE cOMMANDE = db.COMMANDEs.Find(id);
            db.COMMANDEs.Remove(cOMMANDE);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
