using CalciAI.Models;
using CalciAI.Models.Admin;
using CalciAI.Persistance;
using CalciAI.Persistance.Constants;
using Microsoft.Extensions.Logging;
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
    public class UpdateClientCommand : ISqlCommand
    {
        public OperatorUserId OperatorUserId { get; set; }
        public AddClientMasterModel AddClientMasterModel { get; set; }
        public string SecretKey { get; set; }
        public string IPAddress { get; set; }
    }
    public class UpdateClientProcessor : ISqlProcessor<UpdateClientCommand>
    {
        private readonly ILoggerAdaptor<UpdateClientProcessor> _logAdaptor;

        public UpdateClientProcessor(ILogger<UpdateClientProcessor> logger)
        {
            _logAdaptor = new LoggerAdaptor<UpdateClientProcessor>(logger);
        }

        public ValueTask<ProcessResult> Validate(UpdateClientCommand command)
        {


            if (command.AddClientMasterModel.ClientID == null || command.AddClientMasterModel.ClientID == 0)
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("ClientMaster", "Client id is required"));
            }

            if (command.AddClientMasterModel.ClientName == null || string.IsNullOrEmpty(command.AddClientMasterModel.ClientName.Trim()))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("ClientMaster", "Client name is required"));
            }

            if (command.AddClientMasterModel.Mobile == null || string.IsNullOrEmpty(command.AddClientMasterModel.Mobile.Trim()))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("ClientMaster", "Mobile is required"));
            }

            if (command.AddClientMasterModel.End_Date == null || command.AddClientMasterModel.End_Date == DateTime.MinValue)
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("ClientMaster", "End date is required"));
            }

            if (command.AddClientMasterModel.Status.ToString() == null)
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("ClientMaster", "Status is required"));
            }


            //Regex regEx = new("^[0-9]*$");
            //bool IsNumeric = regEx.IsMatch(command.ClientMasterModel.ClientID);
            //if (!IsNumeric)
            //{
            //    return new ValueTask<ProcessResult>(ProcessResult.Fail("ClientMaster", "Only digits allowed !!"));
            //}

            return new ValueTask<ProcessResult>(ProcessResult.Success());
        }

        public async Task<ProcessResult> Execute(UpdateClientCommand command)
        {
            try
            {
              

                



                SqlParameter[] parameters =
                {
                    new SqlParameter("@UserId", command.OperatorUserId.UserId),
                    new SqlParameter("@ClientMasterID", command.AddClientMasterModel.ClientMasterID),
                    new SqlParameter("@ClientID", command.AddClientMasterModel.ClientID),
                    new SqlParameter("@ClientName", command.AddClientMasterModel.ClientName.Trim().Replace("'","''")),
                   // new SqlParameter("@UserPassword",userPass),
                    new SqlParameter("@Company", command.AddClientMasterModel.Company),
                    new SqlParameter("@City", command.AddClientMasterModel.City),
                    new SqlParameter("@Mobile", command.AddClientMasterModel.Mobile),
                    new SqlParameter("@Start_Date", "1900-01-01"),
                    new SqlParameter("@End_Date", command.AddClientMasterModel.End_Date),
                    new SqlParameter("@Status", command.AddClientMasterModel.Status)
                };

                string result = await SqlService.ExecuteSpWithSingleReturnAsync("Clientmaster_InsertUpdate", parameters);
                return ResponseGenerator.Generator(result, "ClientMaster");
            }
            catch (Exception ex)
            {
                _logAdaptor.LogError("Exception: Add Client Master : {ex.Message}:{command}", ex.Message, JsonSerializer.Serialize(command));
                return ProcessResult.Fail("server", ex.Message);
            }
        }
    }
}
