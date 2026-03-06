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

        protected override Task OnInitializedAsync()
        {
            ModalService = ServiceProvider.GetModalService();
            ModalService?.OnOpenRequest += Open;
            ModalService?.OnCloseRequest += Close;
            return base.OnInitializedAsync();
        }

        Stack<string> SectionNames { get; set; } = [];

        private Task Open(TaskCompletionSource<string> taskCompletionSource)
        {
            string sectionName = Guid.NewGuid().ToString();
            while (SectionNames.Contains(sectionName))
            {
                sectionName = Guid.NewGuid().ToString();
            }
            SectionNames.Push(sectionName);
            StateHasChanged();
            taskCompletionSource.SetResult(sectionName);
            return Task.CompletedTask;
        }
        private Task Close(string sectionName)
        {
            while(SectionNames.Count > 0 && SectionNames.Pop() != sectionName)
            {
                
            }
            StateHasChanged();
            return Task.CompletedTask;
        }
    }
}
