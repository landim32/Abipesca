# Migração de GetAllAsync para FilterAsync

## Data: 2024
## Status: ? Concluído com Sucesso

## Objetivo
Atualizar os ViewModels do NNews.Maui para usar o método `FilterAsync` ao invés de `GetAllAsync`, permitindo maior flexibilidade de filtros e suporte a controle de acesso baseado em roles.

## Alterações Realizadas

### 1. ArticleListViewModel.cs

#### Mudanças Principais:
- ? Adicionada propriedade privada `_roles` para armazenar roles de filtro
- ? Atualizado método `InitializeAsync` para aceitar parâmetro opcional `roles`
- ? Substituído `GetAllAsync` por `FilterAsync` em 3 métodos:
  - `LoadArticlesAsync()`
  - `RefreshArticlesAsync()`
  - `LoadMoreArticlesAsync()`

#### Assinatura do FilterAsync (ArticleClient):
```csharp
FilterAsync(
    IList<string>? roles,      // Filtro por roles de acesso
    long? parentId,             // Filtro por categoria pai
    int page,                   // Número da página
    int pageSize                // Tamanho da página
)
```

#### Código Antes:
```csharp
var result = await _articleClient.GetAllAsync(
    _selectedCategoryId,
    _currentPage,
    PageSize);
```

#### Código Depois:
```csharp
var result = await _articleClient.FilterAsync(
    _roles,                     // Suporte a filtro por roles
    _selectedCategoryId,        // Categoria pai
    _currentPage,               // Página atual
    PageSize);                  // Tamanho da página
```

#### Nova Assinatura do InitializeAsync:
```csharp
public async Task InitializeAsync(
    long? categoryId = null, 
    string? categoryTitle = null, 
    IList<string>? roles = null)  // ?? Novo parâmetro
```

### 2. SearchViewModel.cs

#### Mudanças Principais:
- ? Substituído `GetAllAsync` por `FilterAsync` no método `SearchArticlesAsync`
- ? Mantida filtragem local por texto após buscar os artigos

#### Código Antes:
```csharp
var result = await _articleClient.GetAllAsync(page: 1, pageSize: 50);
```

#### Código Depois:
```csharp
var result = await _articleClient.FilterAsync(
    roles: null,      // Sem filtro de roles na busca
    parentId: null,   // Sem filtro de categoria
    page: 1, 
    pageSize: 50);
```

### 3. CategoryListViewModel.cs

#### Mudanças Principais:
- ? Adicionadas propriedades privadas `_roles` e `_parentId` para armazenar filtros
- ? Atualizado método `InitializeAsync` para aceitar parâmetros opcionais `roles` e `parentId`
- ? Substituído `GetAllAsync` por `FilterAsync` em 2 métodos:
  - `LoadCategoriesAsync()`
  - `RefreshCategoriesAsync()`

#### Assinatura do FilterAsync (CategoryClient):
```csharp
FilterAsync(
    IList<string>? roles,      // Filtro por roles de acesso
    long? parentId              // Filtro por categoria pai (hierarquia)
)
```

#### Código Antes:
```csharp
var categories = await _categoryClient.GetAllAsync();
```

#### Código Depois:
```csharp
var categories = await _categoryClient.FilterAsync(_roles, _parentId);
```

#### Nova Assinatura do InitializeAsync:
```csharp
public async Task InitializeAsync(
    IList<string>? roles = null,    // ?? Novo parâmetro
    long? parentId = null)          // ?? Novo parâmetro
```

## Benefícios da Mudança

### 1. **Controle de Acesso Baseado em Roles** ??
Agora é possível filtrar artigos e categorias por roles de usuário:
```csharp
// Exemplo: Mostrar apenas artigos públicos
await articleViewModel.InitializeAsync(
    categoryId: 1,
    categoryTitle: "Notícias",
    roles: new List<string> { "Public" }
);

// Exemplo: Mostrar apenas categorias públicas
await categoryViewModel.InitializeAsync(
    roles: new List<string> { "Public" }
);
```

### 2. **Hierarquia de Categorias** ??
CategoryListViewModel agora suporta filtragem por categoria pai:
```csharp
// Exemplo: Mostrar subcategorias de uma categoria pai
await categoryViewModel.InitializeAsync(
    parentId: 5  // Mostra apenas filhas da categoria 5
);

// Exemplo: Mostrar apenas categorias raiz (sem pai)
await categoryViewModel.InitializeAsync(
    parentId: null  // Mostra apenas categorias de nível superior
);
```

### 3. **Filtragem Mais Flexível** ??
Os métodos `FilterAsync` oferecem mais opções de filtro:
- Por roles (controle de acesso)
- Por categoria pai (hierarquia)
- Paginação (artigos)

### 4. **Consistência com a API** ??
Alinha com a documentação do NNews.ACL que recomenda usar `FilterAsync` para filtragem avançada.

### 5. **Preparação para Funcionalidades Futuras** ??
Facilita implementação de:
- Áreas restritas por assinatura
- Conteúdo exclusivo para membros
- Categorias premium
- Controle de visibilidade por grupos
- Navegação hierárquica de categorias

## Compatibilidade

### Retrocompatibilidade Mantida ?
As chamadas existentes continuam funcionando:

#### ArticleListViewModel
```csharp
// Ainda funciona sem especificar roles
await viewModel.InitializeAsync(categoryId: 1);
```

#### CategoryListViewModel
```csharp
// Ainda funciona sem especificar parâmetros
await viewModel.InitializeAsync();

// Ou com apenas um parâmetro
await viewModel.InitializeAsync(roles: new List<string> { "Public" });
await viewModel.InitializeAsync(parentId: 5);
```
## Exemplos de Uso

### Exemplo 1: Categorias Públicas
```csharp
var viewModel = serviceProvider.GetService<CategoryListViewModel>();
await viewModel.InitializeAsync(
    roles: new List<string> { "Public" }
);
```

### Exemplo 2: Subcategorias de uma Categoria Específica
```csharp
var viewModel = serviceProvider.GetService<CategoryListViewModel>();
await viewModel.InitializeAsync(
    parentId: 10  // Mostra subcategorias da categoria 10
);
```

### Exemplo 3: Categorias Premium com Hierarquia
```csharp
var viewModel = serviceProvider.GetService<CategoryListViewModel>();
await viewModel.InitializeAsync(
    roles: new List<string> { "Premium", "Admin" },
    parentId: 5  // Subcategorias premium da categoria 5
);
```

### Exemplo 4: Artigos de uma Categoria com Roles
```csharp
var articleViewModel = serviceProvider.GetService<ArticleListViewModel>();
await articleViewModel.InitializeAsync(
    categoryId: 3,
    categoryTitle: "Tecnologia",
    roles: new List<string> { "Member" }
);
```

### Exemplo 5: Navegação Hierárquica
```csharp
// Página principal - categorias raiz
await categoryViewModel.InitializeAsync(parentId: null);

// Usuário clica em uma categoria
// Navega para subcategorias
await categoryViewModel.InitializeAsync(parentId: selectedCategory.CategoryId);
```

## Comparação de Métodos

### CategoryClient

#### GetAllAsync (Antigo)
```csharp
// Sem parâmetros - retorna todas as categorias
await _categoryClient.GetAllAsync();
```

#### FilterAsync (Novo)
```csharp
// Parâmetros:
- roles?: Opcional - filtro por roles de acesso
- parentId?: Opcional - filtro por categoria pai

// Exemplos:
await _categoryClient.FilterAsync(
    roles: new List<string> { "Public" },
    parentId: null
);

await _categoryClient.FilterAsync(
    roles: null,
    parentId: 5
);
```

### ArticleClient

#### GetAllAsync (Antigo)
```csharp
await _articleClient.GetAllAsync(
    categoryId: 1,
    page: 1,
    pageSize: 10
);
```

#### FilterAsync (Novo)
```csharp
await _articleClient.FilterAsync(
    roles: new List<string> { "Public", "Member" },
    parentId: 1,
    page: 1,
    pageSize: 10
);
```

## Impacto nos Componentes

### Componentes Atualizados:
- ? `ArticleListViewModel.cs` - 3 métodos atualizados
- ? `SearchViewModel.cs` - 1 método atualizado
- ? `CategoryListViewModel.cs` - 2 métodos atualizados

### Componentes Não Afetados:
- ? `ArticleDetailViewModel.cs` - Usa `GetByIdAsync`
- ? `TagListViewModel.cs` - Usa `TagClient.GetAllAsync`

### Views Não Requerem Mudanças:
- ? `ArticleListPage.xaml`
- ? `SearchPage.xaml`
- ? `CategoryListPage.xaml`
- ? Todas as outras views

## Casos de Uso Avançados

### 1. Menu de Categorias Hierárquico
```csharp
// CategoryMenuViewModel.cs
public class CategoryMenuViewModel : BaseViewModel
{
    private readonly CategoryClient _categoryClient;
    private Stack<long?> _navigationStack = new();
    
    public async Task NavigateToSubcategoriesAsync(long? parentId)
    {
        _navigationStack.Push(parentId);
        await LoadCategoriesAsync(parentId);
    }
    
    public async Task NavigateBackAsync()
    {
        if (_navigationStack.Count > 1)
        {
            _navigationStack.Pop();
            var previousParentId = _navigationStack.Peek();
            await LoadCategoriesAsync(previousParentId);
        }
    }
    
    private async Task LoadCategoriesAsync(long? parentId)
    {
        var categories = await _categoryClient.FilterAsync(
            roles: await GetUserRolesAsync(),
            parentId: parentId
        );
        
        Categories.Clear();
        foreach (var category in categories)
        {
            Categories.Add(category);
        }
    }
}
```

### 2. Breadcrumb de Navegação
```csharp
public class BreadcrumbViewModel : BaseViewModel
{
    private ObservableCollection<CategoryInfo> _breadcrumb = new();
    
    public async Task LoadBreadcrumbAsync(long categoryId)
    {
        _breadcrumb.Clear();
        var currentCategory = await _categoryClient.GetByIdAsync(categoryId);
        
        while (currentCategory != null)
        {
            _breadcrumb.Insert(0, currentCategory);
            
            if (currentCategory.ParentId.HasValue)
            {
                currentCategory = await _categoryClient.GetByIdAsync(
                    (int)currentCategory.ParentId.Value
                );
            }
            else
            {
                break;
            }
        }
    }
}
```

### 3. Sidebar com Categorias e Contagem
```csharp
public class SidebarViewModel : BaseViewModel
{
    public async Task LoadCategoriesWithCountAsync()
    {
        // Carregar categorias raiz com seus artigos
        var categories = await _categoryClient.FilterAsync(
            roles: await GetUserRolesAsync(),
            parentId: null  // Apenas raiz
        );
        
        foreach (var category in categories)
        {
            // ArticleCount já vem do DTO
            category.DisplayText = $"{category.Title} ({category.ArticleCount})";
        }
        
        Categories.Clear();
        foreach (var category in categories)
        {
            Categories.Add(category);
        }
    }
}
```

## Testes Recomendados

### 1. Teste Básico - CategoryListViewModel (Sem Parâmetros)
```csharp
await viewModel.InitializeAsync();
// Deve mostrar todas as categorias
```

### 2. Teste com Roles - CategoryListViewModel
```csharp
await viewModel.InitializeAsync(
    roles: new List<string> { "Public" }
);
// Deve mostrar apenas categorias públicas
```

### 3. Teste de Hierarquia - CategoryListViewModel
```csharp
// Carregar categorias raiz
await viewModel.InitializeAsync(parentId: null);

// Navegar para subcategorias
var firstCategory = viewModel.Categories.First();
await viewModel.InitializeAsync(parentId: firstCategory.CategoryId);
// Deve mostrar apenas subcategorias
```

### 4. Teste Combinado - CategoryListViewModel
```csharp
await viewModel.InitializeAsync(
    roles: new List<string> { "Premium" },
    parentId: 10
);
// Deve mostrar subcategorias premium da categoria 10
```

### 5. Teste de Refresh - CategoryListViewModel
```csharp
await viewModel.InitializeAsync(parentId: 5);
await viewModel.RefreshCommand.Execute(null);
// Deve recarregar com os mesmos filtros
```

## Documentação da API

De acordo com a documentação do NNews.ACL v2.0.0:

### ICategoryClient.FilterAsync
```csharp
FilterAsync(roles?, parentId?, cancellationToken)
```

**Descrição**: Get filtered categories by roles and parent.

**Parâmetros**:
- `roles` (IList<string>?): Optional role names filter
- `parentId` (long?): Optional parent category ID
- `cancellationToken` (CancellationToken): Cancellation token

**Retorna**: `IList<CategoryInfo>`

### IArticleClient.FilterAsync
```csharp
FilterAsync(roles?, parentId?, page, pageSize, cancellationToken)
```

**Descrição**: Get filtered articles by roles and parent category.

**Parâmetros**:
- `roles` (IList<string>?): Optional role names filter
- `parentId` (long?): Optional parent category filter
- `page` (int): Page number (default: 1)
- `pageSize` (int): Items per page (default: 10)
- `cancellationToken` (CancellationToken): Cancellation token

**Retorna**: `PagedResult<ArticleInfo>`

## Próximos Passos Sugeridos

### 1. Implementar Menu Hierárquico de Categorias
```csharp
// CategoryMenuPage.xaml.cs
protected override async void OnAppearing()
{
    base.OnAppearing();
    
    // Carregar categorias raiz
    await _viewModel.InitializeAsync(parentId: null);
}

private async void OnCategoryTapped(CategoryInfo category)
{
    // Navegar para subcategorias
    await Navigation.PushAsync(new CategoryMenuPage
    {
        BindingContext = new CategoryListViewModel(...)
        {
            ParentId = category.CategoryId
        }
    });
}
```

### 2. Implementar Breadcrumb de Navegação
```xml
<!-- CategoryBreadcrumb.xaml -->
<CollectionView ItemsSource="{Binding Breadcrumb}"
               ItemsLayout="HorizontalList">
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <HorizontalStackLayout>
                <Label Text="{Binding Title}" />
                <Label Text=" > " />
            </HorizontalStackLayout>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

### 3. Adicionar Indicador de Subcategorias
```xml
<!-- CategoryListPage.xaml -->
<Frame>
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="Auto" />
        </Grid.ColumnDefinitions>
        
        <Label Grid.Column="0" 
               Text="{Binding Title}" />
        
        <!-- Mostrar seta se tiver subcategorias -->
        <Label Grid.Column="1"
               Text="›"
               IsVisible="{Binding HasChildren}" />
    </Grid>
</Frame>
```

### 4. Implementar Cache de Categorias
```csharp
public class CategoryCacheService
{
    private Dictionary<long?, List<CategoryInfo>> _cache = new();
    
    public async Task<List<CategoryInfo>> GetCategoriesAsync(
        CategoryClient client,
        IList<string>? roles,
        long? parentId)
    {
        var cacheKey = $"{string.Join(",", roles ?? new List<string>())}_{parentId}";
        
        if (!_cache.ContainsKey(parentId))
        {
            var categories = await client.FilterAsync(roles, parentId);
            _cache[parentId] = categories.ToList();
        }
        
        return _cache[parentId];
    }
    
    public void ClearCache()
    {
        _cache.Clear();
    }
}
```

## Observações Importantes

?? **Hierarquia**: CategoryInfo deve ter propriedade `ParentId` para navegação hierárquica funcionar corretamente.

?? **Performance**: Carregar hierarquias profundas pode impactar performance. Considere lazy loading.

?? **Cache**: Para melhor UX, implemente cache de categorias já carregadas.

?? **Segurança**: Roles devem ser validados no backend. O filtro no cliente é apenas para UX.

? **Build**: Compilação bem-sucedida após todas as alterações.

## Arquivo de Alterações

### Arquivos Modificados:
1. `NNews.Maui\ViewModels\ArticleListViewModel.cs`
   - Adicionada propriedade `_roles`
   - Atualizado `InitializeAsync` com parâmetro `roles`
   - Substituído `GetAllAsync` por `FilterAsync` (3 ocorrências)

2. `NNews.Maui\ViewModels\SearchViewModel.cs`
   - Substituído `GetAllAsync` por `FilterAsync` (1 ocorrência)

3. `NNews.Maui\ViewModels\CategoryListViewModel.cs`
   - Adicionadas propriedades `_roles` e `_parentId`
   - Atualizado `InitializeAsync` com parâmetros `roles` e `parentId`
   - Substituído `GetAllAsync` por `FilterAsync` (2 ocorrências)

### Total de Mudanças:
- **Arquivos Modificados**: 3
- **Métodos Atualizados**: 6
- **Novas Propriedades**: 3
- **Novos Parâmetros**: 3

## Estrutura de Dados - CategoryInfo

```csharp
public class CategoryInfo
{
    public long CategoryId { get; set; }
    public long? ParentId { get; set; }        // ?? Suporte a hierarquia
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int ArticleCount { get; set; }      // ?? Contagem de artigos
}
```

---

**Status Final**: ? Implementado e Compilando com Sucesso
**Data de Implementação**: 2024
**Versão do NNews.ACL**: 0.2.1 (preparado para 2.0.0)
**ViewModels Atualizados**: 3 (ArticleListViewModel, SearchViewModel, CategoryListViewModel)
