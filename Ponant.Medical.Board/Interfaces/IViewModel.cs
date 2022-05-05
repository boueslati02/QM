using System;

namespace Ponant.Medical.Board.Interfaces
{
    public interface IViewModel
    {
        Action CloseAction { get; set; }
    }
}
