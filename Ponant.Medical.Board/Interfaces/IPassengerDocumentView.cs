namespace Ponant.Medical.Board.Interfaces
{
    /// <summary>
    /// Interface de la vue des document du passager
    /// </summary>
    public interface IPassengerDocumentView
    {
        int Id { get; set; }
        int IdCruise { get; set; }
        string LastName { get; set; }
        string UsualName { get; set; }
        string FirstName { get; set; }
        string Email { get; set; }
    }
}