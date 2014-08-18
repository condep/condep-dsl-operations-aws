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
        private readonly AwsBootstrapInputValues _values;

        public AwsBootstrapUserDataOptions(AwsBootstrapInputValues values, IOfferAwsBootstrapOptions options)
        {
            _options = options;
            _values = values;
        }

        public IOfferAwsBootstrapOptions PowerShell(string script)
        {
            _values.UserData = "<powershell>" + script + "</powershell>";
            return _options;
        }

        public IOfferAwsBootstrapOptions RunCmd(string script)
        {
            _values.UserData = "<script>" + script + "</script>";
            return _options;
        }
    }
}