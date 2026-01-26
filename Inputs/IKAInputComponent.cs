using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Komponenty.Inputs
{
    internal interface IKAInputComponent
    {
        string? Label { get; set; }
        string? HelperText { get; set; }
        AdornmentPosition? AdornmentPosition { get; set; }
        string? AdornmentIcon { get; set; }
        string? AdornmentText { get; set; }
        InputType Type { get; set; }
        bool ReadOnly { get; set; }
        bool Disabled { get; set; }
    }
}
