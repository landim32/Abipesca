using NAuth.Maui.Services;
using System.Windows.Input;

namespace NAuth.Maui.ViewModels
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private string _oldPassword = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;

        public ChangePasswordViewModel(IUserService userService)
        {
            _userService = userService;
            ChangePasswordCommand = new Command(async () => await ChangePasswordAsync());
            NavigateBackCommand = new Command(async () => await NavigateBackAsync());
        }

        public string OldPassword
        {
            get => _oldPassword;
            set => SetProperty(ref _oldPassword, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public ICommand ChangePasswordCommand { get; }
        public ICommand NavigateBackCommand { get; }

        private async Task ChangePasswordAsync()
        {
            await ExecuteAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(OldPassword))
                {
                    await ShowErrorAsync("Informe a senha atual");
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 8)
                {
                    await ShowErrorAsync("Nova senha deve ter no mínimo 8 caracteres");
                    return;
                }

                if (!HasLettersAndNumbers(NewPassword))
                {
                    await ShowErrorAsync("Nova senha deve conter letras e números");
                    return;
                }

                if (NewPassword != ConfirmPassword)
                {
                    await ShowErrorAsync("As senhas não coincidem");
                    return;
                }

                var success = await _userService.ChangePasswordAsync(OldPassword, NewPassword);
                
                if (success)
                {
                    await ShowSuccessAsync("Senha alterada com sucesso!");
                    await Shell.Current.GoToAsync("..");
                }
            });
        }

        private async Task NavigateBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        private bool HasLettersAndNumbers(string password)
        {
            return password.Any(char.IsLetter) && password.Any(char.IsDigit);
        }
    }
}
