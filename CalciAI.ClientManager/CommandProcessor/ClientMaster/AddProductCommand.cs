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
    public class AddProductCommand : ISqlCommand
    {
        public OperatorUserId OperatorUserId { get; set; }
        public ProductMasterModel AddProductMasterModel { get; set; }
        public string SecretKey { get; set; }
        public string IPAddress { get; set; }
    }
    public class AddProductProcessor : ISqlProcessor<AddProductCommand>
    {
        private readonly ILoggerAdaptor<AddProductProcessor> _logAdaptor;

        public AddProductProcessor(ILogger<AddProductProcessor> logger)
        {
            _logAdaptor = new LoggerAdaptor<AddProductProcessor>(logger);
        }

        public ValueTask<ProcessResult> Validate(AddProductCommand command)
        {




            if (command.AddProductMasterModel.ProductName == null || string.IsNullOrEmpty(command.AddProductMasterModel.ProductName.Trim()))
            {
                return new ValueTask<ProcessResult>(ProcessResult.Fail("DomainMaster", "City is required"));
            }

           

            return new ValueTask<ProcessResult>(ProcessResult.Success());
        }

        public async Task<ProcessResult> Execute(AddProductCommand command)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@UserId", command.OperatorUserId.UserId),
                    new SqlParameter("@ProductID", command.AddProductMasterModel.ProductID),
                    new SqlParameter("@ProductName", command.AddProductMasterModel.ProductName.Trim())
                };

                string result = await SqlService.ExecuteSpWithSingleReturnAsync("Product_InsertUpdate", parameters);
                return ResponseGenerator.Generator(result, "Product");
            }
            catch (Exception ex)
            {
                _logAdaptor.LogError("Exception: Add Product Master : {ex.Message}:{command}", ex.Message, JsonSerializer.Serialize(command));
                return ProcessResult.Fail("server", ex.Message);
            }
        }
    }
}
