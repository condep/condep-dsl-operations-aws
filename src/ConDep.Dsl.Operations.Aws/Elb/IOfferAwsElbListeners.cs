namespace ConDep.Dsl
{
    public interface IOfferAwsElbListeners
    {
        IOfferAwsElbOptions Add(AwsElbProtocol loadBalancerProtocol, int loadBalancerPort, AwsElbProtocol instanceProtocol, int instancePort);
        IOfferAwsElbOptions AddHttps(int loadBalancerPort, AwsElbProtocol instanceProtocol, int instancePort, string sslCertId);
        IOfferAwsElbOptions AddHttp(int loadBalancerPort, AwsElbProtocol instanceProtocol, int instancePort);
    }
}