using System;

namespace DecisionWebApi.Exceptions
{
    public class EntityExistException : Exception
    {
        public EntityExistException() : base("Entity exist in database")
        {
        }
    }
}