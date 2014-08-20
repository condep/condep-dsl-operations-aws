using System;
using ConDep.Dsl.Operations.Application.Local;
using ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws;
using ConDep.Dsl.Operations.Aws.Bootstrap;
using ConDep.Dsl.Operations.Builders;

namespace ConDep.Dsl
{
    /// <summary>
    /// Usuefult things:
    /// 1) ImageId (Mandatory, ConDep could select default)
    /// 2) InstanceType (t2.micro etc) (Mandatory, ConDep could select default)
    /// 3) Count (min count, max count) (Default 1)
    /// 4) KeyName / KeyPair (public key) (for encrypting win password) (Mandatory)
    /// 5) Private Key (pem) (for decrypting win password) (Mandatory)
    /// 6) UserData (Optional)
    /// 7) Network Interface (new or id) (Optional?)
    ///     7.1) PublicIP?
    ///     7.2) Index
    ///     7.3) SubnetId
    ///     7.4) Security Groups
    ///     7.5) DeleteOnTermination?
    ///     7.6) Private ip(s)
    /// 8) Instance Initiated Shutdown behaviour (stops or terminates) (default stop)
    /// 9) Monitor?
    /// 10) Placement (availability zone)
    /// 11) Private IP
    /// 12) Subnetid (if not on interface??)
    /// 13) Client Token (for idempotencies and termination/description later on)
    /// 14) Block Device Mappings (Disks) (Instance Store Volumes - host volume / EBS - Remote Storage Device)
    ///     If set, will override mapping defined in AMI (image)
    ///     14.1) Device Name (e.g. /dev/sdh)
    ///     14.2) EBS settings
    ///     14.3) NoDevice - Exclude from AMI
    ///     14.4) VirtualName (e.g. ephemeral0) 
    /// </summary>
    public static class AwsBootstrapExtensions
    {
        /// <summary>
        /// Gives access to operations for bootstrapping Amazon AWS resources
        /// </summary>
        /// <param name="bootstrap"></param>
        /// <returns></returns>
        public static IOfferAwsBootstrapOperations Aws(this IOfferBootstrapOperations bootstrap)
        {
            return new AwsBootstrapOperations(((BootstrapOperationsBuilder)bootstrap).LocalOperations);
        }

        /// <summary>
        /// Bootstrap one or more Amazon AWS Virtual Private Cloud (VPC) instances and adds 
        /// them to ConDep's servers collection. This method assume default settings are 
        /// found in ConDep's environment file. If you prefer to specify settings directly, use
        /// one of the other overloads.
        /// </summary>
        /// <param name="bootstrap"></param>
        /// <param name="bootstrapId">Unique, case-sensitive identifier you provide to ensure the idempotency of the bootstrap operation.</param>
        /// <returns></returns>
        public static IOfferAwsBootstrapOperations VpcInstance(this IOfferAwsBootstrapOperations bootstrap, string bootstrapId)
        {
            var mandatoryOptions = new AwsBootstrapMandatoryInputValues(bootstrapId);
            var awsBootstrapOperation = new AwsBootstrapOperation(mandatoryOptions);
            Configure.Local(((AwsBootstrapOperations)bootstrap).LocalOperations, awsBootstrapOperation);
            return bootstrap;
        }

        /// <summary>
        /// Bootstrap one or more Amazon AWS Virtual Private Cloud (VPC) instances and adds 
        /// them to ConDep's servers collection. This method assume default settings are 
        /// found in ConDep's environment file. If you prefer to specify settings directly, use
        /// one of the other overloads.
        /// </summary>
        /// <param name="bootstrap"></param>
        /// <param name="bootstrapId">Unique, case-sensitive identifier you provide to ensure the idempotency of the bootstrap operation.</param>
        /// <param name="options">Additional configuration options for bootstrapping instances.</param>
        /// <returns></returns>
        public static IOfferAwsBootstrapOperations VpcInstance(this IOfferAwsBootstrapOperations bootstrap, string bootstrapId, Action<IOfferAwsBootstrapOptions> options)
        {
            var mandatoryOptions = new AwsBootstrapMandatoryInputValues(bootstrapId);
            var instanceOptions = new AwsBootstrapOptions();
            options(instanceOptions);

            var awsBootstrapOperation = new AwsBootstrapOperation(mandatoryOptions, instanceOptions);
            Configure.Local(((AwsBootstrapOperations)bootstrap).LocalOperations, awsBootstrapOperation);
            return bootstrap;
        }

        /// <summary>
        /// Bootstrap one or more Amazon AWS Virtual Private Cloud (VPC) instances and adds 
        /// them to ConDep's servers collection. This method takes mandatory settings as parameters.
        /// If you prefer to specify settings in the environment configuration file instead, use 
        /// <see cref="VpcInstance(IOfferAwsBootstrapOperations)" /> which takes mandatory
        /// settings from configuration.
        /// </summary>
        /// <param name="bootstrap"></param>
        /// <param name="bootstrapId">Unique, case-sensitive identifier you provide to ensure the idempotency of the bootstrap operation.</param>
        /// <param name="mandatoryConfig">Mandatory configuration for bootstrapping instances.</param>
        /// <returns></returns>
        public static IOfferAwsBootstrapOperations VpcInstance(this IOfferAwsBootstrapOperations bootstrap, string bootstrapId, Action<IOfferAwsBootstrapMandatoryConfig> mandatoryConfig)
        {
            var mandatoryOptions = new AwsBootstrapMandatoryConfig(bootstrapId);
            mandatoryConfig(mandatoryOptions);

            var awsBootstrapOperation = new AwsBootstrapOperation(mandatoryOptions.Values);
            Configure.Local(((AwsBootstrapOperations)bootstrap).LocalOperations, awsBootstrapOperation);
            return bootstrap;
        }

        /// <summary>
        /// Bootstrap one or more Amazon AWS Virtual Private Cloud (VPC) instances and adds 
        /// them to ConDep's servers collection. This method takes mandatory settings as parameters.
        /// If you prefer to specify settings in the environment configuration file instead, use 
        /// <see cref="VpcInstance(IOfferAwsBootstrapOperations, Action{IOfferAwsBootstrapOptions})" /> 
        /// which takes mandatory settings from configuration.
        /// </summary>
        /// <param name="bootstrap"></param>
        /// <param name="bootstrapId">Unique, case-sensitive identifier you provide to ensure the idempotency of the bootstrap operation.</param>
        /// <param name="mandatoryConfig">Mandatory configuration for bootstrapping instances.</param>
        /// <param name="options">Additional configuration options for bootstrapping instances.</param>
        /// <returns></returns>
        public static IOfferAwsBootstrapOperations VpcInstance(this IOfferAwsBootstrapOperations bootstrap, string bootstrapId, Action<IOfferAwsBootstrapMandatoryConfig> mandatoryConfig, Action<IOfferAwsBootstrapOptions> options)
        {
            var mandatoryOptions = new AwsBootstrapMandatoryConfig(bootstrapId);
            mandatoryConfig(mandatoryOptions);

            var instanceOptions = new AwsBootstrapOptions();
            options(instanceOptions);

            var awsBootstrapOperation = new AwsBootstrapOperation(mandatoryOptions.Values, instanceOptions);
            Configure.Local(((AwsBootstrapOperations)bootstrap).LocalOperations, awsBootstrapOperation);
            return bootstrap;
        }
    }
}