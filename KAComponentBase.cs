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

        protected string CssClass => GetCssClass();

        protected virtual void AppendToCssClass(StringBuilder stringBuilder) { }

        protected virtual string GetCssClass()
        {
            StringBuilder stringBuilder = new();

            if (Attributes?.ContainsKey("class") ?? false)
            {
                stringBuilder.AppendLine(Attributes["class"].ToString());
            }

            AppendToCssClass(stringBuilder);

            return stringBuilder.ToString().Replace('\n', ' ');
        }
    }
}
