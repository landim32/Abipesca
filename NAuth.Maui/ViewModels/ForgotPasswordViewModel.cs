using NAuth.Maui.Services;
using System.Windows.Input;

namespace NAuth.Maui.ViewModels
{
    public class ForgotPasswordViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private string _email = string.Empty;

        public ForgotPasswordViewModel(IUserService userService)
        {
            _userService = userService;
            SendRecoveryEmailCommand = new Command(async () => await SendRecoveryEmailAsync());
            NavigateToLoginCommand = new Command(async () => await NavigateToLoginAsync());
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public ICommand SendRecoveryEmailCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        private async Task SendRecoveryEmailAsync()
        {
            await ExecuteAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
                {
                    await ShowErrorAsync("E-mail inválido");
                    return;
                }

                var success = await _userService.SendPasswordRecoveryAsync(Email);
                
                if (success)
                {
                    await ShowSuccessAsync("E-mail de recuperação enviado com sucesso! Verifique sua caixa de entrada.");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
            });
        }

        private async Task NavigateToLoginAsync()
        {
            await Shell.Current.GoToAsync("//LoginPage");
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
