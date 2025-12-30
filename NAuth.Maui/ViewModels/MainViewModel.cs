using NAuth.DTO;
using NAuth.DTO.User;
using NAuth.Maui.Services;
using System.Windows.Input;

namespace NAuth.Maui.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private UserInfo? _currentUser;
        private string _userName = string.Empty;
        private string _userEmail = string.Empty;
        private string _userImageUrl = string.Empty;

        public MainViewModel(IAuthService authService)
        {
            _authService = authService;
            NavigateToProfileCommand = new Command(async () => await NavigateToProfileAsync());
            NavigateToChangePasswordCommand = new Command(async () => await NavigateToChangePasswordAsync());
            LogoutCommand = new Command(async () => await LogoutAsync());
        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string UserEmail
        {
            get => _userEmail;
            set => SetProperty(ref _userEmail, value);
        }

        public string UserImageUrl
        {
            get => _userImageUrl;
            set => SetProperty(ref _userImageUrl, value);
        }

        public ICommand NavigateToProfileCommand { get; }
        public ICommand NavigateToChangePasswordCommand { get; }
        public ICommand LogoutCommand { get; }

        public async Task LoadUserDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                _currentUser = await _authService.GetCurrentUserAsync();
                if (_currentUser is not null)
                {
                    UserName = _currentUser.Name ?? "Usuário";
                    UserEmail = _currentUser.Email ?? string.Empty;
                    UserImageUrl = _currentUser.ImageUrl ?? string.Empty;
                }
            });
        }

        private async Task NavigateToProfileAsync()
        {
            await Shell.Current.GoToAsync("ProfilePage");
        }

        private async Task NavigateToChangePasswordAsync()
        {
            await Shell.Current.GoToAsync("ChangePasswordPage");
        }

        private async Task LogoutAsync()
        {
            bool confirm = false;
            if (Application.Current?.MainPage != null)
            {
                confirm = await Application.Current.MainPage.DisplayAlert(
                    "Logout",
                    "Deseja realmente sair?",
                    "Sim",
                    "Não");
            }

            if (confirm)
            {
                await _authService.LogoutAsync();
                
                // Clear navigation stack and return to login
                await Shell.Current.Navigation.PopToRootAsync();
            }
        }
    }
}
