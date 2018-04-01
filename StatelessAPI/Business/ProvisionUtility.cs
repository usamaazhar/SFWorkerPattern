using BackgroundStatelessWorker.Contracts;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using StatelessAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatelessAPI.Business
{
    public class ProvisionUtility : IProvisionUtility
    {
        private string m_appName = FabricRuntime.GetActivationContext().ApplicationName;

        /// <summary>
        /// Checks if a backend with this name exists or not
        /// </summary>
        /// <param name="backendName"></param>
        /// <returns></returns>
        public async Task<bool> CheckIfBackendExists(string backendName)
        {
            Uri serviceName = new Uri($"{m_appName}/{backendName}");
            using (var fabricClient = new FabricClient())
            {
                var serviceList = await fabricClient.QueryManager.GetServiceListAsync(new Uri(m_appName), serviceName);
                if (serviceList.Count > 0)
                    return true;
                return false;
            }
        }



        /// <summary>
        /// Provisions a new backend and name it uniquely according to the name in param
        /// </summary>
        /// <param name="backendName"></param>
        /// <returns></returns>
        public async Task ProvisionNewBackend(string backendName)
        {
            Uri serviceName = new Uri($"{m_appName}/{backendName}");
            var applicationName = new Uri(m_appName);
            var payload = backendName;
            StatelessServiceDescription serviceDescription = new StatelessServiceDescription()
            {
                ApplicationName = applicationName,
                ServiceTypeName = "BackgroundStatelessWorkerType",
                ServiceName = serviceName,
                PartitionSchemeDescription = new UniformInt64RangePartitionSchemeDescription(100, 0, 99),
                // to change the number of instaces of this service please change
                //100 to your desired number and similartly change 99
                ServicePackageActivationMode = ServicePackageActivationMode.ExclusiveProcess,
                InitializationData = Encoding.UTF8.GetBytes( $" Backend name is : {payload}")
            };
            //launch service
            try
            {
                using (var fabricClient = new FabricClient())
                {
                    await fabricClient.ServiceManager.CreateServiceAsync(serviceDescription);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IBackgroundWorker GetBackendReference(string backendName)
        {
            IBackgroundWorker backendClient =
                ServiceProxy.Create<IBackgroundWorker>(new Uri($"{m_appName}/{backendName}"), new ServicePartitionKey(1));
            return backendClient;

        }
    }
}
