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


namespace CalciAI.AdminService.ControllerApis
{
    [Route("manage/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserMasterController : ApiControllerBase
    {
        private readonly ISqlBus _sqlBus;
        private readonly IUserMasterService _userMasterService;

        public UserMasterController(ILogger<UserMasterController> logger, IUserMasterService userMasterService, ISqlBus sqlBus) : base(logger)
        {
            _sqlBus = sqlBus;
            _userMasterService = userMasterService;

        }

        [HttpPut("changepassword")] // Change any user password in admin module.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ChangePassword(ResetUserPasswordModel resetUserPasswordModel)
        {
            var secret = SecretKey.secret;
            var currUser = GetCurrentUser();
            var requestIP = WebUtils.GetRemoteIp(Request.HttpContext);

           

            var host = WebUtils.GetHost(HttpContext);
           
            // var secret = OperatorProvider.GetSecretByDomain(host);

            var command = new ChangeUserPasswordCommand
            {
                OperatorUserId = currUser,
                ResetUserPasswordModel = resetUserPasswordModel,
                SecretKey = secret,
                IPAddress = requestIP,
            };

            var response = await _sqlBus.ExecuteSql(command,"");

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpGet] // Provide all the details of feedback
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetAllSubscribeReqByClientID()
        {
            var currUser = GetCurrentUser();

            var userData = await _userMasterService.GetAllSubscribeReqByClientIdAsync(currUser.Username);

            if (userData.IsSuccess)
            {
                return Ok(userData);
            }

            return BadRequest(userData);
        }


        //[HttpPost("client")] // Add admin office user
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesDefaultResponseType]
        //public async Task<IActionResult> AddClient(AddClientMasterModel addClientMasterModel)
        //{
        //    var currUser = GetCurrentUser();

        //    var host = WebUtils.GetHost(HttpContext);
        //    var secret = SecretKey.secret;
        //    var requestIP = WebUtils.GetRemoteIp(Request.HttpContext);
        //  //  addClientMasterModel.UserRole = UserRole.AdminOfficeUser.ToString();

        //    var command = new AddClientCommand
        //    {
        //        SecretKey = secret,
        //        OperatorUserId = currUser,
        //        AddClientMasterModel = addClientMasterModel,
        //        IPAddress = requestIP,
        //    };

        //    var response = await _sqlBus.ExecuteSql(command, currUser.Username);

        //    if (response.IsSuccess)
        //    {
        //        var userData = await _userMasterService.GetByClientIdAsync(currUser.Username,Convert.ToInt32(response.ReturnID) );
        //        return Ok(ProcessResult<AddClientMasterModel>.Success(userData.Data, response.ReturnID, response.Action, response.SuccessMessage));
        //    }

        //    return BadRequest(response);
        //}

        //[HttpPut] // Edit client information from admin module
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesDefaultResponseType]
        //public async Task<IActionResult> UpdateClient(AddClientMasterModel clientMasterModel)
        //{
        //    var currUser = GetCurrentUser();

        //    var requestIP = WebUtils.GetRemoteIp(Request.HttpContext);

        //    var command = new UpdateClientCommand
        //    {
        //        OperatorUserId = currUser,
        //        AddClientMasterModel = clientMasterModel,
        //        IPAddress = requestIP,
        //    };

        //    var response = await _sqlBus.ExecuteSql(command, currUser.Username);

        //    if (response.IsSuccess)
        //    {
        //        var ClientData = await _userMasterService.GetByClientIdAsync(currUser.Username, Convert.ToInt32(clientMasterModel.ClientMasterID));

        //        clientMasterModel.ClientMasterID = ClientData.Data.ClientMasterID;


        //        return Ok(ProcessResult<AddClientMasterModel>.Success(ClientData.Data, response.ReturnID, response.Action, response.SuccessMessage));
        //    }

        //    return BadRequest(response);
        //}

        //[HttpPost("adddomain")] // Add admin office user
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesDefaultResponseType]
        //public async Task<IActionResult> AddDomain(DomainMasterModel addDomainMasterModel)
        //{
        //    var currUser = GetCurrentUser();

        //    var host = WebUtils.GetHost(HttpContext);
        //    var secret = SecretKey.secret;
        //    var requestIP = WebUtils.GetRemoteIp(Request.HttpContext);
        //    //  addClientMasterModel.UserRole = UserRole.AdminOfficeUser.ToString();

        //    var command = new AddDomainCommand
        //    {
        //        SecretKey = secret,
        //        OperatorUserId = currUser,
        //        AddDomainMasterModel = addDomainMasterModel,
        //        IPAddress = requestIP,
        //    };

        //    var response = await _sqlBus.ExecuteSql(command, currUser.Username);

        //    if (response.IsSuccess)
        //    {
        //        var userData = await _userMasterService.GetByDomainIdAsync(currUser.Username, Convert.ToInt32(response.ReturnID));
        //        return Ok(ProcessResult<DomainMasterModel>.Success(userData.Data, response.ReturnID, response.Action, response.SuccessMessage));
        //    }

        //    return BadRequest(response);
        //}

        //[HttpPut("updatedomain")] // Edit client information from admin module
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesDefaultResponseType]
        //public async Task<IActionResult> UpdateDomain(DomainMasterModel addDomainMasterModel)
        //{
        //    var currUser = GetCurrentUser();

        //    var host = WebUtils.GetHost(HttpContext);
        //    var secret = SecretKey.secret;
        //    var requestIP = WebUtils.GetRemoteIp(Request.HttpContext);
        //    //  addClientMasterModel.UserRole = UserRole.AdminOfficeUser.ToString();

        //    var command = new AddDomainCommand
        //    {
        //        SecretKey = secret,
        //        OperatorUserId = currUser,
        //        AddDomainMasterModel = addDomainMasterModel,
        //        IPAddress = requestIP,
        //    };

        //    var response = await _sqlBus.ExecuteSql(command, currUser.Username);

        //    if (response.IsSuccess)
        //    {
        //        var userData = await _userMasterService.GetByDomainIdAsync(currUser.Username, Convert.ToInt32(response.ReturnID));
        //        return Ok(ProcessResult<DomainMasterModel>.Success(userData.Data, response.ReturnID, response.Action, response.SuccessMessage));
        //    }

        //    return BadRequest(response);
        //}

    }
}
