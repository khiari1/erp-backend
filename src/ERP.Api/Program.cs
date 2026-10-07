using ERP.Api.Features.Categories;
using ERP.Api.Features.Produits;
using ERP.Api.Features.Auth;
using ERP.Api.Features.Utilisateurs;
using ERP.Api.Features.Fournisseurs;
using ERP.Api.Features.Achats;
using ERP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ErpDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ErpDatabase")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ERP API - Phase 1", Version = "v1" });

    // Ajoute le bouton "Authorize" dans Swagger pour coller le token Bearer
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Entrer uniquement le token JWT (sans le mot 'Bearer')",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDevServer", policy =>
    {
        policy.WithOrigins(builder.Configuration["Cors:AngularDevServer"] ?? "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ---- Authentification JWT ----
var jwtConfig = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AngularDevServer");
app.UseHttpsRedirection();

app.UseAuthentication();   // doit etre avant UseAuthorization
app.UseAuthorization();

app.MapCategoriesEndpoints();
app.MapProduitsEndpoints();
app.MapAuthEndpoints();
app.MapUtilisateursEndpoints();
app.MapFournisseursEndpoints();
app.MapCommandesFournisseurEndpoints();
app.MapReceptionsEndpoints();

app.MapGet("/api/health/db", async (ErpDbContext db) =>
{
    var peutSeConnecter = await db.Database.CanConnectAsync();
    var nbCategories = peutSeConnecter ? await db.Categories.CountAsync() : 0;
    return Results.Ok(new { connecteALaBase = peutSeConnecter, nombreCategories = nbCategories });
});

app.Run();