using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tp3.Data;
using Tp3.Repositories.Implementations;
using Tp3.Repositories.Interfaces;
using Tp3.Services.Interfaces;
using Tp3.Services.Implementations;
using Tp3.Middleware;
using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;


// Enable legacy timestamp behavior for Npgsql to avoid UTC issues
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext with PostgreSQL and Audit Interceptor
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
           .AddInterceptors(new AuditInterceptor())
);

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Register Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString);

// Register repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();

// Register services
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

// Register database seeder
builder.Services.AddScoped<DatabaseSeeder>();

var app = builder.Build();

// Handle database seeding
if (args.Contains("--seed") || args.Contains("--seed-reset"))
{
    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            if (args.Contains("--seed-reset"))
            {
                logger.LogInformation("Resetting database before seeding...");
                await seeder.ResetDatabaseAsync();
            }

            var hasData = await seeder.HasDataAsync();
            if (!hasData || args.Contains("--force"))
            {
                logger.LogInformation("Starting database seeding...");
                await seeder.SeedDataAsync();
                logger.LogInformation("Database seeding completed successfully");
            }
            else if (args.Contains("--seed"))
            {
                logger.LogInformation("Database already contains data. Use --seed-reset or --seed --force to override.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database seeding");
            Environment.Exit(1);
        }
    }

    // Exit after seeding
    Environment.Exit(0);
}

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseSerilogRequestLogging();

if (!app.Environment.IsDevelopment())
{
    // app.UseExceptionHandler("/Home/Error"); // Replaced by GlobalExceptionHandlerMiddleware
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Health Check endpoint with JSON UI output
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
