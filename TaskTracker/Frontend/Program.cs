using Backend.Service;
using Backend.Repository;
using Backend.Domain;
using Frontend.Components;
using Task = Backend.Domain.Task;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

//repositories
builder.Services.AddSingleton<IRepository<User>, UserRepository>(); 
builder.Services.AddSingleton<IRepository<Task>, TaskRepository>();
builder.Services.AddSingleton<IRepository<Project>, ProjectRepository>();
builder.Services.AddSingleton<IRepository<Resource>, ResourceRepository>();
builder.Services.AddSingleton<IRepository<ResourceType>, ResourceTypeRepository>();

//services
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<SessionService>();
builder.Services.AddSingleton<TaskService>();
builder.Services.AddSingleton<ResourceService>();
builder.Services.AddSingleton<ResourceTypeService>();

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