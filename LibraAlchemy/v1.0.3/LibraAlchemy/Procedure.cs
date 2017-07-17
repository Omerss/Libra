using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraAlchemy
{
    class Procedure
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public int ID { get; set; }
        public bool Paired { get; set; }
    }

    class ProcedureStringComparer : IComparer<Procedure>
    {
        public int Compare(Procedure x, Procedure y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}
