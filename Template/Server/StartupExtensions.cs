using Microsoft.OpenApi.Models;
using Template.Core.Application;
using Template.Core.Application.Contracts;
using Template.Infrastructure.Identity;
using Template.Infrastructure.Persistance;
//using Template.Infrastructure.Persistance.Dapper;
using Template.Server.Services;

namespace Template.Server
{
    public static class StartupExtensions
    {
        public static WebApplication ConfigureServices(
        this WebApplicationBuilder builder)
        {
            AddSwagger(builder.Services);

            builder.Services.AddApplicationServices();
            //builder.Services.AddInfrastructureServices(builder.Configuration);
            //builder.Services.AddPersistenceServices(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<ILoggedInUserService, LoggedInUserService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            return builder.Build();

        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {

            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Template API");
                });
            }

            app.UseAuthentication();
            //app.UseCustomExceptionHandler();
            app.UseCors("Open");
            app.UseAuthorization();

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();


            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            return app;

        }
        private static void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                //      Enter 'Bearer' [space] and then your token in the text input below.
                //      \r\n\r\nExample: 'Bearer 12345abcdef'",
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.ApiKey,
                //    Scheme = "Bearer"
                //});

                //c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                //  {
                //    {
                //      new OpenApiSecurityScheme
                //      {
                //        Reference = new OpenApiReference
                //          {
                //            Type = ReferenceType.SecurityScheme,
                //            Id = "Bearer"
                //          },
                //          Scheme = "oauth2",
                //          Name = "Bearer",
                //          In = ParameterLocation.Header,

                //        },
                //        new List<string>()
                //      }
                //    });

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "GloboTicket Ticket Management API",

                });
            });
        }

        //public static async Task ResetDatabaseAsync(this WebApplication app)
        //{
        //    using var scope = app.Services.CreateScope();
        //    try
        //    {
        //        var context = scope.ServiceProvider.GetService<GloboTicketDbContext>();
        //        if (context != null)
        //        {
        //            await context.Database.EnsureDeletedAsync();
        //            await context.Database.MigrateAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
        //        logger.LogError(ex, "An error occurred while migrating the database.");
        //    }
        //}
    }
}



//if (app.Environment.IsDevelopment())
//{
//    app.UseWebAssemblyDebugging();
//}
//else
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}