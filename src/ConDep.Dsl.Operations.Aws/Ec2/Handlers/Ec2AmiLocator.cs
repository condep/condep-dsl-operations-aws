using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.EC2;
using Amazon.EC2.Model;
using ConDep.Dsl.Logging;

namespace ConDep.Dsl.Operations.Aws.Ec2.Handlers
{
    internal class Ec2AmiLocator
    {
        private readonly IAmazonEC2 _client;

        public Ec2AmiLocator(IAmazonEC2 client)
        {
            _client = client;
        }

        public string Find2008Core()
        {
            Logger.Info("Finding Amazon AMI image for Windows Server 2012 R2...");
            var result = GetSearchResults("Windows_Server-2008-SP2-English-64Bit-Base*");
            return result.ImageId;
        }

        public string Find2012R2Core()
        {
            Logger.Info("Finding Amazon AMI image for Windows Server 2012 R2...");
            var result = GetSearchResults("Windows_Server-2012-R2_RTM-English-64Bit-Base*");
            return result.ImageId;
        }

        public string Find2012Core()
        {
            Logger.Info("Finding Amazon AMI image for Windows Server 2012...");
            var result = GetSearchResults("Windows_Server-2012-RTM-English-64Bit-Base*");
            return result.ImageId;
        }

        public string Find2008R2Core()
        {
            Logger.Info("Finding Amazon AMI image for Windows Server 2008 R2...");
            var result = GetSearchResults("Windows_Server-2008-R2_SP1-English-64Bit-Base*");
            return result.ImageId;
        }
        public string Find2016Core()
        {
            Logger.Info("Finding Amazon AMI image for Windows Server 2016...");
            var result = GetSearchResults("Windows_Server-2016-English-Full-Base-*");
            return result.ImageId;
        }

        public string Find(string name)
        {
            name = name.Replace(" ", "*");
            if (!name.EndsWith("*"))
            {
                name += "*";
            }
            var result = GetSearchResults(name);
            return result.ImageId;
        }

        private Image GetSearchResults(string nameSearchField)
        {
            var request = new DescribeImagesRequest();
            request.ExecutableUsers.Add("all");
            request.Owners.Add("amazon");
            request.Filters = new List<Filter>
            {
                new Filter
                {
                    Name = "platform",
                    Values = new List<string>
                    {
                        "windows"
                    }
                },
                new Filter
                {
                    Name = "name",
                    Values = new List<string>
                    {
                        nameSearchField
                    }
                }
            };

            var results = _client.DescribeImages(request);
            var image = results.Images.Count > 1 ? FilterNewest(results.Images) : results.Images.Single();
            Logger.Info("Found image with id {0}, dated {1}", image.ImageId, GetDate(image.Name).ToString("yyyy-MM-dd"));
            return image;
        }

        private Image FilterNewest(IEnumerable<Image> images)
        {
            return images.OrderByDescending(x => GetDate(x.Name)).First();
        }

        private DateTime GetDate(string name)
        {
            var dateString = name.Substring(name.Length - 10);
            return DateTime.ParseExact(dateString, "yyyy.MM.dd", null);
        }

        internal string FindWithFilters(List<Filter> filters, string filterByOwner = null)
        {

            var request = new DescribeImagesRequest();
            if(filterByOwner != null)
                request.ExecutableUsers.Add(filterByOwner);
            request.Filters = filters;

            var results = _client.DescribeImages(request);
            if (results.Images.Count == 0)
                throw new Exception("Could not find any images matching filter");
            var image = results.Images.Count > 1 ? results.Images.OrderByDescending(x => DateTime.Parse(x.CreationDate)).First() : results.Images.Single();
            Logger.Info("Found image with id {0}, dated {1}", image.ImageId, DateTime.Parse(image.CreationDate).ToString("yyyy-MM-dd"));
            return image.ImageId;
        }
    }
}