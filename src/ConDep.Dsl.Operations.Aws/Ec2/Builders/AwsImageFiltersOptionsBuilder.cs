using Amazon.EC2.Model;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Runtime.Serialization;

namespace ConDep.Dsl.Operations.Aws.Ec2.Builders
{
    internal class AwsFiltersOptionsBuilder : IOfferAwsFilterOptions
    {
        static List<string> ValidNames = new List<string>(new string[]
        {
            "architecture", "block-device-mapping.delete-on-termination",
            "block-device-mapping.device-name", "block-device-mapping.snapshot-id",
            "block-device-mapping.volume-size", "block-device-mapping.volume-type",
            "description", "ena-support", "hypervisor", "image-id",
            "image-type", "is-public", "kernel-id", "manifest-location",
            "name", "owner-alias", "owner-id", "platform",
            "product-code", "product-code.type", "ramdisk-id", "root-device-name",
            "root-device-type", "state", "state-reason-code", "state-reason-message",
            "tag-key", "tag-value", "virtualization-type",
        });
        static Regex ValidTagNameRegex = new Regex("tag:.*");
        List<Filter> _filters;
        public AwsFiltersOptionsBuilder(List<Filter> filters)
        {
            this._filters = filters;
        }
        public IOfferAwsFilterOptions Add(string name, string value)
        {
            validateName(name);
            this._filters.Add(new Filter(name, new List<string>(new string[] { value })));
            return this;
        }

        private void validateName(string name)
        {
            if(!ValidTagNameRegex.Match(name).Success || !ValidNames.Contains(name))
            {
                throw new FilterNameValidationException($"{name} is not a valid name for a filter");
            }
        }

        [Serializable]
        private class FilterNameValidationException : Exception
        {
            public FilterNameValidationException()
            {
            }

            public FilterNameValidationException(string message) : base(message)
            {
            }
        }
    }
}