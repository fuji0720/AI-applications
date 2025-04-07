using AI_applications.Controllers;
using AI_applications.Services;

namespace AI_applications
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.MyConfig();

            builder.Services.AddControllersWithViews();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
                config.AddDebug();
            });

            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            await Console(app);

            app.Run();
        }

        private static async Task Console(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var consoleController = scope.ServiceProvider.GetRequiredService<ConsoleController>();
            await consoleController.Index();
        }
    }
}
