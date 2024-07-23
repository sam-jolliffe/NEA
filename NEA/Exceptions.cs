using System;

namespace NEA
{
    public class NotInListException : Exception
    {
        public NotInListException() : base("Not in list") { }
    }
}
