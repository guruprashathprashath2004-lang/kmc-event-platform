using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using KMC_API_Guruprashath.Data;
using KMC_API_Guruprashath.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Raise the multipart/form body size limits explicitly so a normal photo upload
// (a few MB) is never silently rejected or aborted at the Kestrel/parsing level.
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(opt =>
{
    opt.MultipartBodyLengthLimit = 20 * 1024 * 1024; // 20 MB
    opt.ValueLengthLimit = int.MaxValue;
    opt.MultipartHeadersLengthLimit = int.MaxValue;
});
builder.WebHost.ConfigureKestrel(opt =>
{
    opt.Limits.MaxRequestBodySize = 20 * 1024 * 1024; // 20 MB
});

//Read the connection string
var conn = builder.Configuration.GetConnectionString("conn");
builder.Services.AddDbContext<AppDBContext>(opt => opt.UseSqlServer(conn, sql => sql.EnableRetryOnFailure()));

//All the Repo classes must be listed down like below
builder.Services.AddScoped<CategoryRepo>();
builder.Services.AddScoped<EventRepo>();
builder.Services.AddScoped<RegistrationRepo>();
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());

// Allow the KMC MVC client (running on a different port) to call this API.
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowClient", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// JWT authentication for the Admin-only endpoints (create/update/delete event & category, view registrations).
var jwtKey = builder.Configuration["Jwt:Key"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
        };
    });
builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Seed the database with an admin account, categories and sample KMC events on startup.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    DbSeeder.Seed(db);
}

// Configure the HTTP request pipeline.

// Global safety net: catches ANY unhandled exception anywhere in the pipeline (not just inside
// one controller's try/catch) and returns it as a visible JSON error instead of letting the
// process crash or the request hang silently.
app.UseExceptionHandler(errApp =>
{
    errApp.Run(async context =>
    {
        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var ex = feature?.Error;
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;
        var detail = ex?.InnerException?.Message ?? ex?.Message ?? "Unknown server error.";
        await context.Response.WriteAsJsonAsync(new { error = "Unhandled server error", detail });
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //To use Scalar.
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // serves /images/events (event poster uploads)

app.UseCors("AllowClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
