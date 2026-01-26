using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Komponenty
{
    public abstract class KAComponentBase : ComponentBase
    {
        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object>? Attributes { get; set; }

        protected virtual string GetCssClass()
        {
            if (Attributes?.ContainsKey("class") ?? false)
            {
                return Attributes["class"].ToString() ?? string.Empty;
            }
            return string.Empty;
        }
    }
}
