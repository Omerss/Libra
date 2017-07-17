using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraAlchemy
{
    class Effect
    {
        public Effect()
        {
            InPotion = false;
            IsCountered = false;
            CounterName = "";
            Level = 1;
            Counters_IDs = new List<int>();
            Name = "";
            Power = -1;
            ID = -1;
            Commulative = false;
            Description = "";
            Comment = "";
            FontColor = System.Drawing.Color.Black;
        }
        //From DB
        public string Name { get; set; }
        public int Power { get; set; }
        public List<int> Counters_IDs { get; set; }
        public int ID { get; set; }
        public bool Commulative { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        //From Class
        public int Level { get; set; }
        public bool InPotion { get; set; }
        public bool IsCountered { get; set; }
        public string CounterName { get; set; }
        public System.Drawing.Color FontColor { get; set; } 
        //Other
        public Object Owner { get; set; } //Usually the product that owns the effect

        public override string ToString()
        {
            if (this.IsCountered)
                return " בוטל על ידי ההשפעה " + this.CounterName;
            if (this.FontColor == System.Drawing.Color.Gray)
                return "התבטל עקב כפילות של תוצר";
            if (this.InPotion)
                return " נכנס לשיקוי.";
            else
                return " לא נכנס לשיקוי.";
        }
        

    }

    class EffectComparer : IEqualityComparer<Effect>
    {
        public bool Equals(Effect x, Effect y)
        {
            return x.ID == y.ID;
        }
        public int GetHashCode(Effect obj)
        {
            return obj.ID.GetHashCode();
        }
    }

    class EffectAndLevelComparer : IEqualityComparer<Effect>
    {
        public bool Equals(Effect x, Effect y)
        {
            return (x.ID == y.ID && x.Level == y.Level);
        }
        public int GetHashCode(Effect obj)
        {
            return obj.ID.GetHashCode();
        }
    }

    class EffectStringComparer : IComparer<Effect>
    {
        public int Compare(Effect x, Effect y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }

    class FinalEffect : Effect
    {
        public FinalEffect() : base()
        {
            ChangedBySpecial = false;
        }
        public List<Effect> Parents { get; set; }
        //Specials
        public bool ChangedBySpecial { get; set; }
        public string OldName { get; set; }
        public Special Changer { get; set; }
    }
}
