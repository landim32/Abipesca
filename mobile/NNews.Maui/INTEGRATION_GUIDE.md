# Integração do NNews.Maui no Abipesca.Main

## Como Usar o NNews.Maui

O NNews.Maui já está configurado e pronto para uso no projeto Abipesca.Main. Aqui estão exemplos de como navegar para as páginas de notícias.

### 1. Navegar para Lista de Artigos

```csharp
// Navegar para todos os artigos
await Shell.Current.GoToAsync("articlelist");

// Navegar para artigos de uma categoria específica
var parameters = new Dictionary<string, object>
{
    { "CategoryId", 1L },
    { "CategoryTitle", "Tecnologia" }
};
await Shell.Current.GoToAsync("articlelist", parameters);
```

### 2. Navegar para Detalhes do Artigo

```csharp
var parameters = new Dictionary<string, object>
{
    { "ArticleId", 123L }
};
await Shell.Current.GoToAsync("articledetail", parameters);
```

### 3. Navegar para Lista de Categorias

```csharp
await Shell.Current.GoToAsync("categories");
```

### 4. Navegar para Lista de Tags

```csharp
await Shell.Current.GoToAsync("tags");
```

### 5. Navegar para Busca

```csharp
await Shell.Current.GoToAsync("search");
```

## Exemplo de Implementação no MainPage

Você pode adicionar botões no MainPage para navegar para as páginas de notícias:

### MainPage.xaml (NAuth.Maui)
```xml
<StackLayout Padding="20" Spacing="10">
    <Label Text="Bem-vindo!" FontSize="24" FontAttributes="Bold" />
    
    <!-- Botões de Notícias -->
    <Button Text="Ver Artigos" 
            Command="{Binding NavigateToArticlesCommand}" 
            BackgroundColor="#0066CC" />
    
    <Button Text="Categorias" 
            Command="{Binding NavigateToCategoriesCommand}" 
            BackgroundColor="#0066CC" />
    
    <Button Text="Tags" 
            Command="{Binding NavigateToTagsCommand}" 
            BackgroundColor="#0066CC" />
    
    <Button Text="Buscar" 
            Command="{Binding NavigateToSearchCommand}" 
            BackgroundColor="#0066CC" />
</StackLayout>
```

### MainViewModel.cs (NAuth.Maui)
```csharp
public class MainViewModel : BaseViewModel
{
    public ICommand NavigateToArticlesCommand { get; }
    public ICommand NavigateToCategoriesCommand { get; }
    public ICommand NavigateToTagsCommand { get; }
    public ICommand NavigateToSearchCommand { get; }

    public MainViewModel()
    {
        NavigateToArticlesCommand = new Command(async () => 
            await Shell.Current.GoToAsync("articlelist"));
            
        NavigateToCategoriesCommand = new Command(async () => 
            await Shell.Current.GoToAsync("categories"));
            
        NavigateToTagsCommand = new Command(async () => 
            await Shell.Current.GoToAsync("tags"));
            
        NavigateToSearchCommand = new Command(async () => 
            await Shell.Current.GoToAsync("search"));
    }
}
```

## Configuração da API

A URL da API do NNews está configurada em `appsettings.json`:

```json
{
  "NNews": {
    "ApiUrl": "https://10.0.2.2:5006"
  }
}
```

### URLs por Plataforma

O `MauiProgram.cs` já está configurado para usar URLs diferentes por plataforma:

- **Android (Emulador)**: `https://10.0.2.2:5006`
- **Android (Dispositivo Real)**: `https://192.168.1.100:5006`
- **Windows**: `https://localhost:5006`

## Estrutura de Dados

### ArticleInfo
```csharp
{
    "articleId": 123,
    "categoryId": 1,
    "title": "Título do Artigo",
    "content": "Conteúdo do artigo...",
    "imageName": "imagem.jpg",
    "dateAt": "2024-01-15T10:30:00",
    "category": {
        "categoryId": 1,
        "title": "Tecnologia"
    },
    "tags": [
        {
            "tagId": 1,
            "title": ".NET MAUI"
        }
    ]
}
```

### CategoryInfo
```csharp
{
    "categoryId": 1,
    "title": "Tecnologia",
    "articleCount": 25
}
```

### TagInfo
```csharp
{
    "tagId": 1,
    "title": ".NET MAUI",
    "slug": "dotnet-maui",
    "articleCount": 10
}
```

## Personalização

### Cores

Você pode personalizar as cores editando os arquivos XAML. As cores padrão são:

- **Primary**: #0066CC
- **Success**: #28A745
- **Danger**: #DC3545
- **Background**: #F8F9FA
- **Card**: #FFFFFF
- **Text**: #212529
- **Text Secondary**: #6C757D

### Imagens

As imagens dos artigos são carregadas automaticamente da API. O `ImageUrlConverter` converte nomes de imagens em URLs completas:

- Se a imagem começa com `http://` ou `https://`, usa a URL diretamente
- Caso contrário, concatena com a URL da API: `{ApiUrl}/images/{imageName}`

## Funcionalidades Implementadas

? **Lista de Artigos**
- Infinite scroll (carrega mais ao rolar)
- Pull-to-refresh
- Filtro por categoria
- Exibição de imagem, título, data, categoria e tags

? **Detalhes do Artigo**
- Imagem hero
- Título e metadata
- Tags clicáveis
- Conteúdo formatado
- Compartilhamento

? **Categorias**
- Grid 2 colunas
- Contador de artigos
- Navegação para artigos da categoria

? **Tags**
- Lista ordenada por popularidade
- Contador de artigos
- Slug da tag

? **Busca**
- Busca em tempo real com debounce
- Histórico de buscas
- Empty state

## Troubleshooting

### Erro de SSL no Android

Se encontrar erros de certificado SSL, verifique o `network_security_config.xml`:

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

### Imagens não carregam

Verifique:
1. A URL da API está correta
2. O servidor de imagens está acessível
3. As imagens existem no caminho `/images/` do servidor

### Erros de Navegação

Certifique-se de que todas as rotas estão registradas no `AppShell.xaml.cs`:
- `articlelist`
- `articledetail`
- `categories`
- `tags`
- `search`

## Próximos Passos

Para uma experiência completa, considere implementar:

1. **Autenticação**: Integrar autenticação para acesso a artigos restritos
2. **Favoritos**: Salvar artigos favoritos localmente
3. **Modo Offline**: Cache local de artigos
4. **Notificações**: Push notifications para novos artigos
5. **Compartilhamento Social**: Compartilhar em redes sociais
6. **Comentários**: Sistema de comentários nos artigos
7. **Dark Mode**: Suporte a tema escuro
