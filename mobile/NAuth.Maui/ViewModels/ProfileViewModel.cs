using NAuth.DTO;
using NAuth.DTO.User;
using NAuth.Maui.Services;
using System.Windows.Input;

namespace NAuth.Maui.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private UserInfo? _currentUser;
        private string _name = string.Empty;
        private string _email = string.Empty;
        private DateTime? _birthDate;
        private string _idDocument = string.Empty;
        private string _imageUrl = string.Empty;

        public ProfileViewModel(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
            SaveCommand = new Command(async () => await SaveAsync());
            UploadImageCommand = new Command(async () => await UploadImageAsync());
            NavigateBackCommand = new Command(async () => await NavigateBackAsync());
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

        public string ImageUrl
        {
            get => _imageUrl;
            set => SetProperty(ref _imageUrl, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand UploadImageCommand { get; }
        public ICommand NavigateBackCommand { get; }

        public async Task LoadUserDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                _currentUser = await _authService.GetCurrentUserAsync();
                if (_currentUser is not null)
                {
                    Name = _currentUser.Name ?? string.Empty;
                    Email = _currentUser.Email ?? string.Empty;
                    BirthDate = _currentUser.BirthDate;
                    IdDocument = _currentUser.IdDocument ?? string.Empty;
                    ImageUrl = _currentUser.ImageUrl ?? string.Empty;
                }
            });
        }

        private async Task SaveAsync()
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

                if (_currentUser is null)
                {
                    await ShowErrorAsync("Usuário não encontrado");
                    return;
                }

                _currentUser.Name = Name;
                _currentUser.Email = Email;
                _currentUser.BirthDate = BirthDate;
                _currentUser.IdDocument = IdDocument;

                var updatedUser = await _userService.UpdateProfileAsync(_currentUser);
                
                if (updatedUser != null)
                {
                    await ShowSuccessAsync("Perfil atualizado com sucesso!");
                }
            });
        }

        private async Task UploadImageAsync()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Selecione uma foto"
                });

                if (result != null)
                {
                    await ExecuteAsync(async () =>
                    {
                        using var stream = await result.OpenReadAsync();
                        var imageUrl = await _userService.UploadProfileImageAsync(stream, result.FileName);
                        
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            ImageUrl = imageUrl;
                            await ShowSuccessAsync("Foto atualizada com sucesso!");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Erro ao selecionar imagem: {ex.Message}");
            }
        }

        private async Task NavigateBackAsync()
        {
            await Shell.Current.GoToAsync("..");
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
