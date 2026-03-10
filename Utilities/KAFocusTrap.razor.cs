using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Komponenty.Utilities
{
    public partial class KAFocusTrap : IAsyncDisposable
    {
        [Parameter]
        public string? ElementId { get; set; }
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Inject]
        private KAJavascriptService JavascriptService { get; set; } = null!;

        private string GetElementId()
        {
            if(!string.IsNullOrEmpty(ElementId))
            {
                return ElementId;
            }
            return $"{nameof(KAFocusTrap)}-ElementId-{GetHashCode()}";
        }
        private CancellationTokenSource CTS { get; set; } = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                await JavascriptService.AddFocusTrap(GetElementId(), CTS.Token);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public ValueTask DisposeAsync()
        {
            CTS.Cancel();
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }

    }
}
