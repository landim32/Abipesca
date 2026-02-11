using NAuth.Maui.Services;

namespace Abipesca.Main
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await CheckAuthenticationAsync();
        }

        private async Task CheckAuthenticationAsync()
        {
            try
            {
                var authService = Handler?.MauiContext?.Services?.GetService<IAuthService>();
                if (authService != null)
                {
                    var isAuthenticated = await authService.IsAuthenticatedAsync();
                    if (isAuthenticated)
                    {
                        // Se já estiver autenticado, habilita o menu e vai para a lista de artigos
                        Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
                        await Shell.Current.GoToAsync("//articlelist");
                    }
                    else
                    {
                        // Se não estiver autenticado, desabilita o menu e vai para o login
                        Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
                        await Shell.Current.GoToAsync("//LoginPage");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking authentication: {ex.Message}");
                Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}
