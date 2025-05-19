using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;

namespace CommonLibraryP.MapPKG
{
    public class MapDiagramNode : NodeModel
    {
        private MapComponent mapComponent;
        public MapComponent MapComponent => mapComponent;
        public MapDiagramNode(Point? position = null) : base(position) { }
        public MapDiagramNode SetConfig(MapComponent mapComponent)
        {
            this.mapComponent = mapComponent;
            return this;
        }
        
    }
}
