using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG
{
    public partial class WorkorderRecipeConfig
    {
        public Guid Id { get; set; }
        [Required]
        public string RecipeCategory { get; set; }

        public virtual ICollection<RecipeItem> Recipes { get; set; } = new List<RecipeItem>();

        public virtual ICollection<Workorder> Workorders { get; set; } = new List<Workorder>();
    }
}