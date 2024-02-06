﻿using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Repositories.EFCore;

namespace WebApi.Extensions
{
    public static class ServicesExtensions
    {

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
         //hangi tipi genişletmek istiyorsak onu this anahtar kelimesiyle vermek gerekiyor
            => services.AddDbContext<RepositoryContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("sqlConnection")));

        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        
    }
}
