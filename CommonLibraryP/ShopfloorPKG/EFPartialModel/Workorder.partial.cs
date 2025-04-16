using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG
{
    public partial class Workorder
    {
        public string WorkorderNoAndLot => $"{WorkorderNo}-{Lot}";
    }
}
