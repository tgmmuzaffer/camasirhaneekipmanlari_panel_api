using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using panelApi.DataAccess;
using panelApi.Repository;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.RepoExtension
{
    public static class RepoExtensions
    {
        public static void ConfigureSqlServer(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration["ConnectionStrings:PanelApiConnection"];
            services.AddDbContext<PanelApiDbcontext>(options => options.UseSqlServer(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        }
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IRoleRepo, RoleRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IProductRepo, ProductRepo>();
            services.AddScoped<ICategoryRepo, CategoryRepo>();
            services.AddScoped<IProductPropertyRepo, ProductPropertyRepo>();
            services.AddScoped<IPropertyDesRepo, PropertyDescRepo>();
            services.AddScoped<IPropertyCategoryRepo, PropertyCategoryRepo>();
            services.AddScoped<IContactRepo, ContactRepo>();
            services.AddScoped<ITagRepo, TagRepo>();

        }
    }
}
