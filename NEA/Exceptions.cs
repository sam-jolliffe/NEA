using System;

namespace NEA
{
    public class NotInListException : Exception
    {
        public NotInListException() : base("Not in list") { }
    }
    public class IsOnPlayerException : Exception
    {
        public IsOnPlayerException() : base("Object is on the player"){ }
    }
}
