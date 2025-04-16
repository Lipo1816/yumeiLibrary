using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG
{
    public class WorkorderIdModel
    {
        [Required]
        public Guid WorkorderID { get; set; }
    }
}
