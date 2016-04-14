using System;
using System.Net;
using System.Threading;
using Amazon.EC2;
using Amazon.EC2.Model;
using ConDep.Dsl.Builders;
using ConDep.Dsl.Config;

namespace ConDep.Dsl
{
    public interface IOfferAwsVpcOperations
    {
    }

    public class AwsVpcOperationsBuilder : LocalBuilder, IOfferAwsVpcOperations
    {
        public AwsVpcOperationsBuilder(IOfferAwsOperations awsOps, ConDepSettings settings, CancellationToken token) : base(settings, token)
        {
        }

        public override IOfferLocalOperations Dsl { get; }
    }

    public static class VpcExtensions
    {
        public static IOfferAwsVpcOperations CreateSecurityGroup(this IOfferAwsVpcOperations vpc, string groupName, string description, string vpcId, Action<IOfferAwsVpcSecurityGroupRules> rules)
        {
            return vpc;
        }
    }

    public interface IOfferAwsVpcSecurityGroupRules
    {
        IOfferAwsVpcSecurityGroupInboundRules Inbound { get; }
        IOfferAwsVpcSecurityGroupOutboundRules Outbound { get; }
    }

    public interface IOfferAwsVpcSecurityGroupOutboundRules
    {
    }

    public interface IOfferAwsVpcSecurityGroupInboundRules
    {
    }

    internal class VpcCreateSecurityGroupOperation : LocalOperation
    {
        private readonly string _description;
        private readonly string _groupName;
        private readonly string _vpcId;

        public VpcCreateSecurityGroupOperation(string groupName, string description, string vpcId)
        {
            _description = description;
            _groupName = groupName;
            _vpcId = vpcId;
        }

        public override Result Execute(ConDepSettings settings, CancellationToken token)
        {
            var client = new AmazonEC2Client();
            var request = new CreateSecurityGroupRequest
            {
                Description = _description,
                GroupName = _groupName,
                VpcId = _vpcId
            };

            var response = client.CreateSecurityGroup(request);
            var result = response.HttpStatusCode == HttpStatusCode.Created ? Result.SuccessChanged() : Result.SuccessUnChanged();
            result.Data.HttpStatusCode = response.HttpStatusCode;
            result.Data.GroupId = response.GroupId;
            return result;
        }

        public override string Name => "Create Security Group";
    }
}