namespace Ponant.Medical.Shore.Models
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using MsgReader.Outlook;
    using Ponant.Medical.Common;
    using Ponant.Medical.Common.MailServer;
    using Ponant.Medical.Data.Auth;
    using Ponant.Medical.Shore.Helpers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;

    #region Modèles des vues

    #region VerifyCodeViewModel
    /// <summary>
    /// Modèle de vérification du code
    /// </summary>
    public class VerifyCodeViewModel
    {
        /// <summary>
        /// Code envoyé par mail
        /// </summary>
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// Adresse demandée
        /// </summary>
        public string ReturnUrl { get; set; }
    }
    #endregion

    #region LoginViewModel
    /// <summary>
    /// Modèle de connexion à l'application
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Nom d'utilisateur
        /// </summary>
        [Required]
        [Display(Name = "Login")]
        [StringLength(64)]
        public string Login { get; set; }

        /// <summary>
        /// Mot de passe
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(64)]
        public string Password { get; set; }
    }
    #endregion

    #region ChangePasswordViewModel
    /// <summary>
    /// Modèle de modification du mot de passe
    /// </summary>
    public class ChangePasswordViewModel
    {
        /// <summary>
        /// Ancien mot de passe
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        /// <summary>
        /// Nouveau mot de passe
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [NotEqual("OldPassword")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Confirmation du nouveau mot de passe
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Adresse demandée
        /// </summary>
        public string ReturnUrl { get; set; }
    }
    #endregion

    #region vUser
    /// <summary>
    /// Vue de la grille des utilisateurs
    /// </summary>
    public class vUser
    {
        /// <summary>
        /// Identifiant de l'utilisateur
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Nom de l'agence
        /// </summary>
        public string AgencyName { get; set; }

        /// <summary>
        /// Nom de l'utilisateur
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Prénom de l'utilisateur
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Login de l'utilisateur
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Adresse électronique de l'utilisateur
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Utilisateur actif
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Rôle de l'utilisateur
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Logo
        /// </summary>
        public string LogoName { get; set; }

        /// <summary>
        /// Identifiant du role de l'utilisateur
        /// </summary>
        public string RoleId { get; set; }        

        /// <summary>
        /// Nom du bateau
        /// </summary>
        public string Ship { get; set; }

        /// <summary>
        /// Identifiant du bateau
        /// </summary>
        public int? IdShip { get; set; }

        /// <summary>
        /// Identifiant de l'agence
        /// </summary>
        public int IdAgency { get; set; }
    }
    #endregion

    #region UserViewModel
    public class UserViewModel
    {
        /// <summary>
        /// id de l'agence
        /// </summary>
        [Required]
        [Display(Name = "Agency")]
        public int? IdAgency { get; set; }

        public string AgencyName { get; set; }

        /// <summary>
        /// Nom d'utilisateur
        /// </summary>
        [Required]
        [Display(Name = "Login")]
        [StringLength(256)]
        [System.Web.Mvc.Remote("CheckUserNameValue", "User", AdditionalFields = "Id", ErrorMessage = "Login already exists", HttpMethod = "GET")]
        public string UserName { get; set; }

        /// <summary>
        /// Adresse électronique
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }

        /// <summary>
        /// Nom de famille
        /// </summary>
        [Required]
        [StringLength(32)]
        public string LastName { get; set; }

        /// <summary>
        /// Prénom
        /// </summary>
        [Required]
        [StringLength(32)]
        public string FirstName { get; set; }

        /// <summary>
        /// Utilisateur actif
        /// </summary>
        [Required]
        [Display(Name = "Shore access")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Identifiant du rôle
        /// </summary>
        [Required]
        public string Role { get; set; }

        /// <summary>
        /// Nom du logo
        /// </summary>
        [Display(Name = "Logo")]
        public string LogoName { get; set; }

        /// <summary>
        /// Identifiant
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Mot de passe crypté
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Nom de la personne qui a créé l'utilisateur
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// Nom de la personne qui a modifié l'utilisateur
        /// </summary>
        public string Editor { get; set; }

        /// <summary>
        /// Date de création
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Date de modification
        /// </summary>
        public DateTime ModificationDate { get; set; }

        /// <summary>
        /// L'adresse électronique a été confirmée
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Le mot de passe est à changer
        /// </summary>
        public bool PasswordChange { get; set; }

        /// <summary>
        /// L'authentification à deux facteurs est activée
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Le numéro de téléphone a été confirmé
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Le compte est bloqué
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// ID Ship de connexion
        /// </summary>
        ///    //[Remote("CheckOrderValue", "Criteria", AdditionalFields = "Id", ErrorMessage = "Order value already exists", HttpMethod = "GET")]
        [System.Web.Mvc.Remote("CheckRoleValueShip", "User", AdditionalFields = "Role", ErrorMessage = "Please choose the ship", HttpMethod = "GET")]
        [Display(Name = "Ship name")]
        public int? IdShip { get; set; }
    }
    #endregion

    #region EditUserViewModel
    /// <summary>
    /// Formulaire de modification d'un utilisateur
    /// </summary>
    public class EditUserViewModel : UserViewModel
    { }
    #endregion

    #region CreateUserViewModel
    /// <summary>
    /// Formulaire de création d'un utilisateur
    /// </summary>
    public class CreateUserViewModel : UserViewModel
    {
        /// <summary>
        /// Mot de passe
        /// </summary>
        [Required]
        [RegularExpression(@"^[A-Za-z0-9]{8,}$", ErrorMessage = "The password must be composed of at least one digit and one capital letter on 8 characters.")]
        public string Password { get; set; }
    }
    #endregion

    #region ApplicationUser
    /// <summary>
    /// Classe des utilisateurs de l'application
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Nom de famille
        /// </summary>
        public virtual string LastName { get; set; }

        /// <summary>
        /// Prénom
        /// </summary>
        public virtual string FirstName { get; set; }

        /// <summary>
        /// Utilisateur actif
        /// </summary>
        public virtual bool Enabled { get; set; }

        /// <summary>
        /// Date de dernière connexion
        /// </summary>
        public DateTime? LastConnectionDate { get; set; }

        /// <summary>
        /// Le mot de passe est à changer
        /// </summary>
        public bool PasswordChange { get; set; }

        /// <summary>
        /// Le hash du mot de passe initial
        /// </summary>
        public string PasswordHashInit { get; set; }

        /// <summary>
        /// Nom de la personne qui a créé l'utilisateur
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// Date de création
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Nom de la personne qui a modifié l'utilisateur
        /// </summary>
        public string Editor { get; set; }

        /// <summary>
        /// Date de modification
        /// </summary>
        public DateTime ModificationDate { get; set; }

        /// <summary>
        /// Identifiant du bateau
        /// </summary>
        public int? IdShip { get; set; }

        /// <summary>
        /// Id de l'agence
        /// </summary>
        public int? IdAgency { get; set; }

        /// <summary>
        /// Nom de l'agence
        /// </summary>
        //public string AgencyName { get; set; }

        /// <summary>
        /// Nom du logo
        /// </summary>
        public string LogoName { get; set; }
        
        /// <summary>
        /// Personnalise les claims
        /// </summary>
        /// <param name="manager">Instance du gestionnaire de comptes</param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Notez qu'authenticationType doit correspondre à l'élément défini dans CookieAuthenticationOptions.AuthenticationType
            ClaimsIdentity userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Ajouter les revendications personnalisées de l’utilisateur ici
            return userIdentity;
        }
    }
    #endregion

    #endregion

    #region Contexte de BD
    /// <summary>
    /// Contexte de connexion à la base de données
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Vue affichée pour la liste des utilisateurs
        /// </summary>
        public DbSet<vUser> vUsers { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        public ApplicationDbContext()
            : base("AuthConnectionString", throwIfV1Schema: false)
        {
        }

        /// <summary>
        /// Initialisation
        /// </summary>
        /// <returns></returns>
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
    #endregion

    #region Gestion des utilisateurs
    /// <summary>
    /// Classe de gestion des utilisateurs
    /// </summary>
    public class UserClass
    {
        #region Properties & Constructors

        private readonly ApplicationDbContext _applicationDbContext;

        public UserClass(
            ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        #endregion

        #region Create
        /// <summary>
        /// Création d'un utilisateur
        /// </summary>
        /// <param name="user">Utilisateur à créer</param>
        /// <returns></returns>
        public async Task Create(CreateUserViewModel user)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            try
            {
                // Enregistrement en base       
                applicationUser.Id = Guid.NewGuid().ToString();
                applicationUser.SecurityStamp = Guid.NewGuid().ToString();
                applicationUser.IdAgency = user.IdAgency;
                applicationUser.LastName = user.LastName;
                applicationUser.FirstName = user.FirstName;
                applicationUser.UserName = user.UserName;
                applicationUser.Email = user.Email;
                applicationUser.Enabled = user.Enabled;
                applicationUser.LogoName = user.LogoName;

                string passwordHash = UserHelper.CalculateHash(user.Password);
                applicationUser.PasswordHash = passwordHash;
                applicationUser.PasswordHashInit = passwordHash;
                applicationUser.CreationDate = applicationUser.ModificationDate = DateTime.Now;
                applicationUser.Creator = applicationUser.Editor = HttpContext.Current.User.Identity.Name;
                applicationUser.EmailConfirmed = applicationUser.Enabled = applicationUser.PasswordChange = applicationUser.TwoFactorEnabled = applicationUser.LockoutEnabled = true;
                applicationUser.PhoneNumberConfirmed = false;
                applicationUser.LockoutEnabled = true;
                applicationUser.Roles.Add(new IdentityUserRole() { RoleId = user.Role, UserId = applicationUser.Id });
                applicationUser.IdShip = user.Role == Data.Constants.ROLE_ID_BOARD ? user.IdShip : null;

                _applicationDbContext.Users.Add(applicationUser);
                _applicationDbContext.SaveChanges();

                _SaveUserLogos(ref applicationUser, user.LogoName);
                _applicationDbContext.SaveChanges();
                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.User, LogManager.LogAction.Add, HttpContext.Current.User.Identity.Name, "Add User Id : " + applicationUser.Id);

                // Envoi du mail contenant le login
                using (Storage.Message message = new Storage.Message(Path.Combine(AppSettings.FolderMail, AppSettings.MailAccountUserName)))
                {
                    await MailServer.Send(new Mail()
                    {
                        Body = message.BodyHtml.Replace(AppSettings.TagLastName, applicationUser.LastName).Replace(AppSettings.TagFirstName, applicationUser.FirstName).Replace(AppSettings.TagUserName, applicationUser.UserName),
                        From = AppSettings.AddressNoReply,
                        Recipients = new List<Recipient>() { new Recipient(string.Format("{0} {1}", applicationUser.FirstName, applicationUser.LastName), applicationUser.Email) },
                        Subject = message.Subject
                    });
                }

                // Envoi du mail contenant le mot de passe
                using (Storage.Message message = new Storage.Message(Path.Combine(AppSettings.FolderMail, AppSettings.MailAccountPassword)))
                {
                    await MailServer.Send(new Mail()
                    {
                        Body = message.BodyHtml.Replace(AppSettings.TagLastName, applicationUser.LastName).Replace(AppSettings.TagFirstName, applicationUser.FirstName).Replace(AppSettings.TagPassword, user.Password),
                        From = AppSettings.AddressNoReply,
                        Recipients = new List<Recipient>() { new Recipient(string.Format("{0} {1}", applicationUser.FirstName, applicationUser.LastName), applicationUser.Email) },
                        Subject = message.Subject
                    });
                }
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.User, LogManager.LogAction.Add, HttpContext.Current.User.Identity.Name, "Add User Id : " + applicationUser.Id + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// Suppression d'un utilisateur
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        public void Delete(string id)
        {
            try
            {
                ApplicationUser applicationUser = _applicationDbContext.Users.Find(id);
                applicationUser.Roles.Clear();
                DeleteUserShips(id);
                _applicationDbContext.Users.Remove(applicationUser);
                _applicationDbContext.SaveChanges();
                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.User, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete User Id : " + id);
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.User, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete User Id : " + id + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion

        #region Edit
        /// <summary>
        /// Modifie un utilisateur
        /// </summary>
        /// <param name="user">Utilisateur à modifier</param>
        /// <returns></returns>
        public async Task Edit(EditUserViewModel user)
        {
            ApplicationUser applicationUser = null;
            try
            {
                applicationUser = _applicationDbContext.Users.Find(user.Id);
                bool IsUserNameChanged = applicationUser.UserName != user.UserName;
                applicationUser.IdAgency = user.IdAgency.Value;
                applicationUser.LastName = user.LastName;
                applicationUser.FirstName = user.FirstName;
                applicationUser.UserName = user.UserName;
                applicationUser.Email = user.Email;
                applicationUser.Enabled = user.Enabled;
                applicationUser.LogoName = user.LogoName;
                applicationUser.ModificationDate = DateTime.Now;
                applicationUser.Editor = HttpContext.Current.User.Identity.Name;

                applicationUser.Roles.Clear();
                applicationUser.Roles.Add(new IdentityUserRole() { RoleId = user.Role, UserId = applicationUser.Id });

                applicationUser.IdShip = user.Role == Data.Constants.ROLE_ID_BOARD ? user.IdShip : null;
                _applicationDbContext.SaveChanges();

                _SaveUserLogos(ref applicationUser, user.LogoName);
                _applicationDbContext.SaveChanges();
                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.User, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name, "Edit User Id : " + user.Id);

                // Envoi du mail contenant le login
                if (IsUserNameChanged)
                {
                    using (Storage.Message message = new Storage.Message(Path.Combine(AppSettings.FolderMail, AppSettings.MailNewUserName)))
                    {
                        await MailServer.Send(new Mail()
                        {
                            Body = message.BodyHtml.Replace(AppSettings.TagLastName, applicationUser.LastName).Replace(AppSettings.TagFirstName, applicationUser.FirstName).Replace(AppSettings.TagUserName, applicationUser.UserName),
                            From = AppSettings.AddressNoReply,
                            Recipients = new List<Recipient>() { new Recipient(string.Format("{0} {1}", applicationUser.FirstName, applicationUser.LastName), applicationUser.Email) },
                            Subject = message.Subject
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                DeleteOldLogos(applicationUser, user, false);
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.User, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name, "Edit User Id : " + user.Id + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion

        #region GetRoles
        /// <summary>
        /// Retourne les rôles d'un utilisateur
        /// </summary>
        /// <returns>Liste des rôles d'un utilisateur</returns>
        public List<System.Web.Mvc.SelectListItem> GetRoles()
        {
            if (HttpContext.Current.User.IsInRole(Data.Constants.ROLE_NAME_BOOKING_ADMINISTRATOR))
            {
                return (from r in _applicationDbContext.Roles
                        where r.Name.Equals(Data.Constants.ROLE_NAME_BOOKING)
                        orderby r.Name ascending
                        select new System.Web.Mvc.SelectListItem
                        {
                            Text = r.Name,
                            Value = r.Id
                        }).ToList();
            }
            else if (HttpContext.Current.User.IsInRole(Data.Constants.ROLE_NAME_AGENCY_ADMINISTRATOR))
            {
                return (from r in _applicationDbContext.Roles
                        where r.Name.Equals(Data.Constants.ROLE_NAME_AGENCY)
                        orderby r.Name ascending
                        select new System.Web.Mvc.SelectListItem
                        {
                            Text = r.Name,
                            Value = r.Id
                        }).ToList();
            }
            else if (HttpContext.Current.User.IsInRole(Data.Constants.ROLE_NAME_GROUP))
            {
                return (from r in _applicationDbContext.Roles
                        where r.Name.Equals(Data.Constants.ROLE_NAME_AGENCY_ADMINISTRATOR)
                        orderby r.Name ascending
                        select new System.Web.Mvc.SelectListItem
                        {
                            Text = r.Name,
                            Value = r.Id
                        }).ToList();
            }
            else
            {
                return (from r in _applicationDbContext.Roles
                        orderby r.Name ascending
                        select new System.Web.Mvc.SelectListItem
                        {
                            Text = r.Name,
                            Value = r.Id
                        }).ToList();
            }
        }
        #endregion

        #region GetUser
        /// <summary>
        /// Retourne un utilisateur pour la vue de modification
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <returns>Un utilisateur</returns>
        public EditUserViewModel GetUser(string id)
        {
            EditUserViewModel user = new EditUserViewModel();
            vUser applicationUser = _applicationDbContext.vUsers.Find(id);

            if (applicationUser != null)
            {
                user.Id = applicationUser.Id;
                user.IdAgency = applicationUser.IdAgency;
                user.AgencyName = applicationUser.AgencyName;
                user.LastName = applicationUser.LastName;
                user.FirstName = applicationUser.FirstName;
                user.UserName = applicationUser.Login;
                user.Email = applicationUser.Email;
                user.Enabled = applicationUser.Enabled;
                user.Role = applicationUser.RoleId;
                user.IdShip = applicationUser.IdShip;
                user.LogoName = applicationUser.LogoName;
            }
            return user;
        }
        #endregion

        #region GetFileName
        /// <summary>
        /// Récupere le nom du fichier
        /// </summary>
        /// <param name="id">Id de l'utilisateur concerné</param>
        /// <param name="fileType">Type du fichier voulu</param>
        /// <returns>Nom du fichier</returns>
        public string GetFileName(string idUser)
        {
            string filename = null;
            filename = _applicationDbContext.Users.Find(idUser).LogoName;
            return filename;
        }
        #endregion

        #region GetUserId
        /// <summary>
        /// Récupere l'id de l'utilisateur
        /// </summary>
        /// <param name="login">login de l'utilisateur</param>
        /// <returns>Nom du fichier</returns>
        public string GetUserId(string login)
        {
            string id = null;
            id = _applicationDbContext.vUsers.FirstOrDefault(u => u.Login.Equals(login)).Id;
            return id;
        }
        #endregion

        #region GetAgencyId
        /// <summary>
        /// Récupere l'id de l'agence
        /// </summary>
        /// <param name="idUser">Id de l'utilisateur</param>
        /// <returns>Id de l'Agence</returns>
        public int GetAgencyId(string idUser)
        {
            return _applicationDbContext.vUsers.Find(idUser).IdAgency;
        }
        #endregion

        #region GetAgencyName
        /// <summary>
        /// Récupere le nom de l'agence
        /// </summary>
        /// <param name="idUser">Id de l'utilisateur</param>
        /// <returns>Nom de l'agence</returns>
        public string GetAgencyName(string idUser)
        {
            return _applicationDbContext.vUsers.Find(idUser).AgencyName;
        }
        #endregion

        #region FileDelete
        /// <summary>
        /// Suppression d'un logo de l'utilisateur
        /// </summary>
        /// <param name="idUser">Identifiant du langage</param>
        /// <returns>Identifiant du questionnaire du langage, null sinon</returns>
        public bool FileDelete(string idUser)
        {
            bool fileIsDeleted = false;
            try
            {
                string path = null;
                string filename = null;

                ApplicationUser user = _applicationDbContext.Users.Find(idUser);
                if (user != null)
                {
                    path = AppSettings.FolderLogos;
                    filename = user.LogoName;
                    user.LogoName = null;
                    fileIsDeleted = true;
                }

                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(filename))
                {
                    _applicationDbContext.SaveChanges();
                    FileManager.FileDelete(path, (idUser + filename)); // Suppression des fichiers physiques
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.User, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete Logo File Id : " + idUser.ToString());
                }
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.User, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete logo File Id : " + idUser.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
            return fileIsDeleted;
        }
        #endregion

        #region ResetPassword
        /// <summary>
        /// Attribue un nouveau mot de passe à un utilisateur
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <returns>Vrai si les actions ont été exécutées, faux sinon</returns>
        public async Task<bool> ResetPassword(string id)
        {
            bool isReset = false;

            try
            {
                ApplicationUser applicationUser = _applicationDbContext.Users.Find(id);

                if (applicationUser != null)
                {
                    // MAJ du user
                    string password = UserHelper.CreateRandomPassword(AppSettings.RequireDigit, AppSettings.RequireLowercase, AppSettings.RequireNonLetterOrDigit, AppSettings.RequireUppercase, AppSettings.RequiredLength);
                    applicationUser.PasswordHash = UserHelper.CalculateHash(password);
                    applicationUser.PasswordChange = true;
                    applicationUser.ModificationDate = DateTime.Now;
                    applicationUser.Editor = HttpContext.Current.User.Identity.Name;
                    _applicationDbContext.SaveChanges();
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.User, LogManager.LogAction.Reset, HttpContext.Current.User.Identity.Name, "Reset Password User Id : " + id);

                    // Envoi du mail
                    using (Storage.Message message = new Storage.Message(Path.Combine(AppSettings.FolderMail, AppSettings.MailResetPassword)))
                    {
                        List<Recipient> recipients = new List<Recipient>
                        {
                            new Recipient("", applicationUser.Email)
                        };

                        await MailServer.Send(new Mail()
                        {
                            Body = message.BodyHtml.Replace(AppSettings.TagLastName, applicationUser.LastName).Replace(AppSettings.TagFirstName, applicationUser.FirstName).Replace(AppSettings.TagPassword, password),
                            From = AppSettings.AddressNoReply,
                            Recipients = recipients,
                            Subject = message.Subject
                        });
                    }

                    isReset = true;
                }
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.User, LogManager.LogAction.Reset, HttpContext.Current.User.Identity.Name, "Reset Password User Id : " + id + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }

            return isReset;
        }
        #endregion

        #region IsValidUserName
        /// <summary>
        /// Retourne un indicateur d'utilisation de l'ordre
        /// </summary>
        /// <param name="userNameValue">Valeur du nom d'utilisateur à tester</param>
        /// <param name="forceUserId">Force la désactivation du test sur l'id de l'utilisateur</param>
        /// <returns>Vrai si l'utilisateur n'existe pas, faux sinon</returns>
        public bool IsValidUserName(string userNameValue, string forceUserId = null)
        {
            int result = 0;

            if (!string.IsNullOrEmpty(forceUserId))
            {
                result = (from u in _applicationDbContext.Users where u.UserName.Equals(userNameValue) && !u.Id.Equals(forceUserId) select u.Id).Count();
            }
            else
            {
                result = (from u in _applicationDbContext.Users where u.UserName.Equals(userNameValue) select u.Id).Count();
            }

            return result <= 0;
        }
        #endregion

        #region GetLogoName
        public string GetLogoName(int idAgency)
        {
            vUser vUser = _applicationDbContext.vUsers.Where(u => u.IdAgency.Equals(idAgency))
                                        .Where(u => !string.IsNullOrEmpty(u.LogoName))
                                        .FirstOrDefault();
            return string.Concat(vUser?.Id,vUser?.LogoName);
        }
        #endregion

        #region Private

        #region SaveUserLogo
        /// <summary>
        /// Enregistrement des logos commun entre _Create et _Edit
        /// </summary>
        /// <param name="User">Utilisateur concerné</param>
        /// <param name="logo">Liste des logo a enregistré</param>
        private void _SaveUserLogos(ref ApplicationUser user, string filename)
        {
            string destinationPath = null;
            string sourceFile = HttpContext.Current.User.Identity.Name;
            string originName = null;

            if (!string.IsNullOrEmpty(filename))
            {
                sourceFile = sourceFile + "_LogoName" + filename;
                destinationPath = AppSettings.FolderLogos;
                originName = user.LogoName;
                user.LogoName = filename;

                if (!string.IsNullOrEmpty(originName) && (originName != filename)) // Suppression de l'ancien fichier lors d'une modification
                {
                    FileManager.FileDelete(destinationPath, (user.Id + originName));
                }

                FileManager.FileMove(AppSettings.FolderTemp, sourceFile, destinationPath, (user.Id + filename), true);
            }
        }
        #endregion

        #region DeleteOldLogos
        /// <summary>
        /// Supprime les logos en cas d'erreur
        /// </summary>
        /// <param name="user">utilisateur en erreur</param>
        /// <param name="model">model de donnée en cours</param>
        /// <param name="deleteAll">Indicateur de suppression des fichier déplacer</param>
        private void DeleteOldLogos(ApplicationUser user, UserViewModel model, bool deleteAll = false)
        {
            string CurrentUser = HttpContext.Current.User.Identity.Name;
            FileManager.FileDelete(AppSettings.FolderTemp, (CurrentUser + "_LogoName" + model.LogoName), true);

            if (deleteAll)
            {
                FileManager.FileDelete(AppSettings.FolderTemp, (CurrentUser + "_LogoName" + model.LogoName));
            }
        }
        #endregion

        #region DeleteUserShips
        /// <summary>
        /// Supprime les users dans la table AspNetUserShips
        /// </summary>
        /// <param name="IdUser">id de l'utilisateur que l'on souhaite supprimer </param>
        public void DeleteUserShips(string idUser)
        {
            try
            {
                using (AuthContext db = new AuthContext())
                {
                    List<AspNetUserShips> aspNetUserShips = db.AspNetUserShips.Where(anus => anus.IdUser.Equals(idUser)).ToList();
                    if (aspNetUserShips.Any())
                    {
                        db.AspNetUserShips.RemoveRange(aspNetUserShips);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.User, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, string.Concat("Error when we want delete the user ", idUser, " : ", ex.Message));
                throw;
            }
        }
        #endregion

        #endregion
    }
    #endregion
}
