using CalciAI.CommonManager.CommandProcessor.ClientMaster;
using CalciAI.CommonManager.CommandProcessor.UserMaster;
using CalciAI.CommonManager.Services;
using CalciAI.Models;
using CalciAI.Models.client;
using CalciAI.Persistance;
using CalciAI.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;


namespace CalciAI.Clientservice.ControllerApis
{
    [Route("manage/[controller]")]
    [ApiController]
    [Authorize(Roles = "Client")]
    public class SubscribeMasterController : ApiControllerBase
    {
        private readonly ISqlBus _sqlBus;
        private readonly IUserCLMasterService _userMasterService;

        public SubscribeMasterController(ILogger<SubscribeMasterController> logger, IUserCLMasterService userMasterService, ISqlBus sqlBus) : base(logger)
        {
            _sqlBus = sqlBus;
            _userMasterService = userMasterService;

        }

        [HttpPost("addSubscribe")] // Add admin office user
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> AddSubscribe(SubscribeMasterModel addSubscribeMasterModel)
        {
            var currUser = GetCurrentUser();

            var host = WebUtils.GetHost(HttpContext);
            var secret = SecretKey.secret;
            var requestIP = WebUtils.GetRemoteIp(Request.HttpContext);
            //  addClientMasterModel.UserRole = UserRole.AdminOfficeUser.ToString();

            var command = new AddSubscribeCommand
            {
                SecretKey = secret,
                OperatorUserId = currUser,
                AddSubscribeMasterModel = addSubscribeMasterModel,
                IPAddress = requestIP,
            };

            var response = await _sqlBus.ExecuteSql(command, currUser.Username);

            if (response.IsSuccess)
            {
                var userData = await _userMasterService.GetBySubscribeIdAsync(currUser.Username, Convert.ToInt32(response.ReturnID));
                return Ok(ProcessResult<SubscribeMasterModel>.Success(userData.Data, response.ReturnID, response.Action, response.SuccessMessage));
            }

            return BadRequest(response);
        }

        [HttpPut] // Edit client information from admin module
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpdateCity(SubscribeMasterModel addSubscribeMasterModel)
        {
            var currUser = GetCurrentUser();

            var host = WebUtils.GetHost(HttpContext);
            var secret = SecretKey.secret;
            var requestIP = WebUtils.GetRemoteIp(Request.HttpContext);
            //  addClientMasterModel.UserRole = UserRole.AdminOfficeUser.ToString();

            var command = new AddSubscribeCommand
            {
                SecretKey = secret,
                OperatorUserId = currUser,
                AddSubscribeMasterModel = addSubscribeMasterModel,
                IPAddress = requestIP,
            };

            var response = await _sqlBus.ExecuteSql(command, currUser.Username);

            if (response.IsSuccess)
            {
                var userData = await _userMasterService.GetBySubscribeIdAsync(currUser.Username, Convert.ToInt32(response.ReturnID));
                return Ok(ProcessResult<SubscribeMasterModel>.Success(userData.Data, response.ReturnID, response.Action, response.SuccessMessage));
            }

            return BadRequest(response);
        }

    }
}
