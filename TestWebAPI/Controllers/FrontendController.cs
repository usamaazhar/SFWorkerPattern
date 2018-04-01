using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestWebAPI.Business;
using TestWebAPI.Interfaces;

namespace TestWebAPI.Controllers
{
    [Produces("application/json")]
    public class FrontendController : Controller
    {
        private IProvisionUtility m_provisionUtility;
        /// <summary>
        /// This method will send the call to the backend if it exists if not
        /// it will create one for the caller
        /// </summary>
        /// <returns></returns>
        ///
        public FrontendController()
        {
            m_provisionUtility = new ProvisionUtility();
        }

        [Route("Ping/{backedName}")]
        public async Task<IActionResult> RouteToBackend(string backedName)
        {
            try
            {
                if (!await m_provisionUtility.CheckIfBackendExists(backedName))
                {
                    await m_provisionUtility.ProvisionNewBackend(backedName);
                }

                var backendClient = m_provisionUtility.GetBackendReference(backedName);
                string message = backendClient.PingBackend("payload").Result;
                return Ok(message);
            }
            catch (Exception ex)
            {
                return Ok("Success");
                throw;
            }
        }


    }
}