namespace Ponant.Medical.Board.Interfaces
{
    public interface IView
    {
        IViewModel ViewModel { get; set; }
        
        void Show();
    }
}
