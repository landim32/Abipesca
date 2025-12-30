using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NAuth.Maui.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
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

        protected async Task ExecuteAsync(Func<Task> operation)
        {
            if (IsBusy) return;

            try
            {
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
    }
}
