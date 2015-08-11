using System;
using System.Collections.Generic;
using Amazon.ElasticLoadBalancing.Model;

namespace ConDep.Dsl.Operations.Aws.Elb
{
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
            return Add(loadBalancerProtocol, loadBalancerPort, instanceProtocol, instancePort, null);
        }

        public IOfferAwsElbOptions AddHttps(int loadBalancerPort, AwsElbProtocol instanceProtocol, int instancePort, string sslCertId)
        {
            return Add(AwsElbProtocol.Https_secure_http, loadBalancerPort, instanceProtocol, instancePort, sslCertId);
        }

        public IOfferAwsElbOptions AddHttp(int loadBalancerPort, AwsElbProtocol instanceProtocol, int instancePort)
        {
            return Add(AwsElbProtocol.Http, loadBalancerPort, instanceProtocol, instancePort, null);
        }

        private IOfferAwsElbOptions Add(AwsElbProtocol loadBalancerProtocol, int loadBalancerPort, AwsElbProtocol instanceProtocol, int instancePort, string sslCertId)
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

            if (!string.IsNullOrWhiteSpace(sslCertId))
            {
                listener.SSLCertificateId = sslCertId;
            }

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
}