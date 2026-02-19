var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TournamentPlanner_Api>("tournamentplanner-api")
    .WithUrlForEndpoint("https", url =>
    {
        url.DisplayText = "Scalar (HTTPS)";
        url.Url = "/scalar";
    })
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar";
        url.Url = "/scalar";
    }); ;

builder.Build().Run();
