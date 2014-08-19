using Amazon.EC2.Model;

namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public interface IOfferAwsBootstrapUserDataOptions
    {
        IOfferAwsBootstrapOptions PowerShell(string script);
        IOfferAwsBootstrapOptions RunCmd(string script);
    }

    internal class AwsBootstrapUserDataOptions : IOfferAwsBootstrapUserDataOptions
    {
        private readonly IOfferAwsBootstrapOptions _options;
        private readonly RunInstancesRequest _values;

        public AwsBootstrapUserDataOptions(RunInstancesRequest values, IOfferAwsBootstrapOptions options)
        {
            _options = options;
            _values = values;
        }

        public IOfferAwsBootstrapOptions PowerShell(string script)
        {
            _values.UserData = @"<powershell>netsh advfirewall firewall add rule name=""WinRM Public in"" protocol=TCP dir=in profile=any localport=5985 remoteip=any localip=any action=allow\n" + script + "</powershell>";
            return _options;
        }

        public IOfferAwsBootstrapOptions RunCmd(string script)
        {
            _values.UserData = @"<script>netsh advfirewall firewall add rule name=""WinRM Public in"" protocol=TCP dir=in profile=any localport=5985 remoteip=any localip=any action=allow\n" + script + "</script>";
            return _options;
        }
    }
}