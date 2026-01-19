using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Text;
using CalciAI.Web;
using CalciAI.Models;
using CalciAI.Auth;
using CalciAI.AuthManager.Services;
using MongoDB.Driver;
using CalciAI.Persistance;

namespace CalciAI.AuthService.ControllerApis
{
    [Route("sso/[controller]")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        private readonly ILoggerAdaptor<AuthController> _logAdaptor;
        
        public AuthController(ILogger<AuthController> logger) : base(logger)
        {
            _logAdaptor = new LoggerAdaptor<AuthController>(logger);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GenerateToken([FromForm] string username, [FromForm] string password, [FromForm] int licenseId)
        {
            var secret = SecretKey.secret;
            var pas = Crypto.Md5.GetMd5Hash(secret, "123");


            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return BadRequest("login", "Username and password required");
            }

            var origin = HttpContext.Request.Headers["Origin"].ToString();

            var host = WebUtils.GetHost(HttpContext);
            var requestIP = WebUtils.GetRemoteIp(Request.HttpContext);
            //  var secret = OperatorProvider.GetSecretByDomain(host);


         //   var secret = "CALCI-AI_2435264343";


            //var operatorId = OperatorProvider.GetOperatorByDomain(host);

            //if (string.IsNullOrEmpty(operatorId))
            //{
            //    return BadRequest("login", "Operator Not Found..!!");
            //}

            var checkValidation = new LoginValidationModel
            {
                UserName = username,
                UserPassword = password,
               // OperatorID = operatorId,
               // LicenseID = licenseId,
                Secret = secret,
                IpAddress = requestIP,
                RequestedPortal = origin.ToString().Contains("client.") ? "ClientWeb" : ""
            };

            var response = await LoginValidationService.CheckLoginUserAsync(checkValidation);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Exception);
            }

            var user = response.Data;

            var tokenData = new TokenData
            {
                Username = user.UserName,
                Role = user.UserRole.ToString(),
                Expiration = DateTime.UtcNow.AddDays(10),
                IpAddress = requestIP,
               
                
            };

            var encodedData = AuthUtil.EncodeToken(tokenData, secret);

            Response.Cookies.Append(AuthFields.AUTH_TOKEN, encodedData, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            _logAdaptor.LogInformation("LOGIN: User:{Username}, Role:{Role}, IP: {requestIP}", tokenData.Username, tokenData.Role, requestIP);

            var tdata = new TokenResponse
            {
                Token = encodedData,
                Name = user.Name,
                Expiration = tokenData.Expiration,
                EnforcePassword = user.EnforcePassword
            };

            return Ok(ProcessResult<TokenResponse>.Success(tdata, 0, 1, user.Msg));
        }


        [HttpGet]
        [Route("/TestMacc")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
      
        public IActionResult CheckAPI()
        {

            return Ok("Testsuccessfully!!");
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public IActionResult ValidateToken()
        {
            var secret = SecretKey.secret;
            if (!Request.Headers.ContainsKey(AuthFields.AUTH_HEADER))
            {
                return BadRequest("validate", "Token required");
            }

            var token = (string)Request.Headers[AuthFields.AUTH_HEADER];

            if (token == null)
            {
                return null;
            }

            //var host = WebUtils.GetHost(HttpContext);
          //  var secret = OperatorProvider.GetSecretByDomain(host);

            var response = AuthUtil.DecodeJwtToken(token.Replace(AuthFields.AUTH_HEADER_TOKEN_PREFIX, ""), secret);

            if (!response.IsSuccess)
            {
                return BadRequest("validate", "Token expired");
            }

            var decodedtokenData = response.Data;

            var tokenData = new TokenData
            {
                Username = decodedtokenData.Username,
                Role = decodedtokenData.Role,
                IpAddress = decodedtokenData.IpAddress,
                Expiration = decodedtokenData.Expiration
            };

            return Ok(ProcessResult<TokenData>.Success(tokenData, 0, 1, "Token validated successfully!!"));
        }

        public static string Encrypt(string plainText)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] keyBytes = Encoding.UTF8.GetBytes("1EACF8DB45E8BF93DCD5AFCCBE11D");

            byte[] encryptedBytes = new byte[plainBytes.Length];

            for (int i = 0; i < plainBytes.Length; i++)
            {
                encryptedBytes[i] = (byte)(plainBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Convert.ToBase64String(encryptedBytes);
        }
    }
}