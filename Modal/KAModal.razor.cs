using Microsoft.AspNetCore.Components;
using System.Text;

namespace Komponenty.Modal
{
    public partial class KAModal : KAComponentBase
    {
        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Inject]
        private IServiceProvider ServiceProvider { get; set; } = null!;

        private KAModalService? ModalService { get; set; }
        private Transition _currentTransition = Transition.None;

        protected override Task OnInitializedAsync()
        {
            ModalService = ServiceProvider.GetModalService();
            return base.OnInitializedAsync();
        }

        protected override void AppendToCssClass(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("ka-modal");
            if(_currentTransition is not Transition.None)
            {
                stringBuilder.AppendLine($"ka-transition-{_currentTransition}");
            }
            base.AppendToCssClass(stringBuilder);
        }

        public bool IsOpen => !string.IsNullOrEmpty(_sectionName);
        private string _sectionName = string.Empty;

        public async Task OpenAsync()
        {
            _sectionName = await (ModalService?.Open() ?? Task.FromResult(string.Empty));
            await PlayTransition(Transition.Enter);
        }
        public async Task CloseAsync()
        {
            await PlayTransition(Transition.Exit);
            if(!string.IsNullOrEmpty(_sectionName))
            {
                await (ModalService?.Close(_sectionName) ?? Task.CompletedTask);
            }
            _sectionName = string.Empty;
        }

        private CancellationTokenSource PlayTransitionCTS { get; set; } = new();
        private async Task PlayTransition(Transition transition)
        {
            PlayTransitionCTS.Cancel();
            PlayTransitionCTS = new();
            CancellationToken token = PlayTransitionCTS.Token;

            _currentTransition = transition;
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
