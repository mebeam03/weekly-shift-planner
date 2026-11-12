using ShiftScheduler.Components;
using ShiftScheduler.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton(new ScheduleStore(
    Path.Combine(builder.Environment.ContentRootPath, "schedule-data.json")));

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
