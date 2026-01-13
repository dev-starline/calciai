using CalciAI.Models;
using CalciAI.Persistance;
using CalciAI.Persistance.Constants;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CalciAI.AuthManager.Services
{
    public static class LoginValidationService
    {
        public static async Task<ProcessResult<UserLoginDetail>> CheckLoginUserAsync(LoginValidationModel loginValidationModel)
        {
            try
            {
                SqlParameter[] parameters =
                {
                  
                    new SqlParameter("@UserName", loginValidationModel.UserName),
                    new SqlParameter("@UserPassword", Crypto.Md5.GetMd5Hash(loginValidationModel.Secret, loginValidationModel.UserPassword)),
                    new SqlParameter("@RequestedPortal", loginValidationModel.RequestedPortal),
                    new SqlParameter("@LastLoginIP", loginValidationModel.IpAddress),
                };

                var data = await SqlService.ExecuteReaderAsync<UserLoginDetail>(SPNames.UserMaster_CheckUserLogin, parameters);

                if(data.IsSuccess)
                {
                    return ProcessResult<UserLoginDetail>.Success(data);
                }

                return ProcessResult<UserLoginDetail>.Fail("Request", data.Msg.ToString());
            }
            catch (Exception ex)
            {
                return ProcessResult<UserLoginDetail>.Fail("Exception", ex.Message);
            }
        }
    }
}