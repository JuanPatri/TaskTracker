using Repository;
using Domain;
using Frontend.Components;
using Microsoft.EntityFrameworkCore;
using Service;
using Service.ExportService;
using Task = Domain.Task;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

//bbd
builder.Services.AddDbContext<SqlContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        providerOptions => providerOptions.EnableRetryOnFailure()
    )
);

// Repositories 
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IRepository<Project>, ProjectRepository>();

builder.Services.AddSingleton<IRepository<Task>, TaskRepository>();
builder.Services.AddSingleton<IRepository<Resource>, ResourceRepository>();
builder.Services.AddSingleton<IRepository<ResourceType>, ResourceTypeRepository>();
builder.Services.AddSingleton<IRepository<Notification>, NotificationRepository>();

//services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<TaskService>();   
builder.Services.AddScoped<ResourceService>();          
builder.Services.AddScoped<CriticalPathService>();      
builder.Services.AddScoped<NotificationService>();     

builder.Services.AddSingleton<ResourceTypeService>();
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