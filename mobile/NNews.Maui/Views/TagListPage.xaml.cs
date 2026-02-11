using NNews.Maui.ViewModels;

namespace NNews.Maui.Views
{
    public partial class TagListPage : ContentPage
    {
        private readonly TagListViewModel _viewModel;

        public TagListPage(TagListViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }
    }
}
