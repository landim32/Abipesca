using NAuth.Maui.Views;

namespace Abipesca.Main
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("ForgotPasswordPage", typeof(ForgotPasswordPage));
            Routing.RegisterRoute("ResetPasswordPage", typeof(ResetPasswordPage));
            Routing.RegisterRoute("MainPage", typeof(NAuth.Maui.Views.MainPage));
            Routing.RegisterRoute("ProfilePage", typeof(ProfilePage));
            Routing.RegisterRoute("ChangePasswordPage", typeof(ChangePasswordPage));
        }
    }
}
