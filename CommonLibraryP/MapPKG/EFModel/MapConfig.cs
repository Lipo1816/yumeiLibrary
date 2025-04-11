using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MapPKG
{
    public class MapConfig
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string ImageName { get; set; } = null!;
        [Required]
        public string ImageType { get; set; } = null!;
        [Required]
        public byte[] ImageByte { get; set; } = null!;

        public virtual ICollection<MapComponent> MapComponents { get; set; } = new List<MapComponent>();
    }
}
