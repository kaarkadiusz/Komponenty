using Microsoft.AspNetCore.Components;
using System.Text;

namespace Komponenty.Modal
{
    public partial class KAModal : KAComponentBase, IAsyncDisposable
    {
        [CascadingParameter]
        internal Func<KAModalReference>? GetReferenceFunc { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Inject]
        private IServiceProvider ServiceProvider { get; set; } = null!;

        private KAModalService? ModalService { get; set; }

        public bool IsOpen { get; private set; }

        private Transition _currentTransition = Transition.None;
        private KAModalReference? ModalReference { get; set; }
        private bool IsServiceModal { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ModalService = ServiceProvider.GetModalService();
            await InitializeModalReference();
            await base.OnInitializedAsync();
        }
        private async Task InitializeModalReference()
        {
            if (GetReferenceFunc is not null)
            {
                ModalReference = GetReferenceFunc();
                IsServiceModal = true;
                await OpenAsync();
            }
            else if (ModalService is not null)
            {
                ModalReference = await ModalService.CreateKAModalReference();
            }
            ModalReference?.ModalRef = this;
        }

        protected override void AppendToCssClass(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("ka-modal");
            if (_currentTransition is not Transition.None)
            {
                stringBuilder.AppendLine($"ka-transition-{_currentTransition}");
            }
            base.AppendToCssClass(stringBuilder);
        }

        public async Task OpenAsync()
        {
            IsOpen = true;
            StateHasChanged();
            await PlayTransition(Transition.Enter);
        }
        public async Task CloseAsync()
        {
            if (ModalService is not null && ModalReference is not null)
            {
                await ModalService.CloseAsync(ModalReference);
            }
        }
        internal async Task CloseInternal()
        {
            await PlayTransition(Transition.Exit);
            IsOpen = false;
            StateHasChanged();
        }

        private CancellationTokenSource PlayTransitionCTS { get; set; } = new();
        private async Task PlayTransition(Transition transition)
        {
            PlayTransitionCTS.Cancel();
            PlayTransitionCTS = new();
            CancellationToken token = PlayTransitionCTS.Token;

            _currentTransition = transition;
            StateHasChanged();
            try
            {
                await Task.Delay(150, token);
            }
            catch (TaskCanceledException)
            {
                return;
            }
            _currentTransition = Transition.None;
        }

        public async ValueTask DisposeAsync()
        {
            if(ModalService is not null && ModalReference is not null)
            {
                await ModalService.DestroyKAModalReference(ModalReference);
            }
        }

        private enum Transition
        {
            None,
            Enter,
            Exit,
        }
    }
}
