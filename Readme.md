## Migrations

### Add a new migration

```sh
dotnet ef migrations add Refactor --project src/TournamentPlanner.Data/TournamentPlanner.Data --startup-project src/TournamentPlanner.Api/TournamentPlanner.Api --context AuthDbContext --output-dir Migrations
```

### Apply migrations to the database

```sh
dotnet ef database update --startup-project src/TournamentPlanner.Api/TournamentPlanner.Api --context AuthDbContext
```
