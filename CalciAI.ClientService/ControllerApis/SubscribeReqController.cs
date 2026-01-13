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
    public class SubscribeReqController : ApiControllerBase
    {
        private readonly ISqlBus _sqlBus;
        private readonly IUserCLMasterService _userMasterService;

        public SubscribeReqController(ILogger<SubscribeReqController> logger, IUserCLMasterService userMasterService, ISqlBus sqlBus) : base(logger)
        {
            _sqlBus = sqlBus;
            _userMasterService = userMasterService;

        }

        [HttpPost("addSubReq")] // Add admin office user
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> AddCity(SubscribeRequestModel addSubReqModel)
        {
            var currUser = GetCurrentUser();

            var host = WebUtils.GetHost(HttpContext);
            var secret = SecretKey.secret;
            var requestIP = WebUtils.GetRemoteIp(Request.HttpContext);
            //  addClientMasterModel.UserRole = UserRole.AdminOfficeUser.ToString();

            var command = new AddSubReqCommand
            {
                SecretKey = secret,
                OperatorUserId = currUser,
                AddSubReqModel = addSubReqModel,
                IPAddress = requestIP,
            };

            var response = await _sqlBus.ExecuteSql(command, currUser.Username);

            if (response.IsSuccess)
            {
                var userData = await _userMasterService.GetBySubscribeReqIdAsync(currUser.Username, Convert.ToInt32(response.ReturnID));
                return Ok(ProcessResult<SubscribeRequestModel>.Success(userData.Data, response.ReturnID, response.Action, response.SuccessMessage));
            }

            return BadRequest(response);
        }

        [HttpPut] // Edit client information from admin module
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpdateCity(SubscribeRequestModel addSubReqModel)
        {
            var currUser = GetCurrentUser();

            var host = WebUtils.GetHost(HttpContext);
            var secret = SecretKey.secret;
            var requestIP = WebUtils.GetRemoteIp(Request.HttpContext);
            //  addClientMasterModel.UserRole = UserRole.AdminOfficeUser.ToString();

            var command = new UpdateSubReqCommand
            {
                SecretKey = secret,
                OperatorUserId = currUser,
                AddSubReqModel = addSubReqModel,
                IPAddress = requestIP,
            };

            var response = await _sqlBus.ExecuteSql(command, currUser.Username);

            if (response.IsSuccess)
            {
                var userData = await _userMasterService.GetBySubscribeReqIdAsync(currUser.Username, Convert.ToInt32(response.ReturnID));
                return Ok(ProcessResult<SubscribeRequestModel>.Success(userData.Data, response.ReturnID, response.Action, response.SuccessMessage));
            }

            return BadRequest(response);
        }

    }
}
