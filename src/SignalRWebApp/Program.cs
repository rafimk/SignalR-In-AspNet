using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using SignalRWebApp.Auth;
using SignalRWebApp.Commands;
using SignalRWebApp.Hubs;
using SignalRWebApp.Middlewares;
using SignalRWebApp.Providers;
using SignalRWebApp.Time;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IClock, UtcClock>();
builder.Services.AddAuth(builder.Configuration);

builder.Services.AddSignalR();

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    // cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
    // cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
});

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

builder.Services.AddCors();

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapPost("/notification", async (ProcessCommand command, ISender dispatcher) =>
{
    await dispatcher.Send(command);
    return Results.NoContent();
});

app.MapPost("/signIn", async (SignInCommand command, ISender dispatcher) =>
{
    var jwtToken = await dispatcher.Send(command);
    return Results.Ok(jwtToken);
});

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthorization();
app.UseAuthentication();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseMiddleware<ExtractUserInfoMiddleware>();
app.MapRazorPages();
app.MapHub<ConnectionHub>("notifications");

app.Run();
