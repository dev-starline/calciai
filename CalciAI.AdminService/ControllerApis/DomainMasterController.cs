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
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using CalciAI.AdminService.ChatHubs;
using Microsoft.AspNet.SignalR.Messaging;

namespace CalciAI.AdminService.ControllerApis
{
    [Route("manage/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DomainMasterController : ApiControllerBase
    {
        private readonly ISqlBus _sqlBus;
        private readonly IUserMasterService _userMasterService;

        private readonly IHubContext<ChatHub> _hubContext;

        public DomainMasterController(ILogger<DomainMasterController> logger, IUserMasterService userMasterService, ISqlBus sqlBus, IHubContext<ChatHub> hubContext) : base(logger)
        {
            _sqlBus = sqlBus;
            _userMasterService = userMasterService;
            _hubContext = hubContext;
        }

        [HttpGet] // Provide data for main listing in admin office user
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetAllDetailsForAdmin()
        {
            var currUser = GetCurrentUser();


            var UserData = await _userMasterService.GetAllDomainDetailsAsync(currUser.Username);

            if (UserData.IsSuccess)
            {
                return Ok(UserData);
            }

            return BadRequest(UserData);
        }

        [HttpPost("adddomain")] // Add admin office user
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> AddDomain(DomainMasterModel addDomainMasterModel)
        {
            var currUser = GetCurrentUser();

            var host = WebUtils.GetHost(HttpContext);
            var secret = SecretKey.secret;
            var requestIP = WebUtils.GetRemoteIp(Request.HttpContext);
            //  addClientMasterModel.UserRole = UserRole.AdminOfficeUser.ToString();

            var command = new AddDomainCommand
            {
                SecretKey = secret,
                OperatorUserId = currUser,
                AddDomainMasterModel = addDomainMasterModel,
                IPAddress = requestIP,
            };

            var response = await _sqlBus.ExecuteSql(command, currUser.Username);

            if (response.IsSuccess)
            {
                var userData = await _userMasterService.GetByDomainIdAsync(currUser.Username, Convert.ToInt32(response.ReturnID));
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", userData.Data.URL, userData.Data.DomainID);


                return Ok(ProcessResult<DomainMasterModel>.Success(userData.Data, response.ReturnID, response.Action, response.SuccessMessage));
            }

            return BadRequest(response);
        }

        [HttpPut] // Edit client information from admin module
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpdateDomain(DomainMasterModel addDomainMasterModel)
        {
            var currUser = GetCurrentUser();

            var host = WebUtils.GetHost(HttpContext);
            var secret = SecretKey.secret;
            var requestIP = WebUtils.GetRemoteIp(Request.HttpContext);
            //  addClientMasterModel.UserRole = UserRole.AdminOfficeUser.ToString();

            var command = new AddDomainCommand
            {
                SecretKey = secret,
                OperatorUserId = currUser,
                AddDomainMasterModel = addDomainMasterModel,
                IPAddress = requestIP,
            };

            var response = await _sqlBus.ExecuteSql(command, currUser.Username);

            if (response.IsSuccess)
            {
                var userData = await _userMasterService.GetByDomainIdAsync(currUser.Username, Convert.ToInt32(response.ReturnID));
                return Ok(ProcessResult<DomainMasterModel>.Success(userData.Data, response.ReturnID, response.Action, response.SuccessMessage));
            }

            return BadRequest(response);
        }

    }
}
