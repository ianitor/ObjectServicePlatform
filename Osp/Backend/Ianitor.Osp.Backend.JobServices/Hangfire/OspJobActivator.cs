using System;
using Hangfire;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.JobServices.Hangfire
{
    public class OspJobActivator : JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public OspJobActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}