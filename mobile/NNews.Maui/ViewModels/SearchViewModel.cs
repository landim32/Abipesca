using NNews.ACL;
using NNews.DTO;
using NNews.Maui.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NNews.Maui.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private readonly ArticleClient _articleClient;
        private readonly INavigationService _navigationService;
        private CancellationTokenSource? _searchCts;

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _ = PerformSearchWithDebounceAsync();
                }
            }
        }

        private bool _isSearching;
        public bool IsSearching
        {
            get => _isSearching;
            set => SetProperty(ref _isSearching, value);
        }

        private bool _hasResults;
        public bool HasResults
        {
            get => _hasResults;
            set => SetProperty(ref _hasResults, value);
        }

        public ObservableCollection<ArticleInfo> SearchResults { get; } = new();
        public ObservableCollection<string> RecentSearches { get; } = new();

        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand ArticleSelectedCommand { get; }
        public ICommand RecentSearchSelectedCommand { get; }

        public SearchViewModel(
            ArticleClient articleClient,
            INavigationService navigationService)
        {
            _articleClient = articleClient;
            _navigationService = navigationService;

            SearchCommand = new Command<string>(async (query) => await SearchArticlesAsync(query));
            ClearSearchCommand = new Command(ClearSearch);
            ArticleSelectedCommand = new Command<ArticleInfo>(async (article) => await OnArticleSelected(article));
            RecentSearchSelectedCommand = new Command<string>((query) => SearchText = query);

            LoadRecentSearches();
        }

        private async Task PerformSearchWithDebounceAsync()
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();

            try
            {
                await Task.Delay(500, _searchCts.Token);
                await SearchArticlesAsync(SearchText);
            }
            catch (TaskCanceledException)
            {
                // Debounce canceled, do nothing
            }
        }

        private async Task SearchArticlesAsync(string? searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                SearchResults.Clear();
                HasResults = false;
                return;
            }

            IsSearching = true;
            try
            {
                // Use SearchAsync to search articles by keyword
                var result = await _articleClient.SearchAsync(
                    keyword: searchText,
                    page: 1,
                    pageSize: 50);

                SearchResults.Clear();
                foreach (var article in result.Items)
                {
                    SearchResults.Add(article);
                }

                HasResults = SearchResults.Any();
                
                if (HasResults)
                {
                    SaveRecentSearch(searchText);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching: {ex.Message}");
                await ShowErrorAsync("Erro ao buscar artigos");
            }
            finally
            {
                IsSearching = false;
            }
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;
            SearchResults.Clear();
            HasResults = false;
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

        private void SaveRecentSearch(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return;

            if (RecentSearches.Contains(searchText))
            {
                RecentSearches.Remove(searchText);
            }

            RecentSearches.Insert(0, searchText);

            if (RecentSearches.Count > 10)
            {
                RecentSearches.RemoveAt(RecentSearches.Count - 1);
            }

            // Save to preferences
            Preferences.Default.Set("RecentSearches", string.Join("|", RecentSearches));
        }

        private void LoadRecentSearches()
        {
            var saved = Preferences.Default.Get("RecentSearches", string.Empty);
            if (!string.IsNullOrWhiteSpace(saved))
            {
                var searches = saved.Split('|');
                foreach (var search in searches)
                {
                    RecentSearches.Add(search);
                }
            }
        }
    }
}
