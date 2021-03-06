using System;
using System.Text;
using Amazon.EC2.Model;

namespace ConDep.Dsl.Operations.Aws.Ec2.Builders
{
    internal class AwsBootstrapUserDataOptionsBuilder : IOfferAwsBootstrapUserDataOptions
    {
        private readonly IOfferAwsBootstrapOptions _options;
        private readonly RunInstancesRequest _values;
        private const string WINRM_FIREWALL_RULE = @"netsh advfirewall firewall add rule name=""WinRM Public in"" protocol=TCP dir=in profile=any localport=5985 remoteip=any localip=any action=allow";

        public AwsBootstrapUserDataOptionsBuilder(RunInstancesRequest values, IOfferAwsBootstrapOptions options)
        {
            _options = options;
            _values = values;
            _values.UserData = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("<powershell>{0}</powershell>", WINRM_FIREWALL_RULE)));
        }

        public IOfferAwsBootstrapOptions PowerShell(string script)
        {
            _values.UserData = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("<powershell>{0}\n{1}</powershell>", WINRM_FIREWALL_RULE, script)));
            return _options;
        }

        public IOfferAwsBootstrapOptions RunCmd(string script)
        {
            _values.UserData = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("<script>{0}\n{1}</script>", WINRM_FIREWALL_RULE, script)));
            return _options;
        }
    }
}