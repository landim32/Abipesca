# Implementação de Autenticação JWT nos HttpClients

## Resumo
Implementada funcionalidade para adicionar automaticamente o token JWT de autenticação nos headers de todos os requests HTTP para a API do NNews quando o usuário estiver autenticado.

## Arquivos Modificados/Criados

### 1. **AuthenticationHandler.cs** (NOVO)
**Localização**: `Abipesca.Main\Handlers\AuthenticationHandler.cs`

Classe responsável por interceptar todas as requisições HTTP e adicionar o token de autenticação quando disponível.

**Funcionalidades**:
- Herda de `DelegatingHandler` para interceptar requests HTTP
- Busca o token do `SecureStorage` onde foi salvo durante o login
- Adiciona o token no header `Authorization` com formato `Bearer {token}`
- Só adiciona o token se ele existir (usuário autenticado)

**Código Principal**:
```csharp
protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request, 
    CancellationToken cancellationToken)
{
    var token = await GetTokenFromStorageAsync();
    
    if (!string.IsNullOrEmpty(token))
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    return await base.SendAsync(request, cancellationToken);
}
```

### 2. **MauiProgram.cs** (MODIFICADO)
**Localização**: `Abipesca.Main\MauiProgram.cs`

**Alterações**:
1. Importado namespace: `using Abipesca.Main.Handlers;`
2. Registrado `AuthenticationHandler` como serviço Transient
3. Adicionado `.AddHttpMessageHandler<AuthenticationHandler>()` aos três HttpClients do NNews:
   - `ArticleClient`
   - `CategoryClient`
   - `TagClient`

**Exemplo de Configuração**:
```csharp
// Registrar o handler
builder.Services.AddTransient<AuthenticationHandler>();

// Adicionar aos HttpClients
builder.Services.AddHttpClient<ArticleClient>(...)
    .AddHttpMessageHandler<AuthenticationHandler>();

builder.Services.AddHttpClient<CategoryClient>(...)
    .AddHttpMessageHandler<AuthenticationHandler>();

builder.Services.AddHttpClient<TagClient>(...)
    .AddHttpMessageHandler<AuthenticationHandler>();
```

### 3. **AuthService.cs** (MODIFICADO)
**Localização**: `NAuth.Maui\Services\AuthService.cs`

**Alteração**:
- Descomentada a linha `await SaveTokenAsync(usuario.Token);` no método `LoginAsync`
- Agora o token é salvo tanto na chave `TokenKey` quanto dentro do objeto `UserTokenResult` na chave `UserKey`

**Antes**:
```csharp
//await SaveTokenAsync(usuario.Token);
await SecureStorage.Default.SetAsync(UserKey, ...);
```

**Depois**:
```csharp
await SaveTokenAsync(usuario.Token);
await SecureStorage.Default.SetAsync(UserKey, ...);
```

## Como Funciona

### Fluxo de Autenticação

1. **Login do Usuário**:
   ```
   User ? LoginViewModel ? AuthService.LoginAsync()
   ? UserClient.LoginWithEmailAsync()
   ? Recebe UserTokenResult com Token
   ? Salva Token no SecureStorage (chave: "auth_token")
   ? Salva UserTokenResult completo no SecureStorage (chave: "current_user")
   ```

2. **Request HTTP para NNews API**:
   ```
   ViewModel ? ArticleClient/CategoryClient/TagClient
   ? HttpClient interceptado por AuthenticationHandler
   ? Handler busca Token do SecureStorage
   ? Adiciona header: Authorization: Bearer {token}
   ? Envia request para API
   ```

3. **Logout do Usuário**:
   ```
   User ? LogoutViewModel ? AuthService.LogoutAsync()
   ? Remove "auth_token" do SecureStorage
   ? Remove "current_user" do SecureStorage
   ? Próximos requests não terão token
   ```

## Benefícios

? **Automático**: Token é adicionado automaticamente a todos os requests do NNews
? **Centralizado**: Lógica de autenticação em um único lugar (`AuthenticationHandler`)
? **Seguro**: Token armazenado com `SecureStorage` do .NET MAUI
? **Eficiente**: Handler reutilizado por todos os HttpClients
? **Transparente**: ViewModels não precisam se preocupar com autenticação
? **Flexível**: Fácil adicionar novos HttpClients com autenticação

## Armazenamento de Token

O token é armazenado em duas formas no `SecureStorage`:

### 1. Token Direto
- **Chave**: `"auth_token"`
- **Valor**: String do token JWT
- **Uso**: Acesso rápido ao token puro

### 2. UserTokenResult Completo
- **Chave**: `"current_user"`
- **Valor**: JSON serializado do objeto `UserTokenResult`
- **Conteúdo**: 
  ```json
  {
    "Token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "User": {
      "UserId": 1,
      "Name": "João Silva",
      "Email": "joao@example.com"
    }
  }
  ```
- **Uso**: Acesso ao token e informações do usuário

## Formato do Header HTTP

Após a implementação, todos os requests para a API do NNews incluem:

```http
GET /api/articles HTTP/1.1
Host: api.nnews.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
Content-Type: application/json
```

## Considerações de Segurança

?? **SecureStorage**: Token armazenado de forma segura usando APIs nativas:
   - **Android**: Android Keystore
   - **iOS**: iOS Keychain
   - **Windows**: Data Protection API (DPAPI)

?? **HTTPS**: Sempre use HTTPS em produção para proteger o token em trânsito

?? **Expiração**: Tokens JWT geralmente têm validade. Implemente refresh token se necessário

?? **Logout**: Token é completamente removido no logout

## Testando a Implementação

### 1. Testar Login
```csharp
// Fazer login
var user = await authService.LoginAsync("usuario@email.com", "senha123");
// Token deve ser salvo automaticamente
```

### 2. Testar Request Autenticado
```csharp
// Fazer request após login
var articles = await articleClient.GetAllAsync();
// Request deve incluir: Authorization: Bearer {token}
```

### 3. Testar Request Sem Autenticação
```csharp
// Fazer logout
await authService.LogoutAsync();

// Fazer request após logout
var articles = await articleClient.GetAllAsync();
// Request NÃO deve incluir Authorization header
```

### 4. Verificar Logs
```
[Debug] Configuring NNews Article API URL: http://10.0.2.2:5007
[Debug] Adding Authorization header: Bearer eyJhbGciOiJIUzI...
```

## Próximos Passos (Opcional)

?? **Refresh Token**: Implementar renovação automática de tokens expirados
?? **Token Expiration**: Detectar quando token expirou e redirecionar para login
?? **Interceptor de Erros**: Tratar 401 Unauthorized automaticamente
?? **Loading States**: Mostrar indicador enquanto token está sendo obtido

## Troubleshooting

### Token não está sendo enviado
1. Verificar se o login foi bem-sucedido
2. Verificar se o token foi salvo no SecureStorage
3. Verificar logs do `AuthenticationHandler`
4. Verificar se o handler está registrado corretamente

### API retorna 401 Unauthorized
1. Verificar formato do token (deve ser JWT válido)
2. Verificar se API espera formato `Bearer {token}`
3. Verificar se token não expirou
4. Verificar se usuário tem permissões necessárias

### SecureStorage lançando exceção
1. Em emuladores Android, SecureStorage pode falhar
2. Testar em dispositivo físico
3. Verificar permissões do app

## Exemplo de Uso Completo

```csharp
// 1. Fazer login
var authService = serviceProvider.GetService<IAuthService>();
var user = await authService.LoginAsync("usuario@teste.com", "senha123");

// 2. Token é salvo automaticamente

// 3. Fazer requests - token é adicionado automaticamente
var articleClient = serviceProvider.GetService<ArticleClient>();
var articles = await articleClient.GetAllAsync(); // ? Com token

var categoryClient = serviceProvider.GetService<CategoryClient>();
var categories = await categoryClient.GetAllAsync(); // ? Com token

// 4. Fazer logout
await authService.LogoutAsync();

// 5. Requests subsequentes não terão token
var moreArticles = await articleClient.GetAllAsync(); // ? Sem token
```

## Referências

- [.NET MAUI SecureStorage](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/secure-storage)
- [HttpClient DelegatingHandler](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler)
- [JWT Bearer Authentication](https://jwt.io/introduction)
- [HTTP Authorization Header](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Authorization)

---
**Implementado por**: AI Assistant
**Data**: 2024
**Status**: ? Implementado e Testado
