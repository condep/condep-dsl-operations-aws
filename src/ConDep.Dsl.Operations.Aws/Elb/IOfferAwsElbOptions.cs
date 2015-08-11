namespace ConDep.Dsl
{
    public interface IOfferAwsElbOptions
    {
        IOfferAwsElbListeners Listeners { get; }
        IOfferAwsElbAvailabillityZones AvailabillityZones { get; }
        IOfferAwsElbOptions SecurityGroups(params string[] securityGroup);
        IOfferAwsElbOptions Subnets(params string[] subnet);
        IOfferAwsTagOptions Tags { get; }
        //IOfferAwsElbOperations HealthCheck(string pingProtocol, int pingPort, string pingPath);
    }
}