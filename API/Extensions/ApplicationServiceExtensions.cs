using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using API.Helpers;
using API.SignalR;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddAplicationServices(this IServiceCollection services,IConfiguration config)
        {
            
             services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
             services.AddScoped<ITokenService,TokenService>();
             services.AddScoped<IPhotoService,PhotoService>();

             services.AddScoped<LogUserActivity>();
             
             services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
             services.AddSignalR();
             services.AddSingleton<PresenceTracker>();  

             services.AddScoped<IUnitofWork,UnitOfWork>();


            services.AddDbContext<DataContext>(options => {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });

            return services;

        }
    }
}