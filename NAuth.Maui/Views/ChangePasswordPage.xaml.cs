using NAuth.Maui.ViewModels;

namespace NAuth.Maui.Views
{
    public partial class ChangePasswordPage : ContentPage
    {
        public ChangePasswordPage(ChangePasswordViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
