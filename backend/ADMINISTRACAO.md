# Administração do acervo

Depois de publicar a migration e criar sua conta, promova o e-mail do administrador no SQL Editor do Supabase:

```sql
UPDATE usuarios
SET "Role" = 1
WHERE "Email" = 'admin@email.com';
```

Faça logout e login novamente. O botão **Admin** aparecerá no cabeçalho.

Para a ingestão em produção, configure no Render as variáveis privadas `SupabaseStorage__Url` e `SupabaseStorage__ServiceRoleKey`. O bucket `fontes-anatomia` deve ser privado.
