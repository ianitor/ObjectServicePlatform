using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel;
using Ianitor.Common.Shared;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Backend.Persistence.MongoDb;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using Ianitor.Osp.Backend.Persistence.SystemStores;
using MongoDB.Bson;
using IdentityServiceCollectionExtensions = Microsoft.AspNetCore.Identity.IdentityBuilderExtensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOspPersistence(
            this IServiceCollection services,
            Action<OspSystemConfiguration> setupSystemConfigurationAction = null,
            Action<IdentityOptions> setupAction = null)
        {
            ArgumentValidation.Validate(nameof(services), services);

            if (setupSystemConfigurationAction != null)
            {
                services.Configure(setupSystemConfigurationAction);
            }


            services.AddSingleton<ISystemContext, SystemContext>();
            services.AddTransient<IOspClientStore, ClientStore>();
            services.AddScoped<IOspResourceStore, ResourceStore>();
            services.AddScoped<IOspPersistentGrantStore, PersistentGrantStore>();
            services.AddScoped<IOspIdentityProviderStore, IdentityProviderStore>();

            AddIdentity(services, setupAction);

            return services;
        }

        private static void AddIdentity(IServiceCollection services, Action<IdentityOptions> setupAction)
        {
            var builder = services
                .AddIdentity<OspUser, OspRole>(setupAction)
                .AddRoleStore<OspRoleStore>()
                .AddUserStore<OspUserStore>()
                .AddUserManager<UserManager<OspUser>>()
                .AddRoleManager<RoleManager<OspRole>>()
                .AddDefaultTokenProviders();

            // register custom ObjectId TypeConverter
            RegisterTypeConverter<ObjectId, ObjectIdConverter>();

            builder.Services.AddTransient(
                typeof(IRoleStore<>).MakeGenericType(builder.RoleType), typeof(OspRoleStore));

            builder.Services.AddTransient(
                typeof(IUserStore<>).MakeGenericType(builder.UserType), typeof(OspUserStore));
        }
        
        private static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
        {
            Attribute[] attr = new Attribute[1];
            TypeConverterAttribute vConv = new TypeConverterAttribute(typeof(TC));
            attr[0] = vConv;
            TypeDescriptor.AddAttributes(typeof(T), attr);
        }
    }
}