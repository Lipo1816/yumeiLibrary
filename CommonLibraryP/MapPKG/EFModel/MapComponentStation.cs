using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MapPKG
{
    public class MapComponentStation : MapComponent
    {
        [Required(ErrorMessage ="Station is required")]
        public Guid? StationId { get; set; }
    }
}
