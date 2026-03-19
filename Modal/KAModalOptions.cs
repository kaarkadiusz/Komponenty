using Microsoft.AspNetCore.Components;

namespace Komponenty.Modal
{
    public class KAModalOptions : IKAModalOptions
    {
        public string? Title { get; set; }
        public RenderFragment? HeaderFragment { get; set; }
        public RenderFragment? FooterFragment { get; set; }
        public RenderFragment? Actions { get; set; }
        public KAModalWidth Width { get; set; } = KAModalWidth.Medium;
        public bool ShowCloseButton { get; set; } = true;
        public bool CloseOnBackdropClick { get; set; } = true;
        public bool CloseOnEscapeKey { get; set; } = true;
    }
    internal interface IKAModalOptions
    {
        public string? Title { get; set; }
        public RenderFragment? HeaderFragment { get; set; }
        public RenderFragment? FooterFragment { get; set; }
        public RenderFragment? Actions { get; set; }
        public KAModalWidth Width { get; set; }
        public bool ShowCloseButton { get; set; }
        public bool CloseOnBackdropClick { get; set; }
        public bool CloseOnEscapeKey { get; set; }
    }
}
