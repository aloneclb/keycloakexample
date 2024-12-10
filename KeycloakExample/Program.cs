using KeycloakExample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<OptionsManager>();
//builder.Services.Configure<IdentityServerOption>(builder.Configuration.GetSection("IdentityServer")); options pattern

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//OptionsManager.Initialize(builder.Configuration);


app.MapGet("/", (OptionsManager options) =>
{
    return options.GetIdentityServer().HostName;
});

app.Run();
