using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Aws.Ec2.Handlers;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using System.Threading;

namespace ConDep.Dsl.Operations.Aws.Ec2.Operations
{
    internal class AwsDeregisterImagesOperation : AwsIdentifiedOperation
    {
        private readonly AwsImageDescribeOptionsValues options;
        private readonly AwsImageDeregisterOptionsValues deregisterOptions;

        public AwsDeregisterImagesOperation(AwsImageDescribeOptionsValues options, AwsImageDeregisterOptionsValues deregisterOptions) : base(options)
        {
            this.options = options;
            this.deregisterOptions = deregisterOptions;
        }
        public override string Name => "Deregister images";

        public override Result Execute(ConDepSettings settings, CancellationToken token)
        {
            LoadOptionsFromConfig(settings);
            ValidateMandatoryOptions(options);
            var imageDeregisterer = new Ec2ImageDeregisterer(options, deregisterOptions);
            imageDeregisterer.Deregister();
            return Result.SuccessChanged();
        }
    }
}