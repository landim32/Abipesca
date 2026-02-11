using NNews.ACL;
using NNews.DTO;
using NNews.Maui.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NNews.Maui.ViewModels
{
    public class TagListViewModel : BaseViewModel
    {
        private readonly TagClient _tagClient;
        private readonly INavigationService _navigationService;

        public ObservableCollection<TagInfo> Tags { get; } = new();

        public ICommand LoadTagsCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand TagSelectedCommand { get; }

        public TagListViewModel(
            TagClient tagClient,
            INavigationService navigationService)
        {
            _tagClient = tagClient;
            _navigationService = navigationService;

            LoadTagsCommand = new Command(async () => await LoadTagsAsync());
            RefreshCommand = new Command(async () => await RefreshTagsAsync());
            TagSelectedCommand = new Command<TagInfo>(async (tag) => await OnTagSelected(tag));
        }

        public async Task InitializeAsync()
        {
            await LoadTagsAsync();
        }

        private async Task LoadTagsAsync()
        {
            await ExecuteAsync(async () =>
            {
                // Use ListByRolesAsync for public view filtered by user roles
                var tags = await _tagClient.ListByRolesAsync();

                Tags.Clear();
                foreach (var tag in tags.OrderByDescending(t => t.ArticleCount))
                {
                    Tags.Add(tag);
                }
            });
        }

        private async Task RefreshTagsAsync()
        {
            IsRefreshing = true;
            try
            {
                var tags = await _tagClient.ListByRolesAsync();

                Tags.Clear();
                foreach (var tag in tags.OrderByDescending(t => t.ArticleCount))
                {
                    Tags.Add(tag);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing: {ex.Message}");
                await ShowErrorAsync("Erro ao atualizar tags");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task OnTagSelected(TagInfo tag)
        {
            if (tag == null)
                return;

            var parameters = new Dictionary<string, object>
            {
                { "TagSlug", tag.Slug ?? tag.Title.ToLower() }
            };

            await _navigationService.NavigateToAsync("articlelist", parameters);
        }
    }
}
