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
    public class UpdateCityCommand : ISqlCommand
    {
        public OperatorUserId OperatorUserId { get; set; }
        public CityMasterModel AddCityMasterModel { get; set; }
        public string SecretKey { get; set; }
        public string IPAddress { get; set; }
    }
    public class UpdateCityProcessor : ISqlProcessor<UpdateCityCommand>
    {
        private readonly ILoggerAdaptor<UpdateCityProcessor> _logAdaptor;

        public UpdateCityProcessor(ILogger<UpdateCityProcessor> logger)
        {
            _logAdaptor = new LoggerAdaptor<UpdateCityProcessor>(logger);
        }

        public ValueTask<ProcessResult> Validate(UpdateCityCommand command)
        {
            if (command.AddCityMasterModel.CityName == null || string.IsNullOrEmpty(command.AddCityMasterModel.CityName.Trim()))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("DomainMaster", "City is required"));
            }

            return new ValueTask<ProcessResult>(ProcessResult.Success());
        }

        public async Task<ProcessResult> Execute(UpdateCityCommand command)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@UserId", command.OperatorUserId.UserId),
                    new SqlParameter("@CityID", command.AddCityMasterModel.CityID),
                    new SqlParameter("@CityName", command.AddCityMasterModel.CityName.Trim())
                };

                string result = await SqlService.ExecuteSpWithSingleReturnAsync("City_InsertUpdate", parameters);
                return ResponseGenerator.Generator(result, "City");
            }
            catch (Exception ex)
            {
                _logAdaptor.LogError("Exception: Update City Master : {ex.Message}:{command}", ex.Message, JsonSerializer.Serialize(command));
                return ProcessResult.Fail("server", ex.Message);
            }
        }
    }
}
