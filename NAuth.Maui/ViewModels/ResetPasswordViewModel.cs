using NAuth.Maui.Services;
using System.Windows.Input;

namespace NAuth.Maui.ViewModels
{
    public class ResetPasswordViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private string _recoveryHash = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;

        public ResetPasswordViewModel(IUserService userService)
        {
            _userService = userService;
            ResetPasswordCommand = new Command(async () => await ResetPasswordAsync());
        }

        public string RecoveryHash
        {
            get => _recoveryHash;
            set => SetProperty(ref _recoveryHash, value);
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

        public ICommand ResetPasswordCommand { get; }

        private async Task ResetPasswordAsync()
        {
            await ExecuteAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(RecoveryHash))
                {
                    await ShowErrorAsync("Hash de recuperação inválido");
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 8)
                {
                    await ShowErrorAsync("Senha deve ter no mínimo 8 caracteres");
                    return;
                }

                if (!HasLettersAndNumbers(NewPassword))
                {
                    await ShowErrorAsync("Senha deve conter letras e números");
                    return;
                }

                if (NewPassword != ConfirmPassword)
                {
                    await ShowErrorAsync("As senhas não coincidem");
                    return;
                }

                var success = await _userService.ResetPasswordAsync(RecoveryHash, NewPassword);
                
                if (success)
                {
                    await ShowSuccessAsync("Senha redefinida com sucesso!");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
            });
        }

        private bool HasLettersAndNumbers(string password)
        {
            return password.Any(char.IsLetter) && password.Any(char.IsDigit);
        }
    }
}
