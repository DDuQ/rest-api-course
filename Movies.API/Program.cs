using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.API.Auth;
using Movies.API.Health;
using Movies.API.Mapping;
using Movies.API.Swagger;
using Movies.Application;
using Movies.Application.Database;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        ValidateIssuer = true,
        ValidateAudience = true
    };
});

builder.Services.AddAuthorization(x =>
{
    // x.AddPolicy(AuthConstants.AdminUserPolicyName, p =>
    //     p.RequireClaim(AuthConstants.AdminUserClaimName, "true"));
    
    x.AddPolicy(AuthConstants.AdminUserPolicyName, 
        p => p.AddRequirements(new AdminAuthRequirement(config["ApiKey"]!)));

    x.AddPolicy(AuthConstants.TrustedMemberPolicyName, p =>
        p.RequireAssertion(c =>
            c.User.HasClaim(m => m is { Type: AuthConstants.AdminUserClaimName, Value: "true" }) ||
            c.User.HasClaim(m => m is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" })));
});

builder.Services.AddScoped<ApiKeyAuthFilter>();

builder.Services.AddApiVersioning(x =>
{
    x.DefaultApiVersion = new ApiVersion(1);
    x.AssumeDefaultVersionWhenUnspecified = true;
    x.ReportApiVersions = true;
    x.ApiVersionReader = new HeaderApiVersionReader("api-version");
}).AddMvc().AddApiExplorer();

// builder.Services.AddResponseCaching(); -- Client-side response caching.
builder.Services.AddOutputCache(x =>
{
    x.AddBasePolicy(c => c.Cache());
    x.AddPolicy("MovieCache", c => 
        c.Cache()
        .Expire(TimeSpan.FromMinutes(1))
        .SetVaryByQuery(["title", "year", "sortBy", "page", "pageSize"])
        .Tag("movies"));
});

builder.Services.AddControllers();

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());

builder.Services.AddApplication();
builder.Services.AddDatabase(config["Database:ConnectionString"]!);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            x.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", 
                description.GroupName);
        }
    });
}

app.MapHealthChecks("_health");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

//app.UseCors(); -- This should go before response caching if used.
// app.UseResponseCaching(); -- Client-side response caching.
app.UseOutputCache();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();