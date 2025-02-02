using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG
{
    public class SingleSarialNo
    {
        [Required]
        public string serialNo { get; set; } = string.Empty;
    }
}
