using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StatelessAPI.Business;
using StatelessAPI.Interfaces;

namespace StatelessAPI.Controllers
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
        /// <summary>
        /// Call this get method like this   http://localhost:8453/ping/1
        /// </summary>
        /// <param name="backedName"></param>
        /// <returns></returns>
        [Route("Ping/{backedName}")]
        public async Task<IActionResult> RouteToBackend(string backedName)
        {
            try
            {
                //for the proof of concept Iam considering the backend name as the payload of the request.
                if (!await m_provisionUtility.CheckIfBackendExists(backedName))
                {
                    await m_provisionUtility.ProvisionNewBackend(backedName);
                    return Ok("Provisioned a backend for this request");
                }
                return Ok("Backend for this request is already working ");
            }
            catch (Exception ex)
            {
                return Ok($"Error : {ex.Message}");
            }
        }


    }
}
