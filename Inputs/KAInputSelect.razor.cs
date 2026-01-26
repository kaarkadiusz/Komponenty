using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Komponenty.Inputs
{
    public partial class KAInputSelect<T> : KAInputBase<T>
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
        }

        private new object? ReadOnly { get; }
    }
}
