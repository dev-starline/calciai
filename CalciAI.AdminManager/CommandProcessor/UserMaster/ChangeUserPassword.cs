using CalciAI.Models;
using CalciAI.Models.Admin;
using CalciAI.Persistance;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading.Tasks;

namespace CalciAI.CommonManager.CommandProcessor
{
    public class ChangeUserPasswordCommand : ISqlCommand
    {
        public OperatorUserId OperatorUserId { get; set; }
        public ResetUserPasswordModel ResetUserPasswordModel { get; set; }
        public string SecretKey { get; set; }
        public string IPAddress { get; set; }
    }

    public class ChangeUserPasswordProcessor : ISqlProcessor<ChangeUserPasswordCommand>
    {
        private readonly ILoggerAdaptor<ChangeUserPasswordProcessor> _logAdaptor;

        public ChangeUserPasswordProcessor(ILogger<ChangeUserPasswordProcessor> logger)
        {
            _logAdaptor = new LoggerAdaptor<ChangeUserPasswordProcessor>(logger);
        }

        public ValueTask<ProcessResult> Validate(ChangeUserPasswordCommand command)
        {
           

            if (string.IsNullOrEmpty(command.ResetUserPasswordModel.UserName))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("UserMaster", "User name is required"));
            }

            if (command.ResetUserPasswordModel.OldUserPassword == null || string.IsNullOrEmpty(command.ResetUserPasswordModel.OldUserPassword.Trim()))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("UserMaster", "Old password is required"));
            }

            if (command.ResetUserPasswordModel.UserPassword == null || string.IsNullOrEmpty(command.ResetUserPasswordModel.UserPassword.Trim()))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("UserMaster", "Password is required"));
            }

            if (command.ResetUserPasswordModel.UserConfirmPassword == null || string.IsNullOrEmpty(command.ResetUserPasswordModel.UserConfirmPassword.Trim()))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("UserMaster", "Confirm Password is required"));
            }

            if (command.ResetUserPasswordModel.UserName != command.OperatorUserId.Username)
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("UserMaster", "User name does not matched!!"));
            }

            if (command.ResetUserPasswordModel.OldUserPassword == command.ResetUserPasswordModel.UserPassword)
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("UserMaster", "Old and new password are same !!"));
            }

            if (command.ResetUserPasswordModel.UserPassword != command.ResetUserPasswordModel.UserConfirmPassword)
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("UserMaster", "Password and confirm password does not matched!!"));
            }

            return new ValueTask<ProcessResult>(ProcessResult.Success());
        }

        public async Task<ProcessResult> Execute(ChangeUserPasswordCommand command)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@UserID", command.OperatorUserId.UserId),
                    new SqlParameter("@OldPassword", Crypto.Md5.GetMd5Hash(command.SecretKey, command.ResetUserPasswordModel.OldUserPassword.Trim())),
                    new SqlParameter("@NewPassword", Crypto.Md5.GetMd5Hash(command.SecretKey, command.ResetUserPasswordModel.UserPassword.Trim())),
                    new SqlParameter("@IPAddress", command.IPAddress),
                };

                string result = await SqlService.ExecuteSpWithSingleReturnAsync("UserMaster_ChangePassword", parameters);

              
                var finalResult = ResponseGenerator.Generator(result, "UserMaster");

                //if (finalResult.IsSuccess)
                //{
                //    await RedisRealTimeSocket.SendNotificationAsync(command.ResetUserPasswordModel.UserName, command.OperatorUserId.Username, RedisRealTimeTypes.FORCE_LOGOUT);
                //}

                return ResponseGenerator.Generator(result, "User");
            }
            catch (Exception ex)
            {
                _logAdaptor.LogError("Exception: Update User Password : {ex.Message}:{command}", ex.Message, JsonSerializer.Serialize(command));
                return ProcessResult.Fail("server", ex.Message);
            }
        }
    }
}
