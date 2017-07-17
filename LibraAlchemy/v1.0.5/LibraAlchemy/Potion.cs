using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraAlchemy
{
    class Potion
    {
        public Potion()
        {
            ID = -1;
            Effects = new List<FinalEffect>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public List<FinalEffect> Effects { get; set; }
        public int ID { get; set; }

        public override string ToString()
        {
            string s = Description + Environment.NewLine + "|";
            foreach (FinalEffect fe in Effects)
            {
                s += " " + fe.Name;
                if (fe.Commulative)
                    s+= "("+fe.Level+")";
                s += " |";
            }
            return s;
        }
    }

    class PotionNameComparer : IComparer<Potion>
    {
        public int Compare(Potion x, Potion y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}
