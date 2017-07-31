using Microsoft.Extensions.DependencyInjection;
using Web.DataAccess.IRepository;
using Web.DataAccess.Repository;
using Web.Model.Entities;
using Web.Services.EntitiesServices;

namespace RedMan.Extensions {
    public static class ServiceExtension
    {
        public static IServiceCollection AddMyIdentity(this IServiceCollection services)
        {
            services.AddTransient<IIdentityRepository<User>, IdentityRepository<User>>();
            services.AddTransient<IdentityService>();
            return services;
        }
    }
}
