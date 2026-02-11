using NNews.Maui.ViewModels;

namespace NNews.Maui.Views
{
    [QueryProperty(nameof(CategoryId), "CategoryId")]
    [QueryProperty(nameof(CategoryTitle), "CategoryTitle")]
    [QueryProperty(nameof(TagSlug), "TagSlug")]
    public partial class ArticleListPage : ContentPage
    {
        private readonly ArticleListViewModel _viewModel;

        public long CategoryId { get; set; }  
        public string? CategoryTitle { get; set; }
        public string? TagSlug { get; set; }

        public ArticleListPage(ArticleListViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync(CategoryId, CategoryTitle, TagSlug);
        }
    }
}
