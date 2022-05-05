using MahApps.Metro.Controls;
using Ponant.Medical.Board.ViewModel;

namespace Ponant.Medical.Board.View
{
    /// <summary>
    /// Logique d'interaction pour UpdateCommentView.xaml
    /// </summary>
    public partial class UpdateCommentView : MetroWindow
    {
        public UpdateCommentView(UpdateCommentViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
