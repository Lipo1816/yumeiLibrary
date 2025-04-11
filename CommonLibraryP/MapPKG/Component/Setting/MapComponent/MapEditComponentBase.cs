using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MapPKG.Component
{
    public class MapEditComponentBase:ComponentBase
    {
        [Parameter]
        public MapComponent MapComponentParam { get; set; } = null!;
        [Parameter]
        public EventCallback<Guid> ComponentClickCallback { get; set; }
        [Parameter]
        public Func<MapComponentEditMode, MouseEventArgs, Task>? ComponentMouseEventStartArgsCallback { get; set; }

        [Parameter]
        public bool IsFocus { get; set; }

        protected string IsFocusCss => IsFocus ? "focus" : string.Empty;
        protected string shapeCss => $"left:{MapComponentParam.PositionX}%; top:{MapComponentParam.PositionY}%; width:{MapComponentParam.Width}%; height:{MapComponentParam.Height}%;";

        
    }
}
