namespace ConDep.Dsl.Operations.Aws.Ec2.Model
{
    internal class AwsStartOptionsValues : AwsSimpleOperationOptionsValues
    {
        private string bootstrapId;

        public AwsStartOptionsValues(string bootstrapId) : base(bootstrapId)
        {
            this.bootstrapId = bootstrapId;
        }
    }
}