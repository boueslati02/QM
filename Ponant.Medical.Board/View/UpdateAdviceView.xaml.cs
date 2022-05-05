using MahApps.Metro.Controls;
using Ponant.Medical.Board.ViewModel;

namespace Ponant.Medical.Board.View
{
    /// <summary>
    /// Logique d'interaction pour UpdateAdviceView.xaml
    /// </summary>
    public partial class UpdateAdviceView : MetroWindow
    {
        public UpdateAdviceView(UpdateAdviceViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
