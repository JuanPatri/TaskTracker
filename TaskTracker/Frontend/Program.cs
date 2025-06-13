using Repository;
using Domain;
using Frontend.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using Service;
using Service.ExportService;
using Task = Domain.Task;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

//builder.Services.AddSingleton<InMemoryDatabase>();
builder.Services.AddDbContext<SqlContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        providerOptions => providerOptions.EnableRetryOnFailure()
        )
    );

// Base de datos en memoria para el resto de entidades
builder.Services.AddSingleton<InMemoryDatabase>();

//repositories with bata base
builder.Services.AddScoped<IRepository<User>, UserRepository>(); 

//repositories
builder.Services.AddSingleton<IRepository<Task>, TaskRepository>();
builder.Services.AddSingleton<IRepository<Project>, ProjectRepository>();
builder.Services.AddSingleton<IRepository<Resource>, ResourceRepository>();
builder.Services.AddSingleton<IRepository<ResourceType>, ResourceTypeRepository>();
builder.Services.AddSingleton<IRepository<Notification>, NotificationRepository>();

//services with database
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SessionService>();

//services
builder.Services.AddSingleton<ProjectService>();
builder.Services.AddSingleton<TaskService>();
builder.Services.AddSingleton<ResourceService>();
builder.Services.AddSingleton<ResourceTypeService>();
builder.Services.AddSingleton<CriticalPathService>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddSingleton<ProjectCsvExporter>();
builder.Services.AddSingleton<ProjectJsonExporter>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();