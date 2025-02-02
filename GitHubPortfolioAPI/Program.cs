
using GitHubPortfolioAPI.CachedServises;
using GitHubService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.Configure<GitHubIntegretionOption>(builder.Configuration.GetSection("GitHubIntegretionOption"));

builder.Services.AddGitHubIntegretions(option => builder.Configuration.GetSection("GitHubIntegretionOption").Bind(option));

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IGitHubService, GitHubService.Services.GitHubService>();
builder.Services.Decorate<IGitHubService,CachedGitHubSerivice>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


