using Komponenty.Classes;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace Komponenty.Basic
{
    public partial class KAAlert : KAComponentBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        [Parameter]
        public RenderFragment? ContentFragment { get => ChildContent; set => ChildContent = value; }
        [Parameter]
        public RenderFragment? TitleFragment { get; set; }
        [Parameter]
        public string? Icon { get; set; }
        [Parameter]
        public Color Color { get; set; } = Color.Default;
        [Parameter]
        public KAAlertVariant Variant { get; set; } = KAAlertVariant.Filled;
        [Parameter]
        public bool Dense { get; set; }
        [Parameter]
        public EventCallback OnClick { get; set; }
        [Parameter]
        public EventCallback OnClose { get; set; }

        protected override void AppendToCssClass(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("kaAlert");
            stringBuilder.AppendLine($"kaAlert-color{Color}");
            stringBuilder.AppendLine($"kaAlert-variant{Variant}");
            if(OnClick.HasDelegate)
            {
                stringBuilder.AppendLine("kaAlert-clickable");
            }
            if(Dense)
            {
                stringBuilder.AppendLine("kaAlert-dense");
            }
            base.AppendToCssClass(stringBuilder);
        }
    }

    public enum KAAlertVariant
    {
        Filled,
        Outlined
    }
}
