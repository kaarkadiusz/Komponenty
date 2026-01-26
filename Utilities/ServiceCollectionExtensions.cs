using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Komponenty.Utilities
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKomponentyJavascript(this IServiceCollection services)
        {
            services.AddScoped<KAJavascriptService>();
            return services;
        }
    }
}
