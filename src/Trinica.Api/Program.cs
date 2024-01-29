using Microsoft.AspNetCore.Authentication.JwtBearer;
using Trinica.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions =>
{
    jwtOptions.Authority = builder.Configuration["Authentication:ValidIssuer"];
    jwtOptions.Audience = builder.Configuration["Authentication:Audience"];
    jwtOptions.TokenValidationParameters.ValidIssuer = builder.Configuration["Authentication:ValidIssuer"];
});
builder.Services.InitializeApp(builder.Environment);
builder.Services.AddHealthChecks()
    .AddCheck<ApiHealthCheck>("Sample");

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/healthz");
app.Run();
