using Minuteman.Abstract;
using Ninject;

namespace Minuteman.Tests
{
    public class NinjectFixture 
    {
        private readonly IKernel kernel;

        public NinjectFixture()
        {
            kernel = new StandardKernel(new TestModule());
            Client = kernel.Get<IClient>();
        }

        public IClient Client { get; private set; }
    }
}
