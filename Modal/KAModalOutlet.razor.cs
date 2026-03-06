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
        private IServiceProvider ServiceProvider { get; set; } = null!;

        private KAModalService? ModalService { get; set; }

        private IReadOnlyList<KAModalReference> Modals => ModalService?.Modals ?? [];

        protected override Task OnInitializedAsync()
        {
            ModalService = ServiceProvider.GetModalService();
            ModalService?.ModalsChanged += StateHasChanged;
            return base.OnInitializedAsync();
        }

        
    }
}
