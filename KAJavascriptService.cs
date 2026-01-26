using Microsoft.JSInterop;

namespace Komponenty
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class KAJavascriptService(IJSRuntime jsRuntime) : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/Komponenty/KAJavascriptService.js").AsTask());

        public async Task ScrollToElement(string elementId)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("scrollToElement", elementId);
        }

        internal async Task<int> GetTextAreaLineCount(string textAreaId, string textAreaFakeId)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<int>("getTextAreaLineCount", textAreaId, textAreaFakeId);
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
