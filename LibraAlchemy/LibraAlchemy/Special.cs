using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraAlchemy
{
    class Special
    {
        public Special()
        {
            ID = -1;
            Name = "";
            Power = -1;
            Effect_Pairs = new Dictionary<Effect, Effect>();
            Comment = "";
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public int Power { get; set; }
        public Dictionary<Effect,Effect> Effect_Pairs { get; set; }
        public string Comment { get; set; }

        public override string ToString()
        {
            string s = "";
            foreach (KeyValuePair<Effect, Effect> pair in Effect_Pairs)
            {
                s += pair.Key.Name;
                s += " הופך ל";
                s += pair.Value.Name;
                s += Environment.NewLine;
            }
            return s;
        }
    }
}
