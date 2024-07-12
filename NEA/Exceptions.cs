using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    public class NotInListException : Exception
    {
        public NotInListException() : base("Not in list") { }
    }
}
