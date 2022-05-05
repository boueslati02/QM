using Ponant.Medical.Board.Helpers;
using Ponant.Medical.Board.Interfaces;

namespace Ponant.Medical.Board.ViewModel
{
    public abstract class BasePageViewModel : BaseViewModel, IBasePageViewModel
    {
        /// <summary>
        /// Retourne/Positionne le viewmodel à la vue principale
        /// </summary>
        public IMainViewModel MainViewModel { get; set; }
    }
}