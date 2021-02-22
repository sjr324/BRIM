using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM.BackendClassLibrary
{
    public class Notification
    {
        public int ID;
        private int StatusCode = 0;
        private string message;

        public Notification(int id, string message)
        {
            this.ID = id;
            this.message = message;
        }
    }
}
