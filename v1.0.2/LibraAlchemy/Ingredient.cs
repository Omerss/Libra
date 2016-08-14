using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraAlchemy
{
    class Ingredient
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public string Comment { get; set; }
    }

    class IngredientStringComparer : IComparer<Ingredient>
    {
        public int Compare(Ingredient x, Ingredient y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}
