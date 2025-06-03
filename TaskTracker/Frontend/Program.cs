using Repository;
using Domain;
using Frontend.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using Service;
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

//repositories
builder.Services.AddSingleton<IRepository<User>, UserRepository>(); 
builder.Services.AddSingleton<IRepository<Task>, TaskRepository>();
builder.Services.AddSingleton<IRepository<Project>, ProjectRepository>();
builder.Services.AddSingleton<IRepository<Resource>, ResourceRepository>();
builder.Services.AddSingleton<IRepository<ResourceType>, ResourceTypeRepository>();
builder.Services.AddSingleton<IRepository<Notification>, NotificationRepository>();

//services
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<SessionService>();
builder.Services.AddSingleton<ProjectService>();
builder.Services.AddSingleton<TaskService>();


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