using NAuth.Maui.Views;
using NAuth.Maui.Services;
using NNews.Maui.Views;

namespace Abipesca.Main
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register NAuth routes for navigation
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("ForgotPasswordPage", typeof(ForgotPasswordPage));
            Routing.RegisterRoute("ResetPasswordPage", typeof(ResetPasswordPage));
            Routing.RegisterRoute("MainPage", typeof(NAuth.Maui.Views.MainPage));
            Routing.RegisterRoute("ProfilePage", typeof(ProfilePage));
            Routing.RegisterRoute("ChangePasswordPage", typeof(ChangePasswordPage));

            // Register NNews routes for navigation
            Routing.RegisterRoute("articlelist", typeof(ArticleListPage));
            Routing.RegisterRoute("articledetail", typeof(ArticleDetailPage));
            Routing.RegisterRoute("categories", typeof(CategoryListPage));
            Routing.RegisterRoute("tags", typeof(TagListPage));
            Routing.RegisterRoute("search", typeof(SearchPage));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            try
            {
                var authService = Handler?.MauiContext?.Services?.GetService<IAuthService>();
                if (authService != null)
                {
                    await authService.LogoutAsync();
                }

                // Esconder o flyout menu após logout
                Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;

                // Navegar para a página de login
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during logout: {ex.Message}");
                await DisplayAlert("Erro", "Erro ao fazer logout", "OK");
            }
        }
    }
}
