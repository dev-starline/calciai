using CalciAI.Caching;
using CalciAI.CommonManager.CommandProcessor;
using CalciAI.CommonManager.CommandProcessor.UserMaster;
using CalciAI.CommonManager.Services;
using CalciAI.Models;
using CalciAI.Models.Admin;
using CalciAI.Persistance;
using CalciAI.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace CalciAI.AdminService.ControllerApis
{
    [Route("manage/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ClientMasterController : ApiControllerBase
    {
        private readonly ISqlBus _sqlBus;
        private readonly IUserMasterService _userMasterService;

        public ClientMasterController(ILogger<ClientMasterController> logger, IUserMasterService userMasterService, ISqlBus sqlBus) : base(logger)
        {
            _sqlBus = sqlBus;
            _userMasterService = userMasterService;

        }


        [HttpGet] // Provide all the details of feedback
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetAllCLByDetails()
        {
            var currUser = GetCurrentUser();

            var userData = await _userMasterService.GetAllClientDetailsAsync(currUser.Username);

            if (userData.IsSuccess)
            {
                return Ok(userData);
            }

            return BadRequest(userData);
        }


        [HttpGet("clientdetails/{clientID}")]// Provide data for main listing in admin office user
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetClientDetailsByID(int clientID)
        {
            var currUser = GetCurrentUser();


            var UserData = await _userMasterService.GetByClientIdAsync(currUser.Username, clientID);

            if (UserData.IsSuccess)
            {
                return Ok(UserData);
            }

            return BadRequest(UserData);
        }

        [HttpPost("client")] // Add admin office user
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> AddClient(AddClientMasterModel addClientMasterModel)
        {
            var currUser = GetCurrentUser();

            var host = WebUtils.GetHost(HttpContext);
            var secret = SecretKey.secret;
            var requestIP = WebUtils.GetRemoteIp(Request.HttpContext);
          //  addClientMasterModel.UserRole = UserRole.AdminOfficeUser.ToString();

            var command = new AddClientCommand
            {
                SecretKey = secret,
                OperatorUserId = currUser,
                AddClientMasterModel = addClientMasterModel,
                IPAddress = requestIP,
            };

            var response = await _sqlBus.ExecuteSql(command, currUser.Username);

            if (response.IsSuccess)
            {
                var userData = await _userMasterService.GetByClientIdAsync(currUser.Username,Convert.ToInt32(response.ReturnID) );
                ////S2S call to insert Client record in DB
                //var client = new HttpClient();
                //var otherServerUrl = "https://other-server/api/other-endpoint";  // Change this to the actual URL of the target server
                //var jsonContent = JsonConvert.SerializeObject(addClientMasterModel);  // Serialize the model to JSON
                //var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                //try
                //{
                //    // Step 2: Send the request to the other server
                //    var s2sResponse = await client.PostAsync(otherServerUrl, content);

                //    if (s2sResponse.IsSuccessStatusCode)
                //    {
                //        // Step 3: Handle success, for example:
                //        return Ok(ProcessResult<AddClientMasterModel>.Success(userData.Data, response.ReturnID, response.Action, response.SuccessMessage));
                //    }
                //    else
                //    {
                //        // Step 4: If the request failed, handle the failure accordingly
                //        return StatusCode((int)s2sResponse.StatusCode, "Failed to insert data into the other server.");
                //    }
                //}
                //catch (Exception ex)
                //{
                //    // Step 5: Handle exception
                //    return StatusCode(500, $"Internal Server Error: {ex.Message}");
                //}
                ////end S2S Call
                return Ok(ProcessResult<AddClientMasterModel>.Success(userData.Data, response.ReturnID, response.Action, response.SuccessMessage));
            }

            return BadRequest(response);
        }

        [HttpPut] // Edit client information from admin module
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpdateClient(AddClientMasterModel clientMasterModel)
        {
            var currUser = GetCurrentUser();

            var requestIP = WebUtils.GetRemoteIp(Request.HttpContext);

            var command = new UpdateClientCommand
            {
                OperatorUserId = currUser,
                AddClientMasterModel = clientMasterModel,
                IPAddress = requestIP,
            };

            var response = await _sqlBus.ExecuteSql(command, currUser.Username);

            if (response.IsSuccess)
            {
                var ClientData = await _userMasterService.GetByClientIdAsync(currUser.Username, Convert.ToInt32(clientMasterModel.ClientMasterID));

                clientMasterModel.ClientMasterID = ClientData.Data.ClientMasterID;
             

                return Ok(ProcessResult<AddClientMasterModel>.Success(ClientData.Data, response.ReturnID, response.Action, response.SuccessMessage));
            }

            return BadRequest(response);
        }

    }
}
