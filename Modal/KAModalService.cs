using System;
using System.Collections.Generic;
using System.Text;

namespace Komponenty.Modal
{
    public class KAModalService
    {
        internal delegate Task OpenHandler(TaskCompletionSource<string> taskCompletionSource);
        internal delegate Task CloseHandler(string sectionName);

        internal event OpenHandler? OnOpenRequest;
        internal event CloseHandler? OnCloseRequest;

        public event Action? OnModalClosed;

        internal async Task<string> Open()
        {
            TaskCompletionSource<string> tcs = new();
            await (OnOpenRequest?.Invoke(tcs) ?? Task.CompletedTask);
            await tcs.Task;
            return tcs.Task.Result;
        }
        internal async Task Close(string sectionName)
        {
            await (OnCloseRequest?.Invoke(sectionName) ?? Task.CompletedTask);
        }
    }
}
