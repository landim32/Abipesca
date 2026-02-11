# NNews.Maui

Aplicação .NET MAUI para consumo da API NNews.

## ?? Sobre o Projeto

O NNews.Maui é uma aplicação multiplataforma desenvolvida em .NET MAUI para visualização de artigos, categorias e tags de um sistema de notícias. A aplicação consome a API NNews através das bibliotecas NNews.ACL e NNews.Dtos.

## ?? Tecnologias Utilizadas

- **.NET 8**
- **.NET MAUI** - Framework multiplataforma
- **CommunityToolkit.Mvvm** - Padrão MVVM
- **CommunityToolkit.Maui** - Componentes adicionais
- **Microsoft.Extensions.Http** - Cliente HTTP
- **Microsoft.Extensions.Configuration** - Configuração
- **NNews.ACL** - Biblioteca de acesso à API
- **NNews.Dtos** - Objetos de transferência de dados

## ?? Estrutura do Projeto

```
NNews.Maui/
??? ViewModels/          # ViewModels para padrão MVVM
?   ??? BaseViewModel.cs
?   ??? ArticleListViewModel.cs
?   ??? ArticleDetailViewModel.cs
?   ??? CategoryListViewModel.cs
?   ??? TagListViewModel.cs
?   ??? SearchViewModel.cs
??? Views/               # Páginas XAML
?   ??? ArticleListPage.xaml
?   ??? ArticleDetailPage.xaml
?   ??? CategoryListPage.xaml
?   ??? TagListPage.xaml
?   ??? SearchPage.xaml
??? Services/            # Serviços da aplicação
?   ??? NavigationService.cs
??? Converters/          # Conversores de valor
?   ??? DateTimeConverter.cs
?   ??? ImageUrlConverter.cs
??? Resources/           # Recursos da aplicação
```

## ?? Configuração

### 1. Configurar a URL da API

Edite o arquivo `appsettings.json` no projeto principal (Abipesca.Main):

```json
{
  "NNews": {
    "ApiUrl": "https://your-api-url.com"
  }
}
```

### 2. Adicionar as Bibliotecas

Certifique-se de que as DLLs `NNews.ACL.dll` e `NNews.Dtos.dll` estão na pasta `Lib` na raiz da solução.

### 3. Registrar os Serviços

No arquivo `MauiProgram.cs` do projeto principal, registre os serviços do NNews:

```csharp
// Configure NNews Settings
builder.Services.Configure<NNewsSetting>(
    builder.Configuration.GetSection("NNews"));

// HttpClients
builder.Services.AddHttpClient<IArticleClient, ArticleClient>();
builder.Services.AddHttpClient<ICategoryClient, CategoryClient>();
builder.Services.AddHttpClient<ITagClient, TagClient>();

// ViewModels
builder.Services.AddTransient<ArticleListViewModel>();
builder.Services.AddTransient<ArticleDetailViewModel>();
builder.Services.AddTransient<CategoryListViewModel>();
builder.Services.AddTransient<TagListViewModel>();
builder.Services.AddTransient<SearchViewModel>();

// Pages
builder.Services.AddTransient<ArticleListPage>();
builder.Services.AddTransient<ArticleDetailPage>();
builder.Services.AddTransient<CategoryListPage>();
builder.Services.AddTransient<TagListPage>();
builder.Services.AddTransient<SearchPage>();

// Services
builder.Services.AddSingleton<INavigationService, NavigationService>();
```

## ?? Funcionalidades

### Lista de Artigos
- ? Visualização de artigos em lista
- ? Infinite scroll (paginação automática)
- ? Pull-to-refresh
- ? Filtro por categoria
- ? Exibição de imagem, título, data, categoria e tags
- ? Navegação para detalhes do artigo

### Detalhes do Artigo
- ? Imagem hero em topo
- ? Título e metadata (data, categoria)
- ? Tags clicáveis
- ? Conteúdo formatado
- ? Compartilhamento do artigo

### Categorias
- ? Grid 2 colunas com botões quadrados
- ? Contador de artigos por categoria
- ? Cores diferenciadas
- ? Navegação para artigos da categoria

### Tags
- ? Lista de tags
- ? Contador de artigos por tag
- ? Slug da tag
- ? Navegação para artigos da tag

### Busca
- ? SearchBar com busca em tempo real
- ? Debounce de 500ms
- ? Histórico de buscas recentes
- ? Resultados filtrados
- ? Empty state

## ?? Design

### Cores
- **Primary**: #0066CC
- **Success**: #28A745
- **Danger**: #DC3545
- **Background**: #F8F9FA
- **Card**: #FFFFFF
- **Text**: #212529
- **Text Secondary**: #6C757D

### Tipografia
- **Title**: 24sp, Bold
- **Subtitle**: 18sp, SemiBold
- **Body**: 16sp, Regular
- **Caption**: 14sp, Regular
- **Small**: 12sp, Regular

## ?? Navegação

A aplicação utiliza o Shell navigation do .NET MAUI. As rotas disponíveis são:

- `articlelist` - Lista de artigos (com parâmetros opcionais: CategoryId, CategoryTitle)
- `articledetail` - Detalhes do artigo (parâmetro: ArticleId)
- `categories` - Lista de categorias
- `tags` - Lista de tags
- `search` - Busca de artigos

## ?? Uso

### Navegar para Lista de Artigos

```csharp
await Shell.Current.GoToAsync("articlelist");
```

### Navegar para Lista de Artigos de uma Categoria

```csharp
var parameters = new Dictionary<string, object>
{
    { "CategoryId", categoryId },
    { "CategoryTitle", "Nome da Categoria" }
};
await Shell.Current.GoToAsync("articlelist", parameters);
```

### Navegar para Detalhes do Artigo

```csharp
var parameters = new Dictionary<string, object>
{
    { "ArticleId", articleId }
};
await Shell.Current.GoToAsync("articledetail", parameters);
```

## ??? Desenvolvimento

### Pré-requisitos

- Visual Studio 2022 17.8 ou superior
- .NET 8 SDK
- Workload .NET MAUI instalado
- Para Android: Android SDK 21+
- Para iOS: Xcode 14+ (macOS)
- Para Windows: Windows 10 SDK 19041+

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run
```

## ?? Resolução de Problemas

### Erro de Certificado SSL (Android)

Se encontrar erros de SSL no Android, configure o `network_security_config.xml`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<network-security-config>
    <base-config cleartextTrafficPermitted="true">
        <trust-anchors>
            <certificates src="system" />
            <certificates src="user" />
        </trust-anchors>
    </base-config>
</network-security-config>
```

### Imagens não carregando

Verifique se a URL da API está configurada corretamente no `appsettings.json` e se o servidor de imagens está acessível.

## ?? Licença

Este projeto faz parte da solução Abipesca.

## ?? Contribuidores

Desenvolvido para integração com a API NNews.
