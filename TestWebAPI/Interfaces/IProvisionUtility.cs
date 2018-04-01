using BackgroundStatelessWorker.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebAPI.Interfaces
{
    interface IProvisionUtility
    {
        Task<bool> CheckIfBackendExists(string backendName);
        Task ProvisionNewBackend(string backendName);
        IBackgroundWorker GetBackendReference(string backendName);
    }
}
