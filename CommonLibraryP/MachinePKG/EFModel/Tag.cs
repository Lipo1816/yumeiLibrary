using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public abstract partial class Tag
    {
        public Guid? Id { get; set; }

        public Guid CategoryId { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        [Range(1, 44)]
        public virtual int DataType { get; set; }
        [NotMapped]
        public bool IsMultipleValue => DataType > 10;
        [NotMapped]
        public bool IsBoolean => DataType % 10 == 1;
        [NotMapped]
        public bool IsUshort => DataType % 10 == 2;
        [NotMapped]
        public bool IsString => DataType % 10 == 4;
        public bool UpdateByTime { get; set; } = true;

        //public int SpecialType { get; set; }

        //public bool Bool1 { get; set; }

        //public bool Bool2 { get; set; }

        //public bool Bool3 { get; set; }

        //public bool Bool4 { get; set; }

        //public bool Bool5 { get; set; }

        //public int Int1 { get; set; }

        //public int Int2 { get; set; }

        //public int Int3 { get; set; }

        //public int Int4 { get; set; }

        //public int Int5 { get; set; }

        //public string? String1 { get; set; }

        //public string? String2 { get; set; }

        //public string? String3 { get; set; }

        //public string? String4 { get; set; }

        //public string? String5 { get; set; }

        public virtual TagCategory Category { get; set; } = null!;
    }
}
