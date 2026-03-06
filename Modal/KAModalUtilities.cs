using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Komponenty.Modal
{
    internal static class KAModalUtilities
    {
        public static KAModalService? GetModalService(this IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService(typeof(KAModalService)) is KAModalService modalService)
            {
                return modalService;
            }
            else
            {
                Console.Error.WriteLine($"A modal component could not resolve the {nameof(KAModalService)} service. Modals will not work correctly.");
            }

            return null;
        }
    }
}
