# NNews.Maui - Exemplo de Configuração Completa

## ?? Checklist de Configuração

### ? Passos já Concluídos

- [x] Projeto NNews.Maui criado
- [x] ViewModels implementados (ArticleList, ArticleDetail, CategoryList, TagList, Search)
- [x] Views XAML criadas para todas as páginas
- [x] Services implementados (NavigationService, ArticleClientService, CategoryClientService, TagClientService)
- [x] Converters criados (DateTimeConverter, ImageUrlConverter, InvertedBoolConverter, HasItemsConverter)
- [x] BaseViewModel com funcionalidades comuns
- [x] Integração com Abipesca.Main
- [x] Rotas registradas no AppShell
- [x] Configuração de serviços no MauiProgram.cs
- [x] appsettings.json configurado

### ?? Configurações Necessárias

#### 1. Verifique a URL da API

No arquivo `Abipesca.Main\appsettings.json`:

```json
{
  "NAuthSetting": {
    "ApiUrl": "https://10.0.2.2:5005"
  },
  "NNews": {
    "ApiUrl": "https://10.0.2.2:5006"
  },
  "SessionTimeout": 3600,
  "EnableRememberMe": true
}
```

**Ajuste a porta e o IP conforme seu ambiente:**

- Emulador Android: `https://10.0.2.2:PORTA`
- Dispositivo Android: `https://SEU_IP_LOCAL:PORTA`
- Windows: `https://localhost:PORTA`

#### 2. Verifique as DLLs no diretório Lib

Certifique-se de que as seguintes DLLs existem em `Lib\`:

- `NNews.ACL.dll`
- `NNews.Dtos.dll`
- `NAuth.ACL.dll`
- `NAuth.DTO.dll`

#### 3. Teste a Conectividade

Antes de executar o app, teste se a API está acessível:

```bash
# Windows
curl https://localhost:5006/api/articles

# Android (pelo ADB)
adb shell curl https://10.0.2.2:5006/api/articles
```

## ?? Customização de UI

### Alterando Cores

Edite as cores diretamente nos arquivos XAML ou crie um ResourceDictionary:

**Resources\Styles\Colors.xaml**
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    
    <!-- NNews Colors -->
    <Color x:Key="NNewsPrimary">#0066CC</Color>
    <Color x:Key="NNewsSuccess">#28A745</Color>
    <Color x:Key="NNewsDanger">#DC3545</Color>
    <Color x:Key="NNewsBackground">#F8F9FA</Color>
    <Color x:Key="NNewsCard">#FFFFFF</Color>
    <Color x:Key="NNewsText">#212529</Color>
    <Color x:Key="NNewsTextSecondary">#6C757D</Color>
    
</ResourceDictionary>
```

### Alterando Layout

Para alterar o layout dos cards de artigos, edite:
- `NNews.Maui\Views\ArticleListPage.xaml` - Lista principal
- `NNews.Maui\Views\ArticleDetailPage.xaml` - Detalhes

## ?? Testando a Implementação

### Teste 1: Navegação Básica

Adicione um botão temporário no `MainPage.xaml` do NAuth:

```xml
<Button Text="Testar Notícias" 
        Clicked="OnTestNewsClicked" 
        BackgroundColor="#0066CC" />
```

No code-behind `MainPage.xaml.cs`:

```csharp
private async void OnTestNewsClicked(object sender, EventArgs e)
{
    await Shell.Current.GoToAsync("articlelist");
}
```

### Teste 2: Verificar ViewModels

Execute o app e verifique no Output window:

1. Se os serviços são injetados corretamente
2. Se as chamadas à API são feitas
3. Se há erros de conectividade

### Teste 3: Verificar Navegação

1. Navegue para a lista de artigos
2. Toque em um artigo
3. Verifique se abre a página de detalhes
4. Teste o botão de voltar

## ?? Estrutura de Menu (Sugestão)

Você pode criar um menu no AppShell para acesso rápido:

**AppShell.xaml**
```xml
<Shell.FlyoutBehavior>Flyout</Shell.FlyoutBehavior>

<FlyoutItem Title="Notícias" Icon="news.png">
    <ShellContent Title="Artigos" 
                  ContentTemplate="{DataTemplate newsViews:ArticleListPage}" 
                  Route="articlelist" />
    <ShellContent Title="Categorias" 
                  ContentTemplate="{DataTemplate newsViews:CategoryListPage}" 
                  Route="categories" />
    <ShellContent Title="Tags" 
                  ContentTemplate="{DataTemplate newsViews:TagListPage}" 
                  Route="tags" />
    <ShellContent Title="Buscar" 
                  ContentTemplate="{DataTemplate newsViews:SearchPage}" 
                  Route="search" />
</FlyoutItem>

<FlyoutItem Title="Conta" Icon="account.png">
    <ShellContent Title="Perfil" 
                  ContentTemplate="{DataTemplate authViews:ProfilePage}" 
                  Route="profile" />
    <ShellContent Title="Alterar Senha" 
                  ContentTemplate="{DataTemplate authViews:ChangePasswordPage}" 
                  Route="changepassword" />
</FlyoutItem>
```

## ?? Debug e Logs

Para ver logs detalhados, o `MauiProgram.cs` já está configurado com:

```csharp
#if DEBUG
    builder.Logging.AddDebug();
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif
```

Os logs aparecerão no Output window do Visual Studio durante a execução.

## ?? Exemplo de Resposta da API

### GET /api/articles

```json
{
  "items": [
    {
      "articleId": 1,
      "categoryId": 1,
      "title": "Título do Artigo",
      "content": "Conteúdo completo...",
      "imageName": "artigo1.jpg",
      "dateAt": "2024-01-15T10:30:00",
      "category": {
        "categoryId": 1,
        "title": "Tecnologia"
      },
      "tags": [
        {
          "tagId": 1,
          "title": ".NET MAUI",
          "slug": "dotnet-maui"
        }
      ]
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 50,
  "totalPages": 5,
  "hasPrevious": false,
  "hasNext": true
}
```

## ?? Próximas Melhorias

### Curto Prazo
1. ? Adicionar ícones personalizados para as páginas
2. ?? Criar temas claro/escuro
3. ?? Otimizar para tablets
4. ?? Implementar notificações

### Médio Prazo
1. ?? Implementar cache local com SQLite
2. ?? Modo de leitura offline
3. ? Sistema de favoritos
4. ?? Sistema de comentários

### Longo Prazo
1. ?? Suporte a múltiplos idiomas
2. ??? Artigos em áudio (TTS)
3. ?? Analytics de leitura
4. ?? Recomendações personalizadas

## ?? Suporte

Para problemas ou dúvidas:

1. Verifique os logs no Output window
2. Revise o README.md do NNews.Maui
3. Consulte o INTEGRATION_GUIDE.md para exemplos de uso
4. Verifique a conectividade com a API

## ? Validação Final

Execute esta checklist antes de considerar a implementação completa:

- [ ] A aplicação compila sem erros
- [ ] A lista de artigos carrega corretamente
- [ ] O infinite scroll funciona
- [ ] O pull-to-refresh funciona
- [ ] A navegação para detalhes funciona
- [ ] As imagens carregam corretamente
- [ ] A busca funciona
- [ ] As categorias são exibidas
- [ ] As tags são exibidas
- [ ] O botão de compartilhar funciona
- [ ] A navegação por categorias funciona

## ?? Parabéns!

Se todos os itens da checklist estão marcados, sua implementação do NNews.Maui está completa e funcional!
