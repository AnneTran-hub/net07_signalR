using btb23.Hubs;
using btb23.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IProductService, ProductService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapHub<ProductHub>("/hubs/products");
app.MapFallbackToPage("/_Host");

app.Run();
