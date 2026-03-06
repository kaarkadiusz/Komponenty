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

        Stack<KAModalReference> Modals { get; set; } = [];

        protected override Task OnInitializedAsync()
        {
            ModalService = ServiceProvider.GetModalService();
            ModalService?.OpenThroughModalRequested += OpenThroughModal;
            ModalService?.OpenThroughServiceRequested += OpenThroughService;
            ModalService?.OnCloseRequest += Close;
            return base.OnInitializedAsync();
        }

        private Task<KAModalReference?> OpenThroughModal()
        {
            string sectionName = GetNewGUID();

            KAModalReference modalReference = new(sectionName);
            Modals.Push(modalReference);
            StateHasChanged();

            return Task.FromResult<KAModalReference?>(modalReference);
        }
        private async Task<KAModalReference> OpenThroughService(Type componentType, IDictionary<string, object>? componentParameters = null)
        {
            string sectionName = GetNewGUID();

            KAModalReference modalReference = new(sectionName, componentType, componentParameters);
            Modals.Push(modalReference);
            StateHasChanged();
            if(modalReference.ModalRef is not null)
            {
                await modalReference.ModalRef.OpenAsync();
            }

            return modalReference;
        }
        private string GetNewGUID()
        {
            string guid = Guid.NewGuid().ToString();
            while (Modals.Any(modal => Equals(modal.SectionName, guid)))
            {
                guid = Guid.NewGuid().ToString();
            }
            return guid;
        }
        private async Task Close(KAModalReference modalReference)
        {
            if(!Modals.Contains(modalReference))
            {
                return;
            }

            List<Task> closeTasks = [];
            foreach(KAModalReference curr in Modals)
            {
                closeTasks.Add(curr.ModalRef?.PlayExit() ?? Task.CompletedTask);
                if (curr == modalReference)
                {
                    break;
                }
            }
            await Task.WhenAll(closeTasks);
            while (Modals.Count > 0)
            {
                KAModalReference curr = Modals.Pop();
                StateHasChanged();
                if(curr == modalReference)
                {
                    break;
                }
            }
        }
    }
}
