using CMS;

using Kentico.Xperience.AmazonPersonalize.Module;

[assembly: AssemblyDiscoverable]

[assembly: RegisterModule(typeof(Module))]

namespace Kentico.Xperience.AmazonPersonalize.Module
{
    public class Module : CMS.DataEngine.Module
    {
        public const string NAME = "Kentico.Xperience.AmazonPersonalize";
        public Module() : base(NAME)
        {
        }
    }
}
