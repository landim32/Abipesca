using NNews.Maui.ViewModels;

namespace NNews.Maui.Views
{
    public partial class ArticleDetailPage : ContentPage
    {
        private readonly ArticleDetailViewModel _viewModel;

        public ArticleDetailPage(ArticleDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            if (ContentWebView != null)
            {
                ContentWebView.Navigated += OnWebViewNavigated;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            if (ContentWebView != null)
            {
                ContentWebView.Navigated -= OnWebViewNavigated;
            }
        }

        private async void OnWebViewNavigated(object? sender, WebNavigatedEventArgs e)
        {
            if (ContentWebView == null) return;

            try
            {
                var heightString = await ContentWebView.EvaluateJavaScriptAsync("document.body.scrollHeight.toString()");
                
                if (double.TryParse(heightString, out double height))
                {
                    ContentWebView.HeightRequest = height;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting WebView height: {ex.Message}");
            }
        }
    }
}
