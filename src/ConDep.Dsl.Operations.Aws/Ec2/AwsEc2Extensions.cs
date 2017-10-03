using System;
using System.Collections.Generic;
using ConDep.Dsl.Builders;
using ConDep.Dsl.Operations.Aws;
using ConDep.Dsl.Operations.Aws.Ec2;
using ConDep.Dsl.Operations.Aws.Ec2.Builders;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using ConDep.Dsl.Operations.Aws.Ec2.Operations;

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
    public static class AwsEc2Extensions
    {
        /// <summary>
        /// Bootstrap one or more Amazon AWS Virtual Private Cloud (VPC) instances and adds 
        /// them to ConDep's servers collection. This method assume mandatory settings are 
        /// found in ConDep's environment file. If you prefer to specify settings directly, use
        /// the other overload and specify settings in code. 
        /// </summary>
        /// <param name="ec2"></param>
        /// <param name="bootstrapId">Unique, case-sensitive identifier you provide to ensure the idempotency of the bootstrap operation. 
        /// In AWS this is refered to as the Client Token.</param>
        /// <returns></returns>
        public static Result CreateInstances(this IOfferAwsEc2Operations ec2, string bootstrapId)
        {
            var ec2Builder = ec2 as AwsEc2OperationsBuilder;

            var options = new AwsBootstrapOptionsValues(bootstrapId);
            var awsBootstrapOperation = new AwsBootstrapOperation(options);

            OperationExecutor.Execute((LocalBuilder)ec2, awsBootstrapOperation);
            return ec2Builder.Result;
        }

        /// <summary>
        /// Bootstrap one or more Amazon AWS Virtual Private Cloud (VPC) instances and adds 
        /// them to ConDep's servers collection. This method assume mandatory settings are 
        /// found in ConDep's environment file. If you prefer to specify settings directly, use
        /// the other overload and specify settings in code.
        /// </summary>
        /// <param name="ec2"></param>
        /// <param name="tags">Tags to use as identifiers to ensure the idempotency of the bootstrap operation. 
        /// Make sure that the tag(s) used uniquely identifies instances you want to manage by your ConDep operation.</param>
        /// <returns></returns>
        public static Result CreateInstances(this IOfferAwsEc2Operations ec2, Action<IOfferAwsTagOptions> tags)
        {            
            var ec2Builder = ec2 as AwsEc2OperationsBuilder;

            var instanceOptions = new AwsBootstrapOptionsBuilder(tags);
            var awsBootstrapOperation = new AwsBootstrapOperation(instanceOptions.Values);

            OperationExecutor.Execute((LocalBuilder)ec2, awsBootstrapOperation);
            return ec2Builder.Result;
        }

        /// <summary>
        /// Bootstrap one or more Amazon AWS Virtual Private Cloud (VPC) instances and adds 
        /// them to ConDep's servers collection. This method assume mandatory settings not  
        /// specified in code is found in ConDep's environment file. 
        /// </summary>
        /// <param name="ec2"></param>
        /// <param name="bootstrapId">Unique, case-sensitive identifier you provide to ensure the idempotency of the bootstrap operation.</param>
        /// <param name="options">Additional configuration options for bootstrapping instances.</param>
        /// <returns></returns>
        public static Result CreateInstances(this IOfferAwsEc2Operations ec2, string bootstrapId, Action<IOfferAwsBootstrapOptions> options)
        {
            var ec2Builder = ec2 as AwsEc2OperationsBuilder;

            var instanceOptions = new AwsBootstrapOptionsBuilder(bootstrapId);
            options(instanceOptions);

            var awsBootstrapOperation = new AwsBootstrapOperation(instanceOptions.Values);

            OperationExecutor.Execute((LocalBuilder)ec2, awsBootstrapOperation);
            return ec2Builder.Result;
        }

        /// <summary>
        /// Bootstrap one or more Amazon AWS Virtual Private Cloud (VPC) instances and adds 
        /// them to ConDep's servers collection. This method assume mandatory settings not  
        /// specified in code is found in ConDep's environment file. 
        /// 
        /// </summary>
        /// <param name="ec2"></param>
        /// <param name="tags">EC2 tags to make the instances created identifiable and therefore idempotent</param>
        /// <param name="options">Additional configuration options for bootstrapping instances.</param>
        /// <returns></returns>
        public static Result CreateInstances(this IOfferAwsEc2Operations ec2, Action<IOfferAwsTagOptions> tags, Action<IOfferAwsBootstrapOptions> options)
        {
            var ec2Builder = ec2 as AwsEc2OperationsBuilder;

            var instanceOptions = new AwsBootstrapOptionsBuilder(tags);
            options(instanceOptions);

            var awsBootstrapOperation = new AwsBootstrapOperation(instanceOptions.Values);
            OperationExecutor.Execute((LocalBuilder)ec2, awsBootstrapOperation);
            return ec2Builder.Result;
        }

        /// <summary>
        /// Terminates the instances that are bootstrapped with the given bootstrap-ID.
        /// </summary>
        /// <param name="ec2"></param>
        /// <param name="bootstrapId">Unique, case-sensitive identifier you provide to ensure the idempotency of the bootstrap operation. 
        /// In AWS this is refered to as the Client Token.</param>
        /// <returns></returns>
        public static Result TerminateInstances(this IOfferAwsEc2Operations ec2, string bootstrapId)
        {
            var ec2Builder = ec2 as AwsEc2OperationsBuilder;

            var options = new AwsTerminateOptionsValues(bootstrapId);
            var awsTerminateOperation = new AwsTerminateOperation(options);

            OperationExecutor.Execute((LocalBuilder)ec2, awsTerminateOperation);
            return ec2Builder.Result;
        }

        /// <summary>
        /// Search for AWS intances based on tags
        /// </summary>
        /// <param name="ec2"></param>
        /// <param name="tags">The tags to match</param>
        /// <returns></returns>
        public static Result DiscoverInstances(this IOfferAwsEc2Operations ec2, Action<IOfferAwsTagOptions> tags, Action<IOfferAwsEc2DiscoverOptions> options = null)
        {
            var ec2Builder = ec2 as AwsEc2OperationsBuilder;

            var tagValues = new List<KeyValuePair<string, string>>();
            var tagOptions = new AwsBootstrapTagOptionsBuilder(tagValues);
            tags(tagOptions);

            var awsOptions = new AwsEc2DiscoverOptionsBuilder();
            if (options != null)
            {
                options(awsOptions);
            }

            var awsEc2DiscoverOperation = new AwsEc2DiscoverOperation(tagValues, awsOptions.Values);
            OperationExecutor.Execute((LocalBuilder)ec2, awsEc2DiscoverOperation);
            return ec2Builder.Result;
        }
        /// <summary>
        /// Stops the instances that are bootstrapped with the given bootstrap-ID.
        /// </summary>
        /// <param name="ec2"></param>
        /// <param name="bootstrapId">Unique, case-sensitive identifier you provide to ensure the idempotency of the bootstrap operation. 
        /// In AWS this is refered to as the Client Token.</param>
        /// <returns></returns>
        public static Result StopInstances(this IOfferAwsEc2Operations ec2, string bootstrapId)
        {
            var ec2Builder = ec2 as AwsEc2OperationsBuilder;

            var options = new AwsStopOptionsValues(bootstrapId);
            var awsStopOperation = new AwsStopOperation(options);

            OperationExecutor.Execute((LocalBuilder)ec2, awsStopOperation);
            return ec2Builder.Result;
        }

        //public static IOfferAwsOperations StartInstances(this IOfferAwsEc2Operations ec2)
        //{
        //    throw new NotImplementedException();
        //}

        public static Result CreateImage(this IOfferAwsEc2Operations ec2, string instanceId, string imageName, Action<IOfferAwsImageCreateOptions> options = null)
        {
            var ec2Builder = ec2 as AwsEc2OperationsBuilder;

            var imageOptions = new AwsImageCreateOptionsBuilder(instanceId, imageName);
            options?.Invoke(imageOptions);

            var awsCreateImageOperation = new AwsCreateImageOperation(imageOptions.Values);
            OperationExecutor.Execute((LocalBuilder)ec2, awsCreateImageOperation);
            return ec2Builder.Result;
        }

        /// <summary>
        /// Deregister images based on a filter
        /// </summary>
        /// <param name="ec2"></param>
        /// <param name="filterOptions">Filter to describe images to deregister</param>
        /// <returns></returns>
        public static Result DeregisterImages(this IOfferAwsEc2Operations ec2, Action<IOfferAwsImageDescribeOptions> filterOptions, Action<IOfferAwsImageDeregisterOptions> deregisterOptions = null)
        {

            var ec2Builder = ec2 as AwsEc2OperationsBuilder;
            var imageDescribeOptions = new AwsImageDescribeOptionsBuilder();
            filterOptions.Invoke(imageDescribeOptions);

            var imageDeregisterOptions = new AwsImageDeregisterOptionsBuilder();
            deregisterOptions?.Invoke(imageDeregisterOptions);

            var awsDeregisterImagesOperation = new AwsDeregisterImagesOperation(imageDescribeOptions.Values, imageDeregisterOptions.Values);
            OperationExecutor.Execute((LocalBuilder)ec2, awsDeregisterImagesOperation);
            return ec2Builder.Result;
        }

        //public static IOfferAwsOperations CreateKeyPair(this IOfferAwsEc2Operations ec2, string name)
        //{
        //    throw new NotImplementedException();
        //}

        //public static IOfferAwsOperations ImportKeyPair(this IOfferAwsEc2Operations ec2, string name, string publicKey)
        //{
        //    throw new NotImplementedException();
        //}

        //public static IOfferAwsOperations DeleteKeyPair(this IOfferAwsEc2Operations ec2)
        //{
        //    throw new NotImplementedException();
        //}

        //public static IOfferAwsOperations CreateSnapshot(this IOfferAwsEc2Operations ec2)
        //{
        //    throw new NotImplementedException();
        //}

        //public static IOfferAwsOperations DeleteSnapshot(this IOfferAwsEc2Operations ec2)
        //{
        //    throw new NotImplementedException();
        //}
    }
}