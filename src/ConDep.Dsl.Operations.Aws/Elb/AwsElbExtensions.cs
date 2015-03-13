using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Amazon.ElasticLoadBalancing.Model;
using ConDep.Dsl.Operations.Aws;
using ConDep.Dsl.Operations.Aws.Elb;

namespace ConDep.Dsl
{
    public static class AwsElbExtensions
    {
        public static IOfferAwsOperations CreateLoadBalancer(this IOfferAwsElbOperations elb, string name, Action<IOfferAwsElbOptions> options = null)
        {
            var elbBuilder = elb as AwsElbOperationsBuilder;
            var localBuilder = ((AwsOperationsBuilder) elbBuilder.AwsOperations).LocalOperations;

            var builder = new AwsElbOptionsBuilder(name);
            if (options != null)
            {
                options(builder);
            }

            var op = new AwsElbOperation(builder.Values);
            Configure.Operation(localBuilder, op);
            return elbBuilder.AwsOperations;
        }
    }

    public interface IOfferAwsElbOptions
    {
        IOfferAwsElbListeners Listeners { get; }
        IOfferAwsElbAvailabillityZones AvailabillityZones { get; }
        IOfferAwsElbOptions SecurityGroups(params string[] securityGroup);
        IOfferAwsElbOptions Subnets(params string[] subnet);
        IOfferAwsTagOptions Tags { get; }
        //IOfferAwsElbOperations HealthCheck(string pingProtocol, int pingPort, string pingPath);
    }

    internal class AwsElbListenersBuilder : IOfferAwsElbListeners
    {
        private readonly IOfferAwsElbOptions _rootOptions;
        private readonly List<Listener> _listeners;

        public AwsElbListenersBuilder(IOfferAwsElbOptions rootOptions, List<Listener> listeners)
        {
            _rootOptions = rootOptions;
            _listeners = listeners;
        }

        public IOfferAwsElbOptions Add(AwsElbProtocol loadBalancerProtocol, int loadBalancerPort, AwsElbProtocol instanceProtocol, int instancePort)
        {
            string lbProto = GetProtocol(loadBalancerProtocol);
            string instProto = GetProtocol(instanceProtocol);

            var listener = new Listener
            {
                InstancePort = instancePort,
                InstanceProtocol = instProto,
                LoadBalancerPort = loadBalancerPort,
                Protocol = lbProto
            };
            _listeners.Add(listener);
            return _rootOptions;
        }

        private string GetProtocol(AwsElbProtocol loadBalancerProtocol)
        {
            switch (loadBalancerProtocol)
            {
                case AwsElbProtocol.Http:
                    return "HTTP";
                case AwsElbProtocol.Https_secure_http:
                    return "HTTPS";
                case AwsElbProtocol.Ssl_secure_tcp:
                    return "SSL";
                case AwsElbProtocol.Tcp:
                    return "TCP";
                default:
                    throw new Exception("Option not supported.");
            }
        }
    }

    public enum AwsElbProtocol
    {
        Http,
        Https_secure_http,
        Ssl_secure_tcp,
        Tcp
    }

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

    public interface IOfferAwsElbTags
    {
    }

    public interface IOfferAwsElbSubnets
    {
    }

    public interface IOfferAwsElbSecurityGroups
    {
    }

    public interface IOfferAwsElbAvailabillityZones
    {
    }

    public interface IOfferAwsElbListeners
    {
        IOfferAwsElbOptions Add(AwsElbProtocol loadBalancerProtocol, int loadBalancerPort, AwsElbProtocol instanceProtocol, int instancePort);
    }

    internal class AwsElbOperationsBuilder : IOfferAwsElbOperations
    {
        private readonly IOfferAwsOperations _awsOps;

        public AwsElbOperationsBuilder(IOfferAwsOperations awsOps)
        {
            _awsOps = awsOps;
        }

        public IOfferAwsOperations AwsOperations
        {
            get { return _awsOps; }
        }
    }
}