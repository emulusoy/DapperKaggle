using DapperKaggle.Context;
using DapperKaggle.Repositories.Abstact;
using DapperKaggle.Repositories.Concrete;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DapperKaggleContext>();
builder.Services.AddScoped<IClubsService, ClubsManager>();
builder.Services.AddScoped<IPlayersService, PlayersManager>();
builder.Services.AddScoped<IDashboardService, DashboardManager>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// Uygulama start’ýnda (Program.cs):


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}/{action=Dashboard}/{id?}");

app.Run();
