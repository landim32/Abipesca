using Microsoft.Extensions.Options;
using NNews.ACL;
using NNews.DTO;
using NNews.DTO.Settings;
using NNews.Maui.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NNews.Maui.ViewModels
{
    public class ArticleListViewModel : BaseViewModel
    {
        private readonly ArticleClient _articleClient;
        private readonly INavigationService _navigationService;
        private readonly string _apiUrl;
        private int _currentPage = 1;
        private const int PageSize = 10;
        private bool _hasMoreItems = true;
        private long? _selectedCategoryId;
        private string? _selectedTagSlug;
        private string _title = "Artigos";

        public ObservableCollection<ArticleInfo> Articles { get; } = new();

        private bool _isLoadingMore;
        public bool IsLoadingMore
        {
            get => _isLoadingMore;
            set => SetProperty(ref _isLoadingMore, value);
        }

        public bool HasMoreItems
        {
            get => _hasMoreItems;
            set => SetProperty(ref _hasMoreItems, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public ICommand LoadArticlesCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand LoadMoreCommand { get; }
        public ICommand ArticleSelectedCommand { get; }

        public ArticleListViewModel(
            ArticleClient articleClient,
            INavigationService navigationService,
            IOptions<NNewsSetting> settings)
        {
            _articleClient = articleClient;
            _navigationService = navigationService;
            _apiUrl = settings.Value.ApiUrl;

            LoadArticlesCommand = new Command(async () => await LoadArticlesAsync());
            RefreshCommand = new Command(async () => await RefreshArticlesAsync());
            LoadMoreCommand = new Command(async () => await LoadMoreArticlesAsync());
            ArticleSelectedCommand = new Command<ArticleInfo>(async (article) => await OnArticleSelected(article));
        }

        public async Task InitializeAsync(long? categoryId = null, string? categoryTitle = null, string? tagSlug = null)
        {
            _selectedCategoryId = categoryId;
            _selectedTagSlug = tagSlug;
            
            if (!string.IsNullOrEmpty(tagSlug))
            {
                Title = $"Tag: {tagSlug}";
            }
            else if (!string.IsNullOrEmpty(categoryTitle))
            {
                Title = categoryTitle;
            }
            else if (categoryId.HasValue)
            {
                Title = "Artigos da Categoria";
            }
            else
            {
                Title = "Artigos";
            }

            await LoadArticlesAsync();
        }

        private async Task LoadArticlesAsync()
        {
            await ExecuteAsync(async () =>
            {
                _currentPage = 1;
                HasMoreItems = true;

                PagedResult<ArticleInfo> result;
                
                if (!string.IsNullOrEmpty(_selectedTagSlug))
                {
                    result = await _articleClient.ListByTagAsync(
                        _selectedTagSlug,
                        _currentPage,
                        PageSize);
                }
                else if (_selectedCategoryId.HasValue && _selectedCategoryId.Value > 0)
                {
                    result = await _articleClient.ListByCategoryAsync(
                        _selectedCategoryId.Value,
                        _currentPage,
                        PageSize);
                }
                else
                {
                    result = await _articleClient.ListByRolesAsync(
                        _currentPage,
                        PageSize);
                }

                Articles.Clear();
                foreach (var article in result.Items)
                {
                    Articles.Add(article);
                }

                HasMoreItems = result.HasNext;
            });
        }

        private async Task RefreshArticlesAsync()
        {
            IsRefreshing = true;
            try
            {
                _currentPage = 1;
                HasMoreItems = true;

                PagedResult<ArticleInfo> result;
                
                if (!string.IsNullOrEmpty(_selectedTagSlug))
                {
                    result = await _articleClient.ListByTagAsync(
                        _selectedTagSlug,
                        _currentPage,
                        PageSize);
                }
                else if (_selectedCategoryId.HasValue && _selectedCategoryId.Value > 0)
                {
                    result = await _articleClient.ListByCategoryAsync(
                        _selectedCategoryId.Value,
                        _currentPage,
                        PageSize);
                }
                else
                {
                    result = await _articleClient.ListByRolesAsync(
                        _currentPage,
                        PageSize);
                }

                Articles.Clear();
                foreach (var article in result.Items)
                {
                    Articles.Add(article);
                }

                HasMoreItems = result.HasNext;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing: {ex.Message}");
                await ShowErrorAsync("Erro ao atualizar artigos");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task LoadMoreArticlesAsync()
        {
            if (!HasMoreItems || IsLoadingMore || IsBusy)
                return;

            IsLoadingMore = true;
            try
            {
                _currentPage++;
                
                PagedResult<ArticleInfo> result;
                
                if (!string.IsNullOrEmpty(_selectedTagSlug))
                {
                    result = await _articleClient.ListByTagAsync(
                        _selectedTagSlug,
                        _currentPage,
                        PageSize);
                }
                else if (_selectedCategoryId.HasValue && _selectedCategoryId.Value > 0)
                {
                    result = await _articleClient.ListByCategoryAsync(
                        _selectedCategoryId.Value,
                        _currentPage,
                        PageSize);
                }
                else
                {
                    result = await _articleClient.ListByRolesAsync(
                        _currentPage,
                        PageSize);
                }

                foreach (var article in result.Items)
                {
                    Articles.Add(article);
                }

                HasMoreItems = result.HasNext;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading more: {ex.Message}");
                _currentPage--;
            }
            finally
            {
                IsLoadingMore = false;
            }
        }

        private async Task OnArticleSelected(ArticleInfo article)
        {
            if (article == null)
                return;

            var parameters = new Dictionary<string, object>
            {
                { "ArticleId", article.ArticleId }
            };

            await _navigationService.NavigateToAsync("articledetail", parameters);
        }

        public string GetImageUrl(string? imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
                return "dotnet_bot.png";

            if (imageName.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                imageName.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return imageName;
            }

            return $"{_apiUrl?.TrimEnd('/')}/images/{imageName}";
        }
    }
}
