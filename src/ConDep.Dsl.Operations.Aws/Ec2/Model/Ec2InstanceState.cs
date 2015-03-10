namespace ConDep.Dsl
{
    internal enum Ec2InstanceState
    {
        Pending = 0,
        Running = 16,
        ShuttingDown = 32,
        Terminated = 48,
        Stopping = 64,
        Stopped = 80
    }
}