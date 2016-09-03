using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraAlchemy
{
    class Product
    {
        public Product()
        {
            Effects = new List<Effect>();
            Product_Ingredient = new Ingredient();
            Product_Procedure = new Procedure();
            Comment = "";
            ID = -1;
            Main_Effect_ID = -1;
        }

        public Ingredient Product_Ingredient { get; set; }
        public Procedure Product_Procedure { get; set; }
        public int ID { get; set; }
        public List<Effect> Effects { get; set; }
        public int Main_Effect_ID { get; set; }
        public string Comment { get; set; }

        public string GetProdName()
        {
            return  Product_Ingredient.Name + " + " + Product_Procedure.Name;
        }

        public override string ToString()
        {
            string s = "";
            foreach (Effect e in Effects)
            {
                s += e.Name;
                s += ": ";
                s += e.ToString();
                s += Environment.NewLine;
            }
            return s;
        }
    }
}
