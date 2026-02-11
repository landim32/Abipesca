# ?? NNews.Maui - Quick Reference

## Navigation Commands

```csharp
// Articles
await Shell.Current.GoToAsync("articlelist");

// Articles by Category
await Shell.Current.GoToAsync("articlelist", new Dictionary<string, object> {
    { "CategoryId", categoryId },
    { "CategoryTitle", "Category Name" }
});

// Article Detail
await Shell.Current.GoToAsync("articledetail", new Dictionary<string, object> {
    { "ArticleId", articleId }
});

// Categories
await Shell.Current.GoToAsync("categories");

// Tags
await Shell.Current.GoToAsync("tags");

// Search
await Shell.Current.GoToAsync("search");
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/articles?page=1&pageSize=10` | Get paginated articles |
| GET | `/api/articles?categoryId=1` | Get articles by category |
| GET | `/api/articles/{id}` | Get article by ID |
| GET | `/api/categories` | Get all categories |
| GET | `/api/categories/{id}` | Get category by ID |
| GET | `/api/tags` | Get all tags |
| GET | `/api/tags/{id}` | Get tag by ID |

## Configuration

```json
{
  "NNews": {
    "ApiUrl": "https://10.0.2.2:5006"
  }
}
```

### Platform URLs

| Platform | URL |
|----------|-----|
| Android Emulator | `https://10.0.2.2:5006` |
| Android Device | `https://YOUR_LOCAL_IP:5006` |
| Windows | `https://localhost:5006` |

## ViewModels

| ViewModel | Purpose |
|-----------|---------|
| `ArticleListViewModel` | Article list with pagination |
| `ArticleDetailViewModel` | Single article details |
| `CategoryListViewModel` | Category grid |
| `TagListViewModel` | Tag list |
| `SearchViewModel` | Search functionality |

## Services

| Service | Interface | Purpose |
|---------|-----------|---------|
| `ArticleClientService` | `IArticleClient` | Article API calls |
| `CategoryClientService` | `ICategoryClient` | Category API calls |
| `TagClientService` | `ITagClient` | Tag API calls |
| `NavigationService` | `INavigationService` | Shell navigation |

## Key Features

- ? Infinite Scroll (RemainingItemsThreshold)
- ? Pull-to-Refresh (RefreshView)
- ? Search with Debounce (500ms)
- ? Image Caching
- ? Share Functionality
- ? Category Filter
- ? Search History

## Colors

```csharp
Primary:        #0066CC
Success:        #28A745
Danger:         #DC3545
Background:     #F8F9FA
Card:           #FFFFFF
Text:           #212529
TextSecondary:  #6C757D
```

## Troubleshooting

### SSL Certificate Issues (Android)
Add to `AndroidManifest.xml`:
```xml
<application android:networkSecurityConfig="@xml/network_security_config">
```

### Images Not Loading
1. Check API URL in `appsettings.json`
2. Verify image path: `{ApiUrl}/images/{imageName}`
3. Check network connectivity

### Navigation Not Working
1. Verify routes in `AppShell.xaml.cs`
2. Check if ViewModels/Views are registered in `MauiProgram.cs`
3. Ensure NNews.Maui project reference in Abipesca.Main

## Build & Run

```bash
# Clean
dotnet clean

# Build
dotnet build

# Run (Android)
dotnet build -t:Run -f net8.0-android

# Run (Windows)
dotnet build -t:Run -f net8.0-windows10.0.19041.0
```

## Dependencies

```xml
<PackageReference Include="Microsoft.Maui.Controls" Version="8.0.100" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
<PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
```

## Project Structure

```
NNews.Maui/
??? ViewModels/        # 6 ViewModels
??? Views/             # 5 Pages (XAML + CS)
??? Services/          # 4 Services
??? Converters/        # 4 Converters
??? Documentation/     # 4 Guides
```

## Performance Tips

1. **Images**: Use caching with `CachingEnabled="True"`
2. **Lists**: Use `CollectionView` instead of `ListView`
3. **Scroll**: Implement virtualization with `RemainingItemsThreshold`
4. **API**: Use pagination to limit data transfer
5. **Search**: Implement debounce to reduce API calls

## Common Patterns

### Loading State
```csharp
await ExecuteAsync(async () => {
    // Your async code here
    var articles = await _articleClient.GetAllAsync();
});
```

### Navigation with Parameters
```csharp
var parameters = new Dictionary<string, object> {
    { "ArticleId", articleId }
};
await _navigationService.NavigateToAsync("articledetail", parameters);
```

### Pull-to-Refresh
```xml
<RefreshView IsRefreshing="{Binding IsRefreshing}"
             Command="{Binding RefreshCommand}">
    <!-- Your content -->
</RefreshView>
```

### Infinite Scroll
```xml
<CollectionView ItemsSource="{Binding Articles}"
               RemainingItemsThreshold="2"
               RemainingItemsThresholdReachedCommand="{Binding LoadMoreCommand}">
    <!-- Your items -->
</CollectionView>
```

## Resources

- ?? [README.md](README.md) - Full documentation
- ?? [SETUP_GUIDE.md](SETUP_GUIDE.md) - Configuration guide
- ?? [INTEGRATION_GUIDE.md](INTEGRATION_GUIDE.md) - Integration examples
- ?? [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md) - Complete summary

---

**Version**: 1.0  
**Last Updated**: 2024  
**Status**: ? Production Ready
