using Application.Common.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("CleanArchitectureDb"));
            }
            else
            {
                if (configuration.GetValue<bool>("UseSQLite"))
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.ConfigureWarnings(options => options.Ignore(RelationalEventId.AmbientTransactionWarning));
                        options.UseSqlite(@"Data Source=E:\ICMS.db", sqlLiteOptions => sqlLiteOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                    });
                }
                else
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.EnableSensitiveDataLogging();
                        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                    });

                }
            }



            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());


            return services;
        }
    }
}
