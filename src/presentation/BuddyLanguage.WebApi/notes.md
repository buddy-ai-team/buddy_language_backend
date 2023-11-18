```shell
dotnet ef migrations add InitialCreate -p ../../infrastructure/BuddyLanguage.Data.EntityFramework --context AppDbContext
```
```
dotnet ef database update --context AppDbContext
```

```shell
dotnet ef migrations add I
nitialCreate -p ../../infrastructure/BuddyLanguage.Infrastructure --context ChatGptDbContext
dotnet ef database update --context ChatGptDbContext
```
