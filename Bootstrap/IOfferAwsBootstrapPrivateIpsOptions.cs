namespace ConDep.Dsl
{
    public interface IOfferAwsBootstrapPrivateIpsOptions
    {
        IOfferAwsBootstrapPrivateIpsOptions Add(string ip, bool isPrimary = false);
    }

    class AwsBootstrapPrivateIpsOptions : IOfferAwsBootstrapPrivateIpsOptions
    {
        private readonly AwsNetworkInterfaceValues _values;

        public AwsBootstrapPrivateIpsOptions(AwsNetworkInterfaceValues values)
        {
            _values = values;
        }

        public IOfferAwsBootstrapPrivateIpsOptions Add(string ip, bool isPrimary = false)
        {
            _values.PrivateIps.Add(new AwsPrivateIp{Ip = ip, IsPrimary = isPrimary});
            return this;
        }
    }

    internal class AwsPrivateIp
    {
        public string Ip { get; set; }
        public bool IsPrimary { get; set; }
    }
}