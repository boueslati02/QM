using MahApps.Metro.Controls;
using Ponant.Medical.Board.ViewModel;

namespace Ponant.Medical.Board.View
{
    /// <summary>
    /// Logique d'interaction pour Documents.xaml
    /// </summary>
    public partial class DocumentsView : MetroWindow
    {
        public DocumentsView(DocumentsViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
