using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Runtime.InteropServices;
using System.Text;

namespace Komponenty.Modal
{
    public partial class KAModal : KAComponentBase, IKAModalOptions, IAsyncDisposable
    {
        [CascadingParameter]
        internal Func<KAModalReference>? GetReferenceFunc { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        [Parameter]
        public string? Title { get; set; }
        [Parameter]
        public RenderFragment? HeaderFragment { get; set; }
        [Parameter]
        public RenderFragment? FooterFragment { get; set; }
        [Parameter]
        public RenderFragment? Actions { get; set; }
        [Parameter]
        public KAModalWidth Width { get; set; } = KAModalWidth.Medium;
        [Parameter]
        public bool ShowCloseButton { get; set; } = true;
        [Parameter]
        public bool CloseOnBackdropClick { get; set; } = true;
        [Parameter]
        public bool CloseOnEscapeKey { get; set; } = true;
        [Parameter]
        public bool KeepMounted { get; set; }

        [Inject]
        private KAModalService ModalService { get; set; } = null!;

        public bool IsOpen { get; private set; }

        private Transition _currentTransition = Transition.None;
        private KAModalReference? _modalReference;

        private string WrapperElementId => $"{nameof(KAModal)}-{nameof(WrapperElementId)}-{GetHashCode()}";
        private ElementReference? WrapperElementReference { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitializeModalReference();
            await base.OnInitializedAsync();
        }

        private async Task InitializeModalReference()
        {
            if (GetReferenceFunc is not null)
            {
                _modalReference = GetReferenceFunc();
                await OpenAsync();
            }
            else if (ModalService is not null)
            {
                _modalReference = await ModalService.CreateKAModalReference();
            }
            _modalReference?.ModalRef = this;
        }

        protected override void AppendToCssClass(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("ka-modal-wrapper");
            if (_currentTransition is not Transition.None)
            {
                stringBuilder.AppendLine($"ka-modal-transition-{_currentTransition}");
            }
            stringBuilder.AppendLine($"ka-modal-width-{Width}");
            if(!IsOpen && KeepMounted)
            {
                stringBuilder.AppendLine($"ka-modal-hidden");
            }
            base.AppendToCssClass(stringBuilder);
        }

        public async Task OpenAsync()
        {
            IsOpen = true;
            StateHasChanged();
            await PlayTransition(Transition.Enter);
            await (WrapperElementReference?.FocusAsync() ?? ValueTask.CompletedTask);
        }
        public async Task CloseAsync()
        {
            if (_modalReference is not null)
            {
                await ModalService.CloseAsync(_modalReference);
            }
        }
        internal async Task CloseInternal()
        {
            await PlayTransition(Transition.Exit);
            IsOpen = false;
            StateHasChanged();
        }
        internal async Task FocusInternal()
        {
            await (WrapperElementReference?.FocusAsync() ?? ValueTask.CompletedTask);
        }

        private async Task BackdropClicked()
        {
            if(CloseOnBackdropClick)
            {
                await CloseAsync();
            }
        }
        private async Task KeyPressed(KeyboardEventArgs args)
        {
            if(args.Key == "Escape" && CloseOnEscapeKey)
            {
                await CloseAsync();
            }
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
            if(_modalReference is not null)
            {
                await ModalService.DestroyKAModalReference(_modalReference);
            }
        }

        private enum Transition
        {
            None,
            Enter,
            Exit,
        }
    }

    public enum KAModalWidth
    {
        Unset,
        Small,
        Medium,
        Large,
    }

}
