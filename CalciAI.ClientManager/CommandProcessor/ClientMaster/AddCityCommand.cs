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
    public class AddCityCommand : ISqlCommand
    {
        public OperatorUserId OperatorUserId { get; set; }
        public CityMasterModel AddCityMasterModel { get; set; }
        public string SecretKey { get; set; }
        public string IPAddress { get; set; }
    }
    public class AddCityProcessor : ISqlProcessor<AddCityCommand>
    {
        private readonly ILoggerAdaptor<AddCityProcessor> _logAdaptor;

        public AddCityProcessor(ILogger<AddCityProcessor> logger)
        {
            _logAdaptor = new LoggerAdaptor<AddCityProcessor>(logger);
        }

        public ValueTask<ProcessResult> Validate(AddCityCommand command)
        {




            if (command.AddCityMasterModel.CityName == null || string.IsNullOrEmpty(command.AddCityMasterModel.CityName.Trim()))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("DomainMaster", "City is required"));
            }

           

            return new ValueTask<ProcessResult>(ProcessResult.Success());
        }

        public async Task<ProcessResult> Execute(AddCityCommand command)
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
                _logAdaptor.LogError("Exception: Add City Master : {ex.Message}:{command}", ex.Message, JsonSerializer.Serialize(command));
                return ProcessResult.Fail("server", ex.Message);
            }
        }
    }
}
