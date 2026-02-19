var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter(
    "postgres-password",
    value: builder.Configuration["POSTGRES_PASSWORD"] ?? "postgres",
    secret: false);

var postgres = builder.AddPostgres("postgres", password: postgresPassword)
    .WithDataVolume()
    .WithPgAdmin();

var db = postgres.AddDatabase("tournamentdb");

var api = builder.AddProject<Projects.TournamentPlanner_Api>("tournamentplanner-api")
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

builder.AddJavaScriptApp("tournamentplanner-web", "../../TournamentPlanner.Web", "start")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(targetPort: 4200)
    .WithExternalHttpEndpoints();

builder.Build().Run();
