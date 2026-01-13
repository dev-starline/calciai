using CalciAI.Models;
using CalciAI.Models.Admin;
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

namespace CalciAI.CommonManager.CommandProcessor.UserMaster
{
    public class AddDomainCommand : ISqlCommand
    {
        public OperatorUserId OperatorUserId { get; set; }
        public DomainMasterModel AddDomainMasterModel { get; set; }
        public string SecretKey { get; set; }
        public string IPAddress { get; set; }
    }
    public class AddDomainProcessor : ISqlProcessor<AddDomainCommand>
    {
        private readonly ILoggerAdaptor<AddDomainProcessor> _logAdaptor;

        public AddDomainProcessor(ILogger<AddDomainProcessor> logger)
        {
            _logAdaptor = new LoggerAdaptor<AddDomainProcessor>(logger);
        }

        public ValueTask<ProcessResult> Validate(AddDomainCommand command)
        {




            if (command.AddDomainMasterModel.DomainName == null || string.IsNullOrEmpty(command.AddDomainMasterModel.DomainName.Trim()))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("DomainMaster", "Name is required"));
            }

            if (command.AddDomainMasterModel.URL == null || string.IsNullOrEmpty(command.AddDomainMasterModel.URL.Trim()))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("DomainMaster", "URL is required"));
            }

            if (command.AddDomainMasterModel.Fetch_Type == null || string.IsNullOrEmpty(command.AddDomainMasterModel.Fetch_Type.Trim()))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("DomainMaster", "Seperator is required"));
            }

            if (command.AddDomainMasterModel.Fetch_Type == "HTML")
            {
                if (command.AddDomainMasterModel.Target_Point == null || string.IsNullOrEmpty(command.AddDomainMasterModel.Target_Point.Trim()))
                {
                    return new ValueTask<ProcessResult>(ProcessResult.Fail("DomainMaster", "Target is required"));
                }

                if (command.AddDomainMasterModel.Target_Mode == null || string.IsNullOrEmpty(command.AddDomainMasterModel.Target_Mode.Trim()))
                {
                    return new ValueTask<ProcessResult>(ProcessResult.Fail("DomainMaster", "Mode is required"));
                }
            }

            return new ValueTask<ProcessResult>(ProcessResult.Success());
        }

        public async Task<ProcessResult> Execute(AddDomainCommand command)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@UserId", command.OperatorUserId.UserId),
                    new SqlParameter("@DomainID", command.AddDomainMasterModel.DomainID),
                    new SqlParameter("@DomainName", command.AddDomainMasterModel.DomainName.Trim()),
                    new SqlParameter("@URL", command.AddDomainMasterModel.URL.Trim()),
                    new SqlParameter("@Fetch_Type", command.AddDomainMasterModel.Fetch_Type.Trim()),
                    new SqlParameter("@Target_Point", command.AddDomainMasterModel.Target_Point),
                    new SqlParameter("@Target_Mode", command.AddDomainMasterModel.Target_Mode),
                };

                string result = await SqlService.ExecuteSpWithSingleReturnAsync("Domain_InsertUpdate", parameters);
                return ResponseGenerator.Generator(result, "Domain");
            }
            catch (Exception ex)
            {
                _logAdaptor.LogError("Exception: Add Client Master : {ex.Message}:{command}", ex.Message, JsonSerializer.Serialize(command));
                return ProcessResult.Fail("server", ex.Message);
            }
        }
    }
}
