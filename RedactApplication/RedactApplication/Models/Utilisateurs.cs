using System;
using System.Collections.Generic;
using System.Linq;


namespace RedactApplication.Models
{
    class Utilisateurs
    {       
        public List<UTILISATEURViewModel> GetListUtilisateur()
        {
            redactapplicationEntities db = new redactapplicationEntities();
            var req = db.UTILISATEURs.ToList();
            List<UTILISATEURViewModel> listeUser = new List<UTILISATEURViewModel>();
            foreach (var user in req)
            {
                string stringRoleUser = this.GetUtilisateurRoleToString(user.userId);
                if (stringRoleUser!="5")
                {
                    listeUser.Add(new UTILISATEURViewModel
                    {
                        userId = user.userId,
                        userNom = user.userNom,
                        userPrenom = user.userPrenom,
                        userMail = user.userMail,
                        userRole = stringRoleUser,
                        redactSkype =  user.redactSkype,
                        redactPhone = user.redactPhone,
                        redactNiveau = user.redactNiveau,
                        redactModePaiement = user.redactModePaiement,
                        redactThemes = user.redactThemes,
                        redactReferenceur = user.redactReferenceur,
                        redactVolume = user.redactVolume,
                        redactTarif = user.redactTarif,
                        redactVolumeRestant = user.redactVolumeRestant
                    });
                }
            }
            return listeUser.OrderBy(x => x.userNom).ThenBy(x => x.userPrenom).ToList();
           
        }
        public List<UTILISATEURViewModel> GetListUtilisateur(int? numpage, int? nbrow)
        {
            try
            {
                List<UTILISATEURViewModel> data = GetListUtilisateur();
                int offset = 0;

                if (nbrow != 10 && nbrow != 50)
                {
                    return data.Distinct().ToList();
                }
                else
                {
                    if (numpage != null) offset = (int) ((numpage - 1) * nbrow);
                    if (offset != 0)
                    {
                        return data.Skip(offset).Take((int)nbrow).Distinct().ToList();
                    }
                    else
                    {
                        return data.Take((int)nbrow).Distinct().ToList();
                    }
                }
            }
            catch
            {
            }
            return new List<UTILISATEURViewModel>();
        }
        public List<int> GetUtilisateurRole(Guid id)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == id);
            var data = (from idrole in db.UserRoles
                        where idrole.idUser == utilisateur.userId
                        select (int)idrole.idRole).ToList<int>();
            return data;
        }
        public string GetUtilisateurRoleToString(Guid id)
        {
            var data = this.GetUtilisateurRole(id);
            if (data != null && data.Count >= 1)
            {
                string x = "";
                foreach (var role in data)
                {
                    x += role.ToString() + ",";
                }
                return x.Substring(0, x.Length - 1);
            }
            return "";
        }
        public UTILISATEUR GetUtilisateur(Guid id)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == id);
            return utilisateur;
        }
        public List<UTILISATEURViewModel> SearchUtilisateur(string userFullName)
        {
            var temp = (new SearchData()).UserSearch(userFullName);
            List<UTILISATEURViewModel> listeUser = temp?.Select(x => new UTILISATEURViewModel
            {
                userId = x.userId,
                userNom = x.userNom,
                userPrenom = x.userPrenom,
                userMail = x.userMail,
                userRole = this.GetUtilisateurRoleToString(x.userId),
                redactSkype = x.redactSkype,
                redactPhone = x.redactPhone,
                redactNiveau = x.redactNiveau,
                redactModePaiement = x.redactModePaiement,
                redactThemes = x.redactThemes,
                redactReferenceur = x.redactReferenceur,
                redactVolume = x.redactVolume,
                redactTarif = x.redactTarif,
                redactVolumeRestant = x.redactVolumeRestant
            }).ToList();
            return listeUser?.OrderBy(x => x.userNom).ThenBy(x => x.userPrenom).ToList();
        }
    }
}