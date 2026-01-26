using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Komponenty.Inputs
{
    public partial class KAInputTextArea : KAInputBase<string>
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;
        private KAJavascriptService JavascriptService { get; set; } = null!;

        [Parameter]
        public int? MaxLength { get; set; }
        [Parameter]
        public int? Rows { get; set; }
        [Parameter]
        public int? MaxRows { get; set; }
        [Parameter]
        public bool AllowGrowing { get; set; }

        public override Type[] ValidTypes => [
            typeof(string)
            ];

        private const string TextAreaFakeIdAttribute = "kompInputTextAreaFake";

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            CurrentRows = Rows ?? 2;
            JavascriptService = new(JSRuntime);
            await ClearRefreshCurrentRowsCooldown();
        }

        private int CurrentRows
        {
            get
            {
                return field;
            }
            set
            {
                int lowerBound = Rows ?? 2;
                int upperBound = MaxRows ?? int.MaxValue;
                field = Math.Clamp(value, lowerBound, upperBound);
            }
        }

        private TaskCompletionSource RefreshCurrentRowsTCS { get; set; } = new();
        private CancellationTokenSource RefreshCurrentRowsCTS { get; set; } = new();
        private async Task RefreshCurrentRows()
        {
            if (!AllowGrowing)
            {
                return;
            }
            RefreshCurrentRowsCTS.Cancel();
            RefreshCurrentRowsCTS = new();
            CancellationToken token = RefreshCurrentRowsCTS.Token;
            await Task.Run(async () => await RefreshCurrentRowsTCS.Task, token);
            CurrentRows = await GetTextAreaRows();
            RefreshCurrentRowsTCS = new();
            var _ = ClearRefreshCurrentRowsCooldown();
        }
        private async Task ClearRefreshCurrentRowsCooldown()
        {
            await Task.Delay(250);
            RefreshCurrentRowsTCS.TrySetResult();
        }
        private async Task<int> GetTextAreaRows()
        {
            return await JavascriptService.GetTextAreaLineCount(IdAttribute, TextAreaFakeIdAttribute);
        }

        protected override string GetCssClass()
        {
            StringBuilder sb = new();

            sb.AppendLine(base.GetCssClass());

            return sb.ToString();
        }
    }
}
