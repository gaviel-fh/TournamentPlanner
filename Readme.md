## Migrations

### Add a new migration

```sh
dotnet ef migrations add <MigrationName> --project src/TournamentPlanner.Auth/TournamentPlanner.Auth --startup-project src/TournamentPlanner.Api/TournamentPlanner.Api --context AuthDbContext --output-dir Data/Migrations
```

### Apply migrations to the database

```sh
dotnet ef database update --startup-project src/TournamentPlanner.Api/TournamentPlanner.Api --context AuthDbContext
```
