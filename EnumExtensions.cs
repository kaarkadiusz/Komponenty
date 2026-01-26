using System;
using System.Collections.Generic;
using System.Text;

namespace Komponenty
{
    internal static class EnumExtensions
    {
        extension<T>(T value) where T : struct, Enum
        {
            /// <summary>
            /// Gets the current value of the enum or, if the current value is not defined, the default value for this type <see cref="T"/>.
            /// </summary>
            /// <returns>The current value of enum or the default value for this type <see cref="T"/>.</returns>
            public T GetValueOrDefault()
            {
                if (Enum.IsDefined(value))
                {
                    return value;
                }
                else
                {
                    return default;
                }
            }
        }
    }
}
