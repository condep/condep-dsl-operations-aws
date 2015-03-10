using System;

namespace ConDep.Dsl
{
    public class OperationConfigException : Exception
    {
        public OperationConfigException(string message) : base(message)
        {
            
        }

        public OperationConfigException(string message, Exception exception) : base(message, exception)
        {
            
        }
    }
}