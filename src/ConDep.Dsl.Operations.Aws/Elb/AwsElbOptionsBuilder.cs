using System.Linq;
using Amazon.ElasticLoadBalancing.Model;

namespace ConDep.Dsl.Operations.Aws.Elb
{
    internal class AwsElbOptionsBuilder : IOfferAwsElbOptions
    {
        private CreateLoadBalancerRequest _request;
        private readonly AwsElbListenersBuilder _listeners;

        public AwsElbOptionsBuilder(string elbName)
        {
            _request = new CreateLoadBalancerRequest(elbName);
            _listeners = new AwsElbListenersBuilder(this, _request.Listeners);
        }

        public IOfferAwsElbListeners Listeners { get { return _listeners; } }
        public IOfferAwsElbAvailabillityZones AvailabillityZones { get; private set; }

        public IOfferAwsElbOptions SecurityGroups(params string[] securityGroup)
        {
            _request.SecurityGroups = securityGroup.ToList();
            return this;
        }

        public IOfferAwsElbOptions Subnets(params string[] subnet)
        {
            _request.Subnets = subnet.ToList();
            return this;
        }
        public IOfferAwsTagOptions Tags { get; private set; }

        public CreateLoadBalancerRequest Values { get { return _request; } }
    }
}