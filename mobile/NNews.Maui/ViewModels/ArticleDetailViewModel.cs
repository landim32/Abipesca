using Microsoft.Extensions.Options;
using NNews.ACL;
using NNews.DTO;
using NNews.DTO.Settings;
using NNews.Maui.Services;
using System.Windows.Input;

namespace NNews.Maui.ViewModels
{
    [QueryProperty(nameof(ArticleId), "ArticleId")]
    public class ArticleDetailViewModel : BaseViewModel
    {
        private readonly ArticleClient _articleClient;
        private readonly INavigationService _navigationService;
        private readonly string _apiUrl;
        
        private long _articleId;
        public long ArticleId
        {
            get => _articleId;
            set
            {
                SetProperty(ref _articleId, value);
                LoadArticleCommand.Execute(null);
            }
        }

        private ArticleInfo? _article;
        public ArticleInfo? Article
        {
            get => _article;
            set => SetProperty(ref _article, value);
        }

        private string _formattedContent = string.Empty;
        public string FormattedContent
        {
            get => _formattedContent;
            set => SetProperty(ref _formattedContent, value);
        }

        private string _htmlContent = string.Empty;
        public string HtmlContent
        {
            get => _htmlContent;
            set => SetProperty(ref _htmlContent, value);
        }

        public ICommand LoadArticleCommand { get; }
        public ICommand ShareCommand { get; }
        public ICommand TagSelectedCommand { get; }
        public ICommand CategorySelectedCommand { get; }
        public ICommand BackCommand { get; }

        public ArticleDetailViewModel(
            ArticleClient articleClient,
            INavigationService navigationService,
            IOptions<NNewsSetting> settings)
        {
            _articleClient = articleClient;
            _navigationService = navigationService;
            _apiUrl = settings.Value.ApiUrl;

            LoadArticleCommand = new Command(async () => await LoadArticleAsync());
            ShareCommand = new Command(async () => await ShareArticleAsync());
            TagSelectedCommand = new Command<TagInfo>(async (tag) => await OnTagSelected(tag));
            CategorySelectedCommand = new Command(async () => await OnCategorySelected());
            BackCommand = new Command(async () => await _navigationService.GoBackAsync());
        }

        private async Task LoadArticleAsync()
        {
            await ExecuteAsync(async () =>
            {
                Article = await _articleClient.GetByIdAsync((int)ArticleId);
                
                if (Article != null)
                {
                    FormattedContent = FormatContent(Article.Content);
                    HtmlContent = FormatHtmlContent(Article.Content);
                }
            });
        }

        private string FormatContent(string content)
        {
            // Remove HTML tags for simple text display
            var text = System.Text.RegularExpressions.Regex.Replace(content, "<.*?>", string.Empty);
            return System.Net.WebUtility.HtmlDecode(text);
        }

        private string FormatHtmlContent(string content)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no'>
    <style>
        * {{
            box-sizing: border-box;
        }}
        html, body {{
            margin: 0;
            padding: 0;
            overflow: hidden;
            width: 100%;
            height: auto;
        }}
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            font-size: 16px;
            line-height: 1.6;
            color: #212529;
            padding: 0;
            background-color: white;
            word-wrap: break-word;
            overflow-wrap: break-word;
        }}
        img {{
            max-width: 100% !important;
            height: auto !important;
            display: block;
            margin: 16px 0;
            object-fit: contain;
        }}
        p {{
            margin: 0 0 16px 0;
            word-wrap: break-word;
            overflow-wrap: break-word;
        }}
        h1, h2, h3, h4, h5, h6 {{
            margin: 24px 0 16px 0;
            font-weight: bold;
            line-height: 1.25;
            word-wrap: break-word;
            overflow-wrap: break-word;
        }}
        h1 {{ font-size: 2em; }}
        h2 {{ font-size: 1.5em; }}
        h3 {{ font-size: 1.25em; }}
        ul, ol {{
            margin: 0 0 16px 0;
            padding-left: 24px;
        }}
        li {{
            margin: 4px 0;
            word-wrap: break-word;
            overflow-wrap: break-word;
        }}
        a {{
            color: #0066CC;
            text-decoration: none;
            word-wrap: break-word;
            overflow-wrap: break-word;
        }}
        blockquote {{
            margin: 16px 0;
            padding: 8px 16px;
            border-left: 4px solid #DEE2E6;
            background-color: #F8F9FA;
            color: #6C757D;
            word-wrap: break-word;
            overflow-wrap: break-word;
        }}
        code {{
            background-color: #F8F9FA;
            padding: 2px 6px;
            border-radius: 4px;
            font-family: 'Courier New', monospace;
            font-size: 14px;
            word-wrap: break-word;
            overflow-wrap: break-word;
        }}
        pre {{
            background-color: #F8F9FA;
            padding: 12px;
            border-radius: 4px;
            margin: 16px 0;
            max-width: 100%;
            overflow: visible;
        }}
        pre code {{
            background-color: transparent;
            padding: 0;
            white-space: pre-wrap;
            word-wrap: break-word;
        }}
        table {{
            width: 100%;
            max-width: 100%;
            border-collapse: collapse;
            margin: 16px 0;
            display: table;
        }}
        th, td {{
            border: 1px solid #DEE2E6;
            padding: 8px;
            text-align: left;
            word-wrap: break-word;
            overflow-wrap: break-word;
        }}
        th {{
            background-color: #F8F9FA;
            font-weight: bold;
        }}
        iframe, video {{
            max-width: 100% !important;
            height: auto !important;
        }}
        div, section, article {{
            max-width: 100%;
            word-wrap: break-word;
            overflow-wrap: break-word;
        }}
    </style>
</head>
<body>
    {content}
</body>
</html>";
        }

        private async Task ShareArticleAsync()
        {
            if (Article == null)
                return;

            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = Article.Title,
                Text = $"{Article.Title}\n\n{FormattedContent.Substring(0, Math.Min(200, FormattedContent.Length))}..."
            });
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

        private async Task OnCategorySelected()
        {
            if (Article?.Category == null)
                return;

            var parameters = new Dictionary<string, object>
            {
                { "CategoryId", Article.Category.CategoryId },
                { "CategoryTitle", Article.Category.Title }
            };

            await _navigationService.NavigateToAsync("articlelist", parameters);
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
