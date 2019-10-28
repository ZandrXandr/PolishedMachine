using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptionalUI
{
    public interface SelectableUIelement
    {
        bool IsMouseOverMe { get; }

        bool CurrentlySelectableMouse { get; }

        bool CurrentlySelectableNonMouse { get; }
    }
}
