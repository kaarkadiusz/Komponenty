using Microsoft.AspNetCore.Components;
using System.Text;

namespace Komponenty.Modal
{
    public partial class KAModal : KAComponentBase
    {
        [CascadingParameter]
        internal KAModalReference? ModalReference { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Inject]
        private IServiceProvider ServiceProvider { get; set; } = null!;

        public bool IsOpen => ModalReference is not null;

        private KAModalService? ModalService { get; set; }

        private Transition _currentTransition = Transition.None;

        protected override async Task OnInitializedAsync()
        {
            ModalService = ServiceProvider.GetModalService();
            if (ModalReference is not null)
            {
                await OpenAsync();
            }
            await base.OnInitializedAsync();
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
            if (await GetModalReferenceAsync() is KAModalReference modalReference)
            {
                await PlayTransition(Transition.Enter);
            }
        }
        private async Task<KAModalReference?> GetModalReferenceAsync(bool createNew = true)
        {
            if (ModalReference is not null)
            {
                return ModalReference;
            }
            else if (createNew)
            {
                KAModalReference? modalReference = await (ModalService?.Open() ?? Task.FromResult<KAModalReference?>(null));
                ModalReference = modalReference;
                ModalReference?.ModalRef = this;
                return ModalReference;
            }
            return null;
        }
        public async Task CloseAsync()
        {
            if (await GetModalReferenceAsync(false) is KAModalReference modalReference)
            {
                await (ModalService?.Close(modalReference) ?? Task.CompletedTask);
                ModalReference = null;
            }
        }
        public async Task PlayExit()
        {
            await PlayTransition(Transition.Exit);
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

        private enum Transition
        {
            None,
            Enter,
            Exit,
        }
    }
}
