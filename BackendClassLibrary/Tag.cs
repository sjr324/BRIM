using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRIM.BackendClassLibrary
{
    public class Tag
    {
        public int ID;
        public string Name;

        public Tag() { }

        public Tag(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}
