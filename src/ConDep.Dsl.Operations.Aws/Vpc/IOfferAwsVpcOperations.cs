using System;
using System.Threading;
using Amazon.EC2;
using Amazon.EC2.Model;
using ConDep.Dsl.Config;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl
{
    public interface IOfferAwsVpcOperations
    {
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

    public class VpcCreateSecurityGroupOperation : LocalOperation
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

        public override void Execute(IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            var client = new AmazonEC2Client();
            var request = new CreateSecurityGroupRequest
            {
                Description = _description,
                GroupName = _groupName,
                VpcId = _vpcId
            };

            client.CreateSecurityGroup(request);
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get
            {
                return "Create Security Group";
            }
        }
    }
}