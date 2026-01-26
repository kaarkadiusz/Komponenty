using Komponenty.Classes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Komponenty.Inputs
{
    public partial class KAInputSelectOption<T> : KAComponentBase
    {
        [CascadingParameter]
        public KAInputSelect<T> Select { get; set; } = null!;
        [Parameter, EditorRequired]
        public T Value { get; set; }
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        private string IdAttribute => $"{nameof(KAInputRadio<>)}-{GetHashCode()}";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if(Select is null)
            {
                throw new InvalidOperationException($"There was no {nameof(KAInputSelect<T>)} found to be associated with this {nameof(KAInputSelectOption<T>)}. \nThe {nameof(KAInputSelectOption<T>)} has to be rendered as a child of {nameof(KAInputSelect<T>)}.");
            }
        }
    }
}
