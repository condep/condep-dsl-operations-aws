using System;
using Amazon.EC2;
using Amazon.EC2.Model;
using ConDep.Dsl.Logging;

namespace ConDep.Dsl.Operations.Aws.Ec2.Handlers
{
    [Serializable]
    public class CouldNotDeleteSnapshotException : Exception
    {
        public CouldNotDeleteSnapshotException()
        {
        }

        public CouldNotDeleteSnapshotException(string message) : base(message)
        {
        }
    }
    class Ec2SnapshotHandler
    {
        private IAmazonEC2 _client;

        public Ec2SnapshotHandler(IAmazonEC2 client)
        {
            _client = client;
        }

        public void Delete(string snapshotId)
        {
            Logger.Info($"Deleting snapshot {snapshotId}");
            try
            {
                var response = _client.DeleteSnapshot(new DeleteSnapshotRequest(snapshotId));
            } catch(Exception e)
            {
                throw new CouldNotDeleteSnapshotException($"Could not delete snapshot with id {snapshotId}: {e.Message}");
            }
        }
    }
}
