# NAuth.Maui - Sistema de Autenticação

Sistema completo de autenticação de usuários para aplicativos .NET MAUI usando a biblioteca NAuth.ACL.

## ?? Características

- ? Login com e-mail e senha
- ? Cadastro de novos usuários
- ? Recuperação de senha via e-mail
- ? Redefinição de senha com hash
- ? Alteração de senha (usuário logado)
- ? Perfil do usuário
- ? Upload de foto de perfil
- ? Persistência de sessão (SecureStorage)
- ? Validações completas de entrada
- ? Tratamento de erros robusto
- ? Design responsivo e intuitivo

## ?? Pré-requisitos

- .NET 8.0 SDK ou superior
- Visual Studio 2022 17.8 ou superior (com workload MAUI)
- Bibliotecas NAuth.ACL e NAuth.DTO

## ??? Estrutura do Projeto

```
NAuth.Maui/
??? Services/               # Serviços de autenticação e usuário
?   ??? IAuthService.cs
?   ??? AuthService.cs
?   ??? IUserService.cs
?   ??? UserService.cs
??? ViewModels/            # ViewModels com lógica de negócio
?   ??? BaseViewModel.cs
?   ??? LoginViewModel.cs
?   ??? RegisterViewModel.cs
?   ??? ForgotPasswordViewModel.cs
?   ??? ResetPasswordViewModel.cs
?   ??? ChangePasswordViewModel.cs
?   ??? ProfileViewModel.cs
?   ??? MainViewModel.cs
??? Views/                 # Páginas XAML
?   ??? LoginPage.xaml
?   ??? RegisterPage.xaml
?   ??? ForgotPasswordPage.xaml
?   ??? ResetPasswordPage.xaml
?   ??? ChangePasswordPage.xaml
?   ??? ProfilePage.xaml
?   ??? MainPage.xaml
??? Helpers/              # Classes auxiliares
    ??? NAuthSetting.cs
```

## ?? Configuração

### 1. Configurar URL da API

Edite o arquivo `MauiProgram.cs` no projeto `Abipesca.Main`:

```csharp
builder.Services.Configure<NAuthSetting>(options =>
{
    options.ApiUrl = "https://sua-api-url.com/api"; // Substitua pela URL da sua API
});
```

Ou edite o arquivo `appsettings.json`:

```json
{
  "NAuthSetting": {
    "ApiUrl": "https://sua-api-url.com/api"
  }
}
```

### 2. Permissões (Android)

Adicione as seguintes permissões no arquivo `AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.CAMERA" />
```

### 3. Permissões (iOS)

Adicione as seguintes entradas no arquivo `Info.plist`:

```xml
<key>NSCameraUsageDescription</key>
<string>Este aplicativo precisa acessar a câmera para tirar fotos de perfil</string>
<key>NSPhotoLibraryUsageDescription</key>
<string>Este aplicativo precisa acessar a galeria de fotos</string>
```

## ?? Uso

### Serviços

#### AuthService

```csharp
// Injetar o serviço
private readonly IAuthService _authService;

// Login
var user = await _authService.LoginAsync(email, password);

// Logout
await _authService.LogoutAsync();

// Verificar autenticação
var isAuthenticated = await _authService.IsAuthenticatedAsync();

// Obter token
var token = await _authService.GetTokenAsync();

// Obter usuário atual
var currentUser = await _authService.GetCurrentUserAsync();
```

#### UserService

```csharp
// Injetar o serviço
private readonly IUserService _userService;

// Cadastrar usuário
var newUser = new UserInfo { /* ... */ };
var result = await _userService.RegisterAsync(newUser);

// Atualizar perfil
var updatedUser = await _userService.UpdateProfileAsync(userInfo);

// Alterar senha
var success = await _userService.ChangePasswordAsync(oldPassword, newPassword);

// Enviar e-mail de recuperação
var success = await _userService.SendPasswordRecoveryAsync(email);

// Redefinir senha com hash
var success = await _userService.ResetPasswordAsync(hash, newPassword);

// Upload de foto
var imageUrl = await _userService.UploadProfileImageAsync(stream, fileName);
```

## ?? Telas Implementadas

### 1. Login (LoginPage)
- Campo de e-mail
- Campo de senha
- Botão de login
- Link para cadastro
- Link para recuperação de senha

### 2. Cadastro (RegisterPage)
- Nome completo
- E-mail
- Senha e confirmação
- Data de nascimento (opcional)
- CPF/Documento (opcional)

### 3. Recuperação de Senha (ForgotPasswordPage)
- Campo de e-mail
- Envio de e-mail de recuperação

### 4. Redefinição de Senha (ResetPasswordPage)
- Nova senha
- Confirmação de senha
- Recebe hash via deep link

### 5. Alteração de Senha (ChangePasswordPage)
- Senha atual
- Nova senha
- Confirmação de senha

### 6. Perfil (ProfilePage)
- Foto de perfil (com upload)
- Edição de dados pessoais
- Nome, e-mail, data de nascimento, CPF

### 7. Página Principal (MainPage)
- Informações do usuário
- Menu de navegação
- Acesso ao perfil
- Alteração de senha
- Logout

## ?? Segurança

- ? Tokens armazenados usando SecureStorage do MAUI
- ? Senhas nunca armazenadas em texto plano
- ? Validação de entrada para prevenir injection
- ? HTTPS obrigatório para todas as requisições
- ? Validações de formato de e-mail
- ? Requisitos de senha (mínimo 8 caracteres, letras e números)

## ?? Validações Implementadas

### Login
- E-mail: formato válido
- Senha: não vazio

### Cadastro
- Nome: mínimo 3 caracteres
- E-mail: formato válido
- Senha: mínimo 8 caracteres, letras e números
- Confirmação de senha: deve coincidir

### Alteração/Redefinição de Senha
- Nova senha: mínimo 8 caracteres, letras e números
- Confirmação: deve coincidir

## ?? Fluxo de Navegação

```
[App Start]
    |
    v
[Verificar Token]
    |
    +-- Token válido --> [MainPage]
    |
    +-- Sem token --> [LoginPage]
                          |
                          +-- Cadastro --> [RegisterPage]
                          |
                          +-- Esqueci senha --> [ForgotPasswordPage]
                          |
                          +-- Login OK --> [MainPage]
                                              |
                                              +-- Perfil --> [ProfilePage]
                                              |
                                              +-- Alterar senha --> [ChangePasswordPage]
                                              |
                                              +-- Logout --> [LoginPage]
```

## ?? Dependências

### NuGet Packages
- Microsoft.Maui.Controls (8.0+)
- Microsoft.Extensions.Http (8.0.1)
- Microsoft.Extensions.Options (8.0.2)

### Bibliotecas Externas
- NAuth.ACL.dll (API client)
- NAuth.DTO.dll (Data Transfer Objects)

## ??? Desenvolvimento

### Adicionar Nova Funcionalidade

1. Criar ViewModel herdando de `BaseViewModel`
2. Criar View (XAML + Code-behind)
3. Registrar no `MauiProgram.cs`
4. Adicionar rota no `AppShell.xaml.cs`

### Customizar UI

As cores e estilos podem ser customizados nos arquivos:
- `Resources/Styles/Colors.xaml`
- `Resources/Styles/Styles.xaml`

## ?? Troubleshooting

### Erro de autenticação
- Verificar se a URL da API está correta
- Verificar conexão com internet
- Verificar logs de debug

### Erro ao fazer upload de foto
- Verificar permissões de câmera/galeria
- Verificar se o arquivo é uma imagem válida
- Verificar tamanho do arquivo

### Erro de navegação
- Verificar se todas as rotas estão registradas no `AppShell.xaml.cs`
- Verificar se as Views estão registradas no DI

## ?? TODO

- [ ] Implementar refresh token
- [ ] Adicionar autenticação biométrica
- [ ] Implementar cache local
- [ ] Adicionar testes unitários
- [ ] Implementar modo offline
- [ ] Adicionar analytics

## ?? Licença

Este projeto está sob a licença MIT.

## ?? Contribuindo

Contribuições são bem-vindas! Por favor, abra uma issue ou pull request.

## ?? Suporte

Para suporte, abra uma issue no GitHub ou entre em contato com a equipe de desenvolvimento.
