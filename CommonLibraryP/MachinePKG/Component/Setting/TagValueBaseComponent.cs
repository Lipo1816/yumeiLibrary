using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Component
{
    public class TagValueBaseComponent:ComponentBase
    {
        [Parameter]
        public Tag? TagParam { get; set; }

        protected bool hasTag => TagParam is not null;

        [Parameter]
        public EventCallback<Object> TagValueSubmit { get; set; }

        protected async Task SubmitTagValue(Object obj)
        {
            await TagValueSubmit.InvokeAsync(obj);
        }
    }
}
