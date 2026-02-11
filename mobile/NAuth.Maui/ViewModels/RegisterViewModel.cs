using NAuth.DTO;
using NAuth.DTO.User;
using NAuth.Maui.Services;
using System.Windows.Input;

namespace NAuth.Maui.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private DateTime? _birthDate;
        private string _idDocument = string.Empty;

        public RegisterViewModel(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
            RegisterCommand = new Command(async () => await RegisterAsync());
            NavigateToLoginCommand = new Command(async () => await NavigateToLoginAsync());
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public DateTime? BirthDate
        {
            get => _birthDate;
            set => SetProperty(ref _birthDate, value);
        }

        public string IdDocument
        {
            get => _idDocument;
            set => SetProperty(ref _idDocument, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        private async Task RegisterAsync()
        {
            await ExecuteAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(Name) || Name.Length < 3)
                {
                    await ShowErrorAsync("Nome deve ter no mínimo 3 caracteres");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
                {
                    await ShowErrorAsync("E-mail inválido");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Password) || Password.Length < 8)
                {
                    await ShowErrorAsync("Senha deve ter no mínimo 8 caracteres");
                    return;
                }

                if (!HasLettersAndNumbers(Password))
                {
                    await ShowErrorAsync("Senha deve conter letras e números");
                    return;
                }

                if (Password != ConfirmPassword)
                {
                    await ShowErrorAsync("As senhas não coincidem");
                    return;
                }

                var newUser = new UserInsertedInfo
                {
                    Name = Name,
                    Email = Email,
                    Password = Password,
                    BirthDate = BirthDate,
                    IdDocument = IdDocument
                };

                var result = await _userService.RegisterAsync(newUser);
                
                if (result != null)
                {
                    await ShowSuccessAsync("Cadastro realizado com sucesso!");
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

        private bool HasLettersAndNumbers(string password)
        {
            return password.Any(char.IsLetter) && password.Any(char.IsDigit);
        }
    }
}
