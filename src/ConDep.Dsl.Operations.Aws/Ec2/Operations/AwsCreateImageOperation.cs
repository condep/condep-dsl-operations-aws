using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using ConDep.Dsl.Operations.Aws.Ec2.Handlers;

namespace ConDep.Dsl.Operations.Aws.Ec2.Operations
{
    class AwsCreateImageOperation : AwsIdentifiedOperation
    {
        private AwsImageCreateOptionsValues options;
        public string imageId { get; private set; }

        public AwsCreateImageOperation(AwsImageCreateOptionsValues options) : base(options)
        {
            this.options = options;
        }

        public override string Name => "Create image from instance";

        public override Result Execute(ConDepSettings settings, CancellationToken token)
        {
            LoadOptionsFromConfig(settings);
            ValidateMandatoryOptions(options);
            var imageCreator = new Ec2ImageCreator(options);
            imageId = imageCreator.Create();
            return Result.SuccessChanged();
        }
    }
}
