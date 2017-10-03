using Amazon;
using Amazon.EC2.Model;
using Amazon.Runtime;
using ConDep.Dsl.Operations.Aws.Ec2.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConDep.Dsl.Operations.Aws
{
    public interface IOfferAwsOperationsOptionValues
    {
        string PrivateKeyFileLocation { get; set; }
        RegionEndpoint RegionEndpoint { get; set; }
        AWSCredentials Credentials { get; set; }
    }
}
