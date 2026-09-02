# Configuração de segredos

Não inclua chaves, senhas ou tokens nos arquivos versionados do projeto.

## JWT local

Crie uma chave aleatória e salve-a como variável de ambiente do usuário:

```powershell
$chaveJwt = [Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(64))
[Environment]::SetEnvironmentVariable("Jwt__SecretKey", $chaveJwt, "User")
```

Feche e abra novamente o terminal ou o VS Code depois disso. O ASP.NET Core lê `Jwt__SecretKey` como `Jwt:SecretKey`.

## Outros segredos locais

- `GEMINI_API_KEY`: chave de acesso à API Gemini.
- `ConnectionStrings__DefaultConnection`: conexão local do banco, caso prefira não mantê-la em `appsettings.Development.json`.
- `SupabaseStorage__ServiceRoleKey`: chave administrativa do Supabase Storage.

`appsettings.Development.json` já é ignorado pelo Git e pode conter configurações exclusivas da sua máquina. Nunca o adicione manualmente ao commit.
