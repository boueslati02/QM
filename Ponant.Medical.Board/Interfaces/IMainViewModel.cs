using System.Threading.Tasks;

namespace Ponant.Medical.Board.Interfaces
{
    /// <summary>
    /// Interface de la page principale
    /// </summary>
    public interface IMainViewModel
    {
        /// <summary>
        /// Retourne/Positionne la page active dans la vue des questionnaires à traiter
        /// </summary>
        IBasePageViewModel ActivePage { get; set; }
        /// <summary>
        /// Retourne/Positionne l'état d'activité d'une tâche de fond. 
        /// Permet l'affichage de l'anneau de progression et d'interdire la fermeture de l'application si au moins une tâche est en cours.
        /// </summary>
        bool IsBackgroundOperationActive { get; set; }
        /// <summary>
        /// Affiche un message de demande de confirmation lorsque l'on ferme l'application 
        /// et qu'il y a des tâches de fond en cours
        /// </summary>
        /// <returns></returns>
        Task<bool> ConfirmClosing();
    }
}
