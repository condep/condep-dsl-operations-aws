using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConDep.Dsl.Operations.Aws.Ec2
{
    public interface IOfferAwsImageCreateDisksOptions
    {
        IOfferAwsImageCreateOptions Add(string deviceName, string virtualName, string deviceToSuppressFromImage = null);
        IOfferAwsImageCreateOptions Add(string deviceName, Action<IOfferAwsBootstrapEbsOptions> ebs, string deviceToSuppressFromImage = null);
    }
}
