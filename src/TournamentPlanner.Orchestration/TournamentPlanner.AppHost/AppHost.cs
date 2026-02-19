var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("postgres-password", secret: true);

var postgres = builder.AddPostgres("postgres", password: postgresPassword)
    .WithHostPort(5432)
    .WithDataVolume()
    .WithPgAdmin();

var db = postgres.AddDatabase("tournamentdb");

builder.AddProject<Projects.TournamentPlanner_Api>("tournamentplanner-api")
    .WithReference(db)
    .WaitFor(db)
    .WithUrlForEndpoint("https", url =>
    {
        url.DisplayText = "Scalar (HTTPS)";
        url.Url = "/scalar";
    })
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar";
        url.Url = "/scalar";
    });

builder.Build().Run();
