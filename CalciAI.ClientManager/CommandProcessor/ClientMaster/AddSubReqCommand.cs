using CalciAI.Models;
using CalciAI.Models.client;
using CalciAI.Persistance;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading.Tasks;

namespace CalciAI.CommonManager.CommandProcessor.ClientMaster
{
    public class AddSubReqCommand : ISqlCommand
    {
        public OperatorUserId OperatorUserId { get; set; }
        public SubscribeRequestModel AddSubReqModel { get; set; }
        public string SecretKey { get; set; }
        public string IPAddress { get; set; }
    }
    public class AddSubReqProcessor : ISqlProcessor<AddSubReqCommand>
    {
        private readonly ILoggerAdaptor<AddSubReqProcessor> _logAdaptor;

        public AddSubReqProcessor(ILogger<AddSubReqProcessor> logger)
        {
            _logAdaptor = new LoggerAdaptor<AddSubReqProcessor>(logger);
        }

        public ValueTask<ProcessResult> Validate(AddSubReqCommand command)
        {




            if (command.AddSubReqModel.URLName == null || string.IsNullOrEmpty(command.AddSubReqModel.URLName.Trim()))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("SubscribeRequest", "URLName is required"));
            }

           

            return new ValueTask<ProcessResult>(ProcessResult.Success());
        }

        public async Task<ProcessResult> Execute(AddSubReqCommand command)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@UserId", command.OperatorUserId.UserId),
                    new SqlParameter("@SRequestId", command.AddSubReqModel.SRequestID),
                    new SqlParameter("@URLName", command.AddSubReqModel.URLName.Trim())
                };

                string result = await SqlService.ExecuteSpWithSingleReturnAsync("Subscribe_Request_InsertUpdate", parameters);
                return ResponseGenerator.Generator(result, "Subscribe_Request");
            }
            catch (Exception ex)
            {
                _logAdaptor.LogError("Exception: Add City Master : {ex.Message}:{command}", ex.Message, JsonSerializer.Serialize(command));
                return ProcessResult.Fail("server", ex.Message);
            }
        }
    }
}
