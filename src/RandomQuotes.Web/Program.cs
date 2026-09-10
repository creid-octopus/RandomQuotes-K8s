namespace RandomQuotes.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton(new Models.BrandingOptions{
                BrandName = !string.IsNullOrWhiteSpace(builder.Configuration["BRAND_NAME"]) ? builder.Configuration["BRAND_NAME"]! : "RandomQuotes",
                BrandColor = !string.IsNullOrWhiteSpace(builder.Configuration["BRAND_COLOR"]) ? builder.Configuration["BRAND_COLOR"]! : "#0d6efd",
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            Models.Quote.Initialize();

            app.Run();
        }
    }
}