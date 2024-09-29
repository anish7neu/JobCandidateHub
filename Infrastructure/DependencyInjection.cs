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
            services.AddDbContext<ApplicationDbContext>(options =>
                                                            options.UseInMemoryDatabase("CandidateDb"));


            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());


            return services;
        }
    }
}
