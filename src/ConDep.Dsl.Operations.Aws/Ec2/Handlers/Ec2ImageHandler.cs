using Amazon.EC2;
using Amazon.EC2.Model;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Runtime.Serialization;

namespace ConDep.Dsl.Operations.Aws.Ec2.Handlers
{


    [Serializable]
    public class CouldNotCreateAMIException : Exception
    {
        public CouldNotCreateAMIException(string message) : base(message) { }

    }
    [Serializable]
    class CouldNotDeregisterImageException : Exception
    {
        public CouldNotDeregisterImageException()
        {
        }

        public CouldNotDeregisterImageException(string message) : base(message)
        {
        }

        public CouldNotDeregisterImageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotDeregisterImageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    class DescribeImagesException : Exception
    {
        public DescribeImagesException(string message) : base(message) { }

    }
    internal class Ec2ImageHandler
    {
        private readonly IAmazonEC2 _client;
        private readonly Ec2InstanceHandler _instanceHandler;

        public Ec2ImageHandler(IAmazonEC2 client)
        {
            _client = client;
            _instanceHandler = new Ec2InstanceHandler(_client);
        }

        public string Create(AwsImageCreateOptionsValues options)
        {
            Logger.Info($"Creating AMI {options.ImageRequest.Name} from instance {options.ImageRequest.InstanceId}");
            try
            {
                var requiredStartingStates = new List<Ec2InstanceState> { Ec2InstanceState.Stopped };
                if (!options.WaitForShutdown)
                    requiredStartingStates.Add(Ec2InstanceState.Running);
                _instanceHandler.WaitForInstanceToReachOneOfStates(options.ImageRequest.InstanceId, requiredStartingStates);
                var response = _client.CreateImage(options.ImageRequest);
                WaitForImageToReachState(response.ImageId, "available");
                return response.ImageId;
            } catch(Exception e)
            {
                throw new CouldNotCreateAMIException($"Could not create image from instance {options.ImageRequest.InstanceId}: {e.Message}"); // Do we get any additional information about the error?
            }
        }
        
        private void WaitForImageToReachState(string imageId, string state)
        {
            Image image = null;
            while ((image = GetImage(imageId)).State.Value != state)
            {
                Logger.Info("Image Id: {0}  Status: {1}", imageId, image.State.Value);
                Thread.Sleep(10000);
            }
        }
        private void WaitForImageToReachOneOfStates(string imageId, IEnumerable<string> states)
        {
            Image image = null;
            while (!states.Contains((image = GetImage(imageId)).State.Value))
            {
                Logger.Info("Image Id: {0}  Status: {1}", imageId, image.State.Value);
                Thread.Sleep(10000);
            }
        }
        public Image GetImage(string imageId)
        {
            var imageRequest = new DescribeImagesRequest
            {
                ImageIds = new[] { imageId }.ToList()
            };
            return GetImages(imageRequest).Last();
        }

        private IEnumerable<Image> GetImages(DescribeImagesRequest imageRequest)
        {
            var response = _client.DescribeImages(imageRequest);
            if (response.HttpStatusCode != HttpStatusCode.OK)
                throw new DescribeImagesException(response.HttpStatusCode.ToString());
            return response.Images;
        }

        private IEnumerable<Image> GetImages(AwsImageDescribeOptionsValues options)
        {
            IEnumerable<Image> images = GetImages(options.DescribeImagesRequest);
            if (options.ExceptNewest > 0)
                images = images.OrderByDescending(i => DateTime.Parse(i.CreationDate)).Skip(options.ExceptNewest);
            return images;
        }

        internal void Deregister(AwsImageDescribeOptionsValues options, AwsImageDeregisterOptionsValues deregisterOptions)
        {
            var images = GetImages(options);
            var snapshotHandler = new Ec2SnapshotHandler(_client);
            foreach (var image in images)
            {
                Logger.Info($"Deregistering image: {image.Name}");
                var snapshots = image.BlockDeviceMappings.Where(bdm => bdm.Ebs != null).Select(bdm => bdm.Ebs?.SnapshotId);
                try
                {
                    _client.DeregisterImage(new DeregisterImageRequest(image.ImageId));
                } catch(Exception e)
                {
                    throw new CouldNotDeregisterImageException($"Error while deregistering image {image.ImageId}: {e.Message}");
                }
                foreach(var snapshot in snapshots)
                {
                    snapshotHandler.Delete(snapshot);
                }
            }
        }
    }
}
