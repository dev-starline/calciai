using CalciAI.Models;
using CalciAI.Models.Admin;
using CalciAI.Models.client;
using CalciAI.Persistance;
using CalciAI.Persistance.Constants;
using Microsoft.Extensions.Logging;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CalciAI.CommonManager.CommandProcessor.ClientMaster
{
    public class UpdateSubscribeCommand : ISqlCommand
    {
        public OperatorUserId OperatorUserId { get; set; }
        public SubscribeMasterModel AddSubscribeMasterModel { get; set; }
        public string SecretKey { get; set; }
        public string IPAddress { get; set; }
    }
    public class UpdateSubscribeProcessor : ISqlProcessor<UpdateSubscribeCommand>
    {
        private readonly ILoggerAdaptor<UpdateSubscribeProcessor> _logAdaptor;

        public UpdateSubscribeProcessor(ILogger<UpdateSubscribeProcessor> logger)
        {
            _logAdaptor = new LoggerAdaptor<UpdateSubscribeProcessor>(logger);
        }

        public ValueTask<ProcessResult> Validate(UpdateSubscribeCommand command)
        {
            if (command.AddSubscribeMasterModel.CityName == null || string.IsNullOrEmpty(command.AddSubscribeMasterModel.CityName.Trim()))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("DomainMaster", "City is required"));
            }
            if (command.AddSubscribeMasterModel.ProductName == null || string.IsNullOrEmpty(command.AddSubscribeMasterModel.ProductName.Trim()))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("DomainMaster", "Product is required"));
            }
            if (command.AddSubscribeMasterModel.URLName == null || string.IsNullOrEmpty(command.AddSubscribeMasterModel.URLName.Trim()))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("URL", "City is required"));
            }


            if (command.AddSubscribeMasterModel.SelectedProduct == null || string.IsNullOrEmpty(command.AddSubscribeMasterModel.SelectedProduct.Trim()))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("DomainMaster", "City is required"));
            }
            if (command.AddSubscribeMasterModel.GST == null)
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("DomainMaster", "GST is required"));
            }

            return new ValueTask<ProcessResult>(ProcessResult.Success());
        }

        public async Task<ProcessResult> Execute(UpdateSubscribeCommand command)
        {
            try
            {
                SqlParameter[] parameters =
                 {
                    new SqlParameter("@UserId", command.OperatorUserId.UserId),
                    new SqlParameter("@CityID", command.AddSubscribeMasterModel.CityID),
                    new SqlParameter("@CityName", command.AddSubscribeMasterModel.CityName.Trim()),
                    new SqlParameter("@ProductID", command.AddSubscribeMasterModel.ProductID),
                    new SqlParameter("@ProductName", command.AddSubscribeMasterModel.ProductName.Trim()),
                    new SqlParameter("@URLID", command.AddSubscribeMasterModel.URLID),
                    new SqlParameter("@URLName", command.AddSubscribeMasterModel.URLName.Trim()),
                    new SqlParameter("@SelectedProduct", command.AddSubscribeMasterModel.SelectedProduct.Trim()),
                    new SqlParameter("@GST", command.AddSubscribeMasterModel.GST)
                };

                string result = await SqlService.ExecuteSpWithSingleReturnAsync("Subscribe_InsertUpdate", parameters);
                return ResponseGenerator.Generator(result, "Subscribe");
            }
            catch (Exception ex)
            {
                _logAdaptor.LogError("Exception: Update Subscribe Master : {ex.Message}:{command}", ex.Message, JsonSerializer.Serialize(command));
                return ProcessResult.Fail("server", ex.Message);
            }
        }
    }
}
