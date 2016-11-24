using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ninject;

namespace Unitoys.Ioc
{
    public class NinjectDependencyResolverForMvc:IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolverForMvc(IKernel kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }
            this.kernel = kernel;
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
    }
}