using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Komponenty.Modal
{
    public partial class KAModalOutlet
    {
        internal const string SecitonOutletName = "ka-modal-section-outlet";

        [Inject]
        private KAModalService ModalService { get; set; } = null!;

        private IReadOnlyList<KAModalReference> Modals => ModalService?.Modals ?? [];

        protected override Task OnInitializedAsync()
        {
            ModalService.ModalsChanged += StateHasChanged;
            return base.OnInitializedAsync();
        }

        private KAModalOptions GetModalOptions(KAModalReference modalReference) => modalReference.ModalOptions ?? _defaultOptions;
        private static readonly KAModalOptions _defaultOptions = new();
    }
}
