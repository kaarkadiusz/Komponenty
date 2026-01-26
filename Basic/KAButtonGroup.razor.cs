using Komponenty.Classes;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace Komponenty.Basic
{
    public partial class KAButtonGroup : KAComponentBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        [Parameter]
        public ButtonGroupDirection Direction { get; set; } = ButtonGroupDirection.Row;

        [Parameter]
        public ButtonType? Type { get; set; }
        [Parameter]
        public Color? Color { get; set; }
        [Parameter]
        public ButtonSize? Size { get; set; }

        protected override string GetCssClass()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("kompButtonGroup");
            sb.AppendLine($"direction{Direction}");
            sb.AppendLine(base.GetCssClass());

            return sb.ToString();
        }
    }
    
    public enum ButtonGroupDirection
    {
        Row,
        Column
    }
}
