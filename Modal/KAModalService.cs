using Microsoft.AspNetCore.Components;

namespace Komponenty.Modal
{
    public class KAModalService
    {
        internal event Action? ModalsChanged;

        internal IReadOnlyList<KAModalReference> Modals => _modals;
        private List<KAModalReference> _modals = [];

        public async Task<KAModalReference?> OpenAsync(Type componentType, IDictionary<string, object>? componentParameters = null)
        {
            return await CreateKAModalReference(componentType, componentParameters);
        }
        public async Task<KAModalReference?> OpenAsync<T>(IDictionary<string, object>? componentParameters = null) where T : ComponentBase
        {
            return await OpenAsync(typeof(T), componentParameters);
        }
        public async Task CloseAsync(KAModalReference modalReference)
        {
            await CloseModalTree(modalReference);
            if(modalReference.IsDynamicComponent)
            {
                await DestroyKAModalReference(modalReference);
            }
        }

        internal async Task<KAModalReference> CreateKAModalReference()
        {
            return await CreateKAModalReference(null, null);
        }
        internal async Task DestroyKAModalReference(KAModalReference modalReference)
        {
            await RemoveModalTree(modalReference);
        }

        private async Task<KAModalReference> CreateKAModalReference(Type? componentType = null, IDictionary<string, object>? componentParameters = null)
        {
            KAModalReference modalReference = new(GetNewSectionName(), componentType, componentParameters);
            _modals.Add(modalReference);
            ModalsChanged?.Invoke();
            return modalReference;
        }
        private async Task CloseModalTree(KAModalReference modalReference)
        {
            int modalReferenceIdx = _modals.IndexOf(modalReference);
            if(modalReferenceIdx == -1)
            {
                return;
            }

            await Task.WhenAll(_modals[modalReferenceIdx..].Select(modalReference => modalReference.ModalRef?.CloseInternal() ?? Task.CompletedTask));
            await (_modals.LastOrDefault(modal => modal.ModalRef?.IsOpen ?? false)?.ModalRef?.FocusInternal() ?? Task.CompletedTask);
        }
        private async Task RemoveModalTree(KAModalReference modalReference)
        {
            int modalReferenceIdx = _modals.IndexOf(modalReference);
            if (modalReferenceIdx == -1)
            {
                return;
            }

            _modals.RemoveRange(modalReferenceIdx, _modals.Count - modalReferenceIdx);
            ModalsChanged?.Invoke();
        }

        private string GetNewSectionName()
        {
            string guid = Guid.NewGuid().ToString();
            while (_modals.Any(modal => Equals(modal.SectionName, guid)))
            {
                guid = Guid.NewGuid().ToString();
            }
            return guid;
        }
    }
}
