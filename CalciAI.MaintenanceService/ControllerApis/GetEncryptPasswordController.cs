using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using CalciAI;

namespace CalciAI.MaintenanceService.ControllerApis
{
    [Route("maintenance/[controller]")]
    [ApiController]
    public class EncryptDecryptController : ControllerBase
    {
        public EncryptDecryptController(ILogger<EncryptDecryptController> logger)
        {
        }

        //[HttpGet("password/{secretKey}/{password}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesDefaultResponseType]
        //public async Task<IActionResult> GetEncryptPassword(string secretKey, string password)
        //{
        //    string encPassword = Crypto.Md5.GetMd5Hash(secretKey, password);

        //    return Ok(encPassword);
        //}


        [HttpPost("encrypt")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetEncryptString([FromBody] string plainText)
        {
            string encString = EncryptionDecryption.EncryptText(plainText);

            return Ok(encString);
        }

        [HttpPost("decrypt")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetDecryptString([FromBody] string encryptText)
        {
            string decString = EncryptionDecryption.DecryptText(encryptText);

            return Ok(decString);
        }
    }
}