using Microsoft.AspNetCore.Components;

namespace Komponenty.Modal
{
    public class KAModalService
    {
        internal delegate Task<KAModalReference?> OpenHandler_OpenThroughModal();
        internal delegate Task<KAModalReference?> OpenHandler_OpenThroughService(Type componentType, IDictionary<string, object>? componentParameters = null);
        internal delegate Task CloseHandler(KAModalReference modalReference);

        internal event OpenHandler_OpenThroughModal? OpenThroughModalRequested;
        internal event OpenHandler_OpenThroughService? OpenThroughServiceRequested;
        internal event CloseHandler? OnCloseRequest;

        public event Action? OnModalClosed;

        internal async Task<KAModalReference?> Open()
        {
            KAModalReference? modalReference = await (OpenThroughModalRequested?.Invoke() ?? Task.FromResult<KAModalReference?>(null));
            return modalReference;
        }
        internal async Task Close(KAModalReference modalReference)
        {
            await (OnCloseRequest?.Invoke(modalReference) ?? Task.CompletedTask);
        }

        public async Task<KAModalReference?> OpenAsync(Type componentType, IDictionary<string, object>? componentParameters = null)
        {
            KAModalReference? modalReference = await (OpenThroughServiceRequested?.Invoke(componentType, componentParameters) ?? Task.FromResult<KAModalReference?>(null));
            return modalReference;
        }
        public async Task<KAModalReference?> OpenAsync<T>(IDictionary<string, object>? componentParameters = null) where T : ComponentBase
        {
            return await OpenAsync(typeof(T), componentParameters);
        }
        public async Task CloseAsync(KAModalReference modalReference)
        {
            if(modalReference.ModalRef is not null)
            {
                await modalReference.ModalRef.CloseAsync();
            }
            else
            {
                await Close(modalReference);
            }
        }
    }
}
