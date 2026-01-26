using Komponenty.Classes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text;

namespace Komponenty.Basic
{
    public partial class KAButton : KAComponentBase
    {
        [CascadingParameter]
        internal KAButtonGroup? ButtonGroup { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }
        [Parameter]
        public string? Text { get; set; }
        [Parameter]
        public ButtonType Type
        {
            get => ButtonGroup?.Type ?? field;
            set => field = value;
        }
        [Parameter]
        public Color Color 
        { 
            get => ButtonGroup?.Color ?? field; 
            set => field = value; 
        }
        [Parameter]
        public ButtonSize Size 
        { 
            get => ButtonGroup?.Size ?? field; 
            set => field = value; 
        } = ButtonSize.Medium;
        [Parameter]
        public string StartIcon { get; set; } = string.Empty;
        [Parameter]
        public string EndIcon { get; set; } = string.Empty;

        private async Task InvokeOnClick(MouseEventArgs args)
        {
            if(OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(args);
            }
        }

        protected override string GetCssClass()
        {
            StringBuilder sb = new();

            sb.AppendLine("kompButton");
            sb.AppendLine($"type{Type}");
            sb.AppendLine($"color{Color}");
            sb.AppendLine($"size{Size}");

            sb.AppendLine(base.GetCssClass());

            return sb.ToString();
        }

        private string GetButtonType()
        {
            if (Attributes?.ContainsKey("type") ?? false)
            {
                return Attributes["type"].ToString() ?? "button";
            }
            return "button";
        }
    }

    public enum ButtonType
    {
        Filled,
        Outlined,
        TextOnly
    }

    public enum ButtonSize
    {
        Small,
        Medium,
        Large,
    }
}
