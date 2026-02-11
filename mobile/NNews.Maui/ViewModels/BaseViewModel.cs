using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NNews.Maui.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        private bool _isRefreshing;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected async Task ExecuteAsync(Func<Task> operation, bool showLoading = true)
        {
            if (IsBusy && showLoading) return;

            try
            {
                if (showLoading)
                    IsBusy = true;
                    
                await operation();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                await ShowErrorAsync(ex.Message);
            }
            finally
            {
                if (showLoading)
                    IsBusy = false;
            }
        }

        protected virtual Task ShowErrorAsync(string message)
        {
            return Application.Current?.MainPage?.DisplayAlert("Erro", message, "OK") ?? Task.CompletedTask;
        }

        protected virtual Task ShowSuccessAsync(string message)
        {
            return Application.Current?.MainPage?.DisplayAlert("Sucesso", message, "OK") ?? Task.CompletedTask;
        }

        protected virtual Task<bool> ShowConfirmAsync(string title, string message, string accept = "Sim", string cancel = "Não")
        {
            return Application.Current?.MainPage?.DisplayAlert(title, message, accept, cancel) ?? Task.FromResult(false);
        }
    }
}
