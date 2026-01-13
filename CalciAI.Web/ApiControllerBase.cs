using CalciAI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;

namespace CalciAI.Web
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected readonly ILogger Logger;

        protected ApiControllerBase(ILogger logger)
        {
            Logger = logger;
        }

        protected string RemoteIp
        {
            get
            {
                return WebUtils.GetRemoteIp(HttpContext);
            }
        }

        protected DeviceType DeviceType
        {
            get
            {
                return WebUtils.GetDeviceType(HttpContext);
            }
        }

        protected OperatorUserId GetCurrentUser()
        {
            if (User == null || !User.Claims.Any())
            {
                return null;
            }

            var user = new OperatorUserId
            {
                Username = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value,
              //  OperatorId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GroupSid)?.Value
            };

            return user;
        }

        protected string GetCurrentUserRole()
        {
            if (User == null || !User.Claims.Any())
            {
                return null;
            }

            return User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
        }

        protected OkObjectResult Ok(IModel value)
        {
            return base.Ok(value);
        }

        protected BadRequestObjectResult BadRequest(string errorCode, string errorMessage)
        {
            return base.BadRequest(ProcessResult.Fail(errorCode, errorMessage));
        }

        protected BadRequestObjectResult BadRequest(Error error, bool convertToProcessResult = true)
        {
            if (convertToProcessResult)
            {
                return base.BadRequest(ProcessResult.Fail(error));
            }

            return base.BadRequest(error);
        }

        protected BadRequestObjectResult ViewAccessBadRequest()
        {
            return base.BadRequest(ProcessResult.Fail("Access", "You haven't access for view this module !!"));
        }

        protected BadRequestObjectResult AddAccessBadRequest()
        {
            return base.BadRequest(ProcessResult.Fail("Access", "You haven't access for add this module !!"));
        }

        protected BadRequestObjectResult EditAccessBadRequest()
        {
            return base.BadRequest(ProcessResult.Fail("Access", "You haven't access for edit this module !!"));
        }

        protected BadRequestObjectResult AddEditAccessBadRequest()
        {
            return base.BadRequest(ProcessResult.Fail("Access", "You haven't access for add/edit this module !!"));
        }

        protected BadRequestObjectResult DeleteAccessBadRequest()
        {
            return base.BadRequest(ProcessResult.Fail("Access", "You haven't access for delete this module !!"));
        }

        public ObjectResult Return<T>(ProcessResult<T> result) where T : IModel
        {
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Exception);
        }
    }
}
