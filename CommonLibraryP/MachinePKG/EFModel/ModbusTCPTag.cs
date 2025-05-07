﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class ModbusTCPTag : Tag
    {
        public override int DataType
        {
            get => base.DataType;
            set
            {
                base.DataType = value;
                if (IsMultipleValue)
                {
                    if (Offset < 2)
                    {
                        Offset = 2;
                    }
                }
                else
                {
                    if (Offset > 1)
                    {
                        Offset = 1;
                    }
                }

                if (IsString)
                {
                    if (Offset < 2)
                    {
                        Offset = 2;
                    }
                }
                else
                {
                    if (StringReverse)
                    {
                        StringReverse = false;
                    }
                }
            }
        }
        [Required]
        [Range(1, 5)]
        public int Station { get; set; }
        public bool InputOrOutput { get; set; }
        [Required]
        [Range(0, 10000)]
        public int StartIndex { get; set; }
        [Required]
        [Range(1, 10)]
        public int Offset { get; set; }
        [NotMapped]
        public int OffsetMinValue => IsMultipleValue ? 2 : 1;

        [NotMapped]
        public bool OffsetEditable => IsMultipleValue || IsString;

        public bool StringReverse { get; set; }


    }
}
