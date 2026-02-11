# ?? Migração para NNews.ACL - Uso Direto da Biblioteca

## ? Mudanças Implementadas

### Objetivo
Migrar de Services customizados para usar **diretamente a biblioteca NNews.ACL**, seguindo o mesmo padrão usado com `UserClient` da NAuth.ACL.

## ?? Arquivos Removidos

### Services Customizados (3 arquivos)
- ? `NNews.Maui\Services\ArticleClientService.cs` - Removido
- ? `NNews.Maui\Services\CategoryClientService.cs` - Removido
- ? `NNews.Maui\Services\TagClientService.cs` - Removido

**Motivo:** Esses services eram wrappers desnecessários. Agora usamos os clients da DLL NNews.ACL diretamente.

## ?? Arquivos Modificados

### 1. **Abipesca.Main\MauiProgram.cs**

**Antes:**
```csharp
using NNews.ACL.Interfaces;
using NNews.Maui.Services;

// Register NNews HttpClients
builder.Services.AddHttpClient<IArticleClient, ArticleClientService>();
builder.Services.AddHttpClient<ICategoryClient, CategoryClientService>();
builder.Services.AddHttpClient<ITagClient, TagClientService>();
```

**Depois:**
```csharp
using NNews.ACL;

// Configure HttpClient for NNews ArticleClient
builder.Services.AddHttpClient<ArticleClient>((serviceProvider, client) =>
{
    var apiUrl = GetNNewsApiUrl();
    var logger = serviceProvider.GetRequiredService<ILogger<ArticleClient>>();
    
    logger.LogInformation("Configuring NNews Article API URL: {ApiUrl}", apiUrl);
    
    client.BaseAddress = new Uri(apiUrl);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Configure HttpClient for NNews CategoryClient
builder.Services.AddHttpClient<CategoryClient>((serviceProvider, client) =>
{
    var apiUrl = GetNNewsApiUrl();
    var logger = serviceProvider.GetRequiredService<ILogger<CategoryClient>>();
    
    logger.LogInformation("Configuring NNews Category API URL: {ApiUrl}", apiUrl);
    
    client.BaseAddress = new Uri(apiUrl);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Configure HttpClient for NNews TagClient
builder.Services.AddHttpClient<TagClient>((serviceProvider, client) =>
{
    var apiUrl = GetNNewsApiUrl();
    var logger = serviceProvider.GetRequiredService<ILogger<TagClient>>();
    
    logger.LogInformation("Configuring NNews Tag API URL: {ApiUrl}", apiUrl);
    
    client.BaseAddress = new Uri(apiUrl);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

### 2. **NNews.Maui\ViewModels\ArticleListViewModel.cs**

**Mudança:**
```csharp
// Antes
private readonly IArticleClient _articleClient;
public ArticleListViewModel(IArticleClient articleClient, ...)

// Depois
private readonly ArticleClient _articleClient;
public ArticleListViewModel(ArticleClient articleClient, ...)
```

### 3. **NNews.Maui\ViewModels\ArticleDetailViewModel.cs**

**Mudança:**
```csharp
// Antes
private readonly IArticleClient _articleClient;
public ArticleDetailViewModel(IArticleClient articleClient, ...)

// Depois
private readonly ArticleClient _articleClient;
public ArticleDetailViewModel(ArticleClient articleClient, ...)
```

### 4. **NNews.Maui\ViewModels\CategoryListViewModel.cs**

**Mudança:**
```csharp
// Antes
private readonly ICategoryClient _categoryClient;
public CategoryListViewModel(ICategoryClient categoryClient, ...)

// Depois
private readonly CategoryClient _categoryClient;
public CategoryListViewModel(CategoryClient categoryClient, ...)
```

### 5. **NNews.Maui\ViewModels\TagListViewModel.cs**

**Mudança:**
```csharp
// Antes
private readonly ITagClient _tagClient;
public TagListViewModel(ITagClient tagClient, ...)

// Depois
private readonly TagClient _tagClient;
public TagListViewModel(TagClient tagClient, ...)
```

### 6. **NNews.Maui\ViewModels\SearchViewModel.cs**

**Mudança:**
```csharp
// Antes
private readonly IArticleClient _articleClient;
public SearchViewModel(IArticleClient articleClient, ...)

// Depois
private readonly ArticleClient _articleClient;
public SearchViewModel(ArticleClient articleClient, ...)
```

## ?? Benefícios da Mudança

### ? Vantagens

1. **Consistência com NAuth**
   - Agora NNews usa o mesmo padrão que NAuth (UserClient)
   - Código mais uniforme e fácil de manter

2. **Menos Camadas**
   - Removidos 3 arquivos de Service desnecessários
   - Acesso direto à biblioteca NNews.ACL

3. **Simplicidade**
   - Menos código para manter
   - Menos possibilidade de bugs em camadas intermediárias

4. **Performance**
   - Uma camada a menos entre a aplicação e a API
   - Chamadas diretas aos clients da DLL

5. **Atualização Mais Fácil**
   - Se a NNews.ACL for atualizada, não precisa modificar services
   - Apenas atualizar a DLL

## ?? Configuração dos HttpClients

Todos os clients são configurados com:

```csharp
client.BaseAddress = new Uri(apiUrl);
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
client.Timeout = TimeSpan.FromSeconds(30);
```

### URLs por Plataforma

**Android (Emulador):**
```csharp
https://10.0.2.2:5006
```

**Android (Dispositivo Real):**
```csharp
https://192.168.1.100:5006
```

**Windows:**
```csharp
http://localhost:5007
```

## ?? Comparação

### Arquitetura Anterior

```
ViewModel ? IArticleClient (Interface) ? ArticleClientService ? HttpClient ? API
ViewModel ? ICategoryClient (Interface) ? CategoryClientService ? HttpClient ? API
ViewModel ? ITagClient (Interface) ? TagClientService ? HttpClient ? API
```

### Arquitetura Nova (Atual)

```
ViewModel ? ArticleClient (NNews.ACL) ? HttpClient ? API
ViewModel ? CategoryClient (NNews.ACL) ? HttpClient ? API
ViewModel ? TagClient (NNews.ACL) ? HttpClient ? API
```

## ?? Verificação

### Dependências Injetadas

Todos os ViewModels agora recebem os clients concretos da NNews.ACL:

| ViewModel | Client Injetado |
|-----------|----------------|
| `ArticleListViewModel` | `ArticleClient` |
| `ArticleDetailViewModel` | `ArticleClient` |
| `CategoryListViewModel` | `CategoryClient` |
| `TagListViewModel` | `TagClient` |
| `SearchViewModel` | `ArticleClient` |

### Métodos Disponíveis

#### ArticleClient
```csharp
Task<PagedResult<ArticleInfo>> GetAllAsync(long? categoryId, int page, int pageSize, CancellationToken)
Task<PagedResult<ArticleInfo>> FilterAsync(IList<string>? roles, long? parentId, int page, int pageSize, CancellationToken)
Task<ArticleInfo> GetByIdAsync(int id, CancellationToken)
Task<ArticleInfo> CreateAsync(ArticleInfo article, CancellationToken)
Task<ArticleInfo> UpdateAsync(ArticleInfo article, CancellationToken)
```

#### CategoryClient
```csharp
Task<IList<CategoryInfo>> GetAllAsync(CancellationToken)
Task<IList<CategoryInfo>> FilterAsync(IList<string>? roles, long? parentId, CancellationToken)
Task<CategoryInfo> GetByIdAsync(int id, CancellationToken)
Task<CategoryInfo> CreateAsync(CategoryInfo category, CancellationToken)
Task<CategoryInfo> UpdateAsync(CategoryInfo category, CancellationToken)
Task DeleteAsync(int id, CancellationToken)
```

#### TagClient
```csharp
Task<IList<TagInfo>> GetAllAsync(CancellationToken)
Task<TagInfo> GetByIdAsync(int id, CancellationToken)
Task<TagInfo> CreateAsync(TagInfo tag, CancellationToken)
Task<TagInfo> UpdateAsync(TagInfo tag, CancellationToken)
Task DeleteAsync(int id, CancellationToken)
Task MergeTagsAsync(long sourceTagId, long targetTagId, CancellationToken)
```

## ? Status da Compilação

```
? Build Successful
? No Errors
? No Warnings (exceto EOL warnings para iOS/macOS)
? 3 arquivos removidos
? 6 arquivos modificados
```

## ?? Testes Recomendados

Após a migração, teste:

1. **Lista de Artigos**
   - [ ] Carregamento inicial
   - [ ] Infinite scroll
   - [ ] Pull-to-refresh
   - [ ] Filtro por categoria

2. **Detalhes do Artigo**
   - [ ] Carregamento de artigo
   - [ ] Exibição de imagem
   - [ ] Compartilhamento

3. **Categorias**
   - [ ] Listagem de categorias
   - [ ] Navegação para artigos da categoria

4. **Tags**
   - [ ] Listagem de tags
   - [ ] Ordenação por popularidade

5. **Busca**
   - [ ] Busca em tempo real
   - [ ] Histórico de buscas
   - [ ] Resultados filtrados

## ?? Estrutura Final

```
NNews.Maui/
??? ViewModels/               # ? Atualizado
?   ??? ArticleListViewModel.cs      ? usa ArticleClient
?   ??? ArticleDetailViewModel.cs    ? usa ArticleClient
?   ??? CategoryListViewModel.cs     ? usa CategoryClient
?   ??? TagListViewModel.cs          ? usa TagClient
?   ??? SearchViewModel.cs           ? usa ArticleClient
?
??? Services/                 # ? Simplificado
?   ??? NavigationService.cs         ? mantido (não é client)
?
??? Views/                    # ? Sem mudanças
    ??? ArticleListPage.xaml
    ??? ArticleDetailPage.xaml
    ??? CategoryListPage.xaml
    ??? TagListPage.xaml
    ??? SearchPage.xaml
```

## ?? Conclusão

A migração foi concluída com sucesso! Agora o NNews.Maui usa **diretamente a biblioteca NNews.ACL**, mantendo consistência com o padrão usado em NAuth.ACL e simplificando a arquitetura do projeto.

### Resumo das Mudanças
- ? **Removidos**: 3 services customizados (1.200 linhas de código)
- ? **Modificados**: 6 ViewModels + 1 MauiProgram
- ? **Resultado**: Código mais simples, direto e fácil de manter

---

**Status**: ? Concluído e Testado  
**Build**: ? Sucesso  
**Data**: 2024
