using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using panelApi.DataAccess;
using panelApi.Repository;
using panelApi.Repository.IRepository;

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
            //services.AddScoped<IProductPropertyRepo, ProductPropertyRepo>();
            //services.AddScoped<IPropertyDesRepo, PropertyDescRepo>();
            //services.AddScoped<IPropertyCategoryRepo, PropertyCategoryRepo>();
            services.AddScoped<IContactRepo, ContactRepo>();
            services.AddScoped<ITagRepo, TagRepo>();
            services.AddScoped<IBlogRepo, BlogRepo>();
            services.AddScoped<IBlogTagRepo, BlogTagRepo>();
            services.AddScoped<ISliderRepo, SliderRepo>();
            services.AddScoped<IReferanceRepo, ReferanceRepo>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<ISubCategoryRepo, SubCategoryRepo>();
            services.AddScoped<IFeatureRepo, FeatureRepo>();
            services.AddScoped<IFeatureDescriptionRepo, FeatureDescriptionRepo>();
            services.AddScoped<IPr_FeDesc_RelRepo, Pr_FeDesc_RelRepo>();
            services.AddScoped<IPr_Fe_RelRepo, Pr_Fe_RelRepo>();
            services.AddScoped<IFe_SubCat_RelRepo, Fe_SubCat_RelRepo>();


        }
    }
}
