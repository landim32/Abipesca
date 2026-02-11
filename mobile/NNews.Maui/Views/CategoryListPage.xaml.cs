using NNews.Maui.ViewModels;

namespace NNews.Maui.Views
{
    public partial class CategoryListPage : ContentPage
    {
        private readonly CategoryListViewModel _viewModel;

        public CategoryListPage(CategoryListViewModel viewModel)
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
