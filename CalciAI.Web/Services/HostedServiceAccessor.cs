using CalciAI.Models;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace CalciAI.Web.Services
{
    public interface IHostedServiceAccessor : IService
    {
        T GetHostedService<T>() where T : IHostedService;
    }

    public class HostedServiceAccessor : IHostedServiceAccessor
    {
        private readonly IEnumerable<IHostedService> _hostedService;

        public HostedServiceAccessor(IEnumerable<IHostedService> hostedServices)
        {
            _hostedService = hostedServices;
        }

        public T GetHostedService<T>() where T : IHostedService
        {
            foreach (var service in _hostedService)
            {
                if (typeof(T) == service.GetType())
                {
                    return (T)service;
                }
            }

            return default;
        }
    }
}
