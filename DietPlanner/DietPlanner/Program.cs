using Domain.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Services.AuthServices;
using System.Text;
using System.Net;
using Services.MealPlanServices;
using EasyCaching.Core.Configurations;
using System.Configuration;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;


namespace DietPlanner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddStackExchangeRedisCache(options =>
            {options.Configuration = "localhost:6379";
            });

            builder.Services.AddNotyf(config =>
            {
                config.DurationInSeconds = 10;
                config.IsDismissable = true;
                config.Position = NotyfPosition.TopCenter;
            }
);
            builder.Services.AddSession();

            builder.Services.AddTransient<Validation>();
            builder.Services.AddTransient<UserDetailRepository>();

            builder.Services.AddScoped<Services.Upload>();
            builder.Services.AddScoped<MealInfoSummarize>();
            builder.Services.AddScoped<MealDetailRepository>();

            builder.Services.AddControllersWithViews();
            
            //Adding Dbcontext 
            builder.Services.AddDbContext<DietContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //adding radis caching
            

            //Adding Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => 
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts( );
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseNotyf();
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        // For API requests, return the unauthorized message
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync("Please log in to access this resource.");
                    }
                    else
                    {
                        string loginUrl = $"/Auth/SignIn?returnUrl={context.Request.Path.Value}&message=Please log in to access this resource.";
                        context.Response.Redirect(loginUrl);
                    }
                }
            });
            app.Use(async (context, next) =>
            {
                var JWTokenCookie = context.Request.Cookies["JwtToken"];
                if (!string.IsNullOrEmpty(JWTokenCookie))
                {
                    context.Request.Headers.Add("Authorization", "Bearer " + JWTokenCookie);
                }
                await next();
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=SignIn}/{id?}");

            app.Run();
        }
    }
}
