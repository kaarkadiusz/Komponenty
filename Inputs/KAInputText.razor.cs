using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Komponenty.Inputs
{
    public partial class KAInputText : KAInputBase<string>
    {
        [Parameter]
        public int? MaxLength { get; set; }

        public override Type[] ValidTypes => [
            typeof(string)
            ];

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
        }
    }
}
