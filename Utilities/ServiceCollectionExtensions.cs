using Komponenty.Modal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Komponenty.Utilities
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKAServices(this IServiceCollection services)
        {
            services
                .AddKomponentyJavascript()
                .AddModalService();
            return services;
        }
        public static IServiceCollection AddKomponentyJavascript(this IServiceCollection services)
        {
            services.AddScoped<KAJavascriptService>();
            return services;
        }
        public static IServiceCollection AddModalService(this IServiceCollection services)
        {
            services.AddScoped<KAModalService>();
            return services;
        }
    }
}
