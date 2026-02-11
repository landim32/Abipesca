using NAuth.Maui.Services;
using System.Windows.Input;

namespace NAuth.Maui.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private string _email = string.Empty;
        private string _password = string.Empty;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new Command(async () => await LoginAsync());
            NavigateToRegisterCommand = new Command(async () => await NavigateToRegisterAsync());
            NavigateToForgotPasswordCommand = new Command(async () => await NavigateToForgotPasswordAsync());
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }
        public ICommand NavigateToForgotPasswordCommand { get; }

        private async Task LoginAsync()
        {
            await ExecuteAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    await ShowErrorAsync("Preencha todos os campos");
                    return;
                }

                if (!IsValidEmail(Email))
                {
                    await ShowErrorAsync("E-mail inválido");
                    return;
                }

                var user = await _authService.LoginAsync(Email, Password);
                if (user != null)
                {
                    // Habilitar o menu flyout após login bem-sucedido
                    Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
                    
                    // Navegar para a lista de artigos após login bem-sucedido
                    await Shell.Current.GoToAsync("//articlelist");
                }
            });
        }

        private async Task NavigateToRegisterAsync()
        {
            await Shell.Current.GoToAsync("RegisterPage");
        }

        private async Task NavigateToForgotPasswordAsync()
        {
            await Shell.Current.GoToAsync("ForgotPasswordPage");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
