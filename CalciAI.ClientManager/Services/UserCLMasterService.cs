using CalciAI.Models;
using CalciAI.Models.Admin;
using CalciAI.Persistance.Constants;
using CalciAI.Persistance;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalciAI.Persistance.Mappers.Admin;
using CalciAI.Persistance.Entities.Common;
using CalciAI.Models.client;

namespace CalciAI.CommonManager.Services
{
    public interface IUserCLMasterService : IService
    {


        Task<ProcessResult<CityMasterModel>> GetByCityIdAsync(string operatorID, int cityId);
        Task<ProcessResult<ProductMasterModel>> GetByProductIdAsync(string operatorID, int prouctId);

        Task<ProcessResult<SubscribeMasterModel>> GetBySubscribeIdAsync(string operatorID, int subscribeId);

        Task<ProcessResult<SubscribeRequestModel>> GetBySubscribeReqIdAsync(string operatorID, int subscribeReqId);
    }
    public class UserCLMasterService : IUserCLMasterService
    {
        public UserCLMasterService()
        {

        }
       
        public async Task<ProcessResult<CityMasterModel>> GetByCityIdAsync(string operatorID, int cityId)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@CityID", cityId),

                };

                var data = await SqlService.ExecuteReaderAsync<CityMaster>("City_GetDetailsByID", parameters);

                var mappedClient = ClientMasterMapper.MapCityToModel(data);

                return ProcessResult<CityMasterModel>.Success(mappedClient);
            }
            catch (Exception ex)
            {
                return ProcessResult<CityMasterModel>.Fail("Exception", ex.Message);
            }
        }

        public async Task<ProcessResult<ProductMasterModel>> GetByProductIdAsync(string operatorID, int prouctId)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ProductID", prouctId),

                };

                var data = await SqlService.ExecuteReaderAsync<ProductMaster>("Product_GetDetailsByID", parameters);

                var mappedClient = ClientMasterMapper.MapProductToModel(data);

                return ProcessResult<ProductMasterModel>.Success(mappedClient);
            }
            catch (Exception ex)
            {
                return ProcessResult<ProductMasterModel>.Fail("Exception", ex.Message);
            }
        }

        public async Task<ProcessResult<SubscribeMasterModel>> GetBySubscribeIdAsync(string operatorID, int subscribeID)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@SubscribeID", subscribeID),

                };

                var data = await SqlService.ExecuteReaderAsync<SubscribeMaster>("Subscribe_GetDetailsByID", parameters);

                var mappedClient = ClientMasterMapper.MapSubscribeToModel(data);

                return ProcessResult<SubscribeMasterModel>.Success(mappedClient);
            }
            catch (Exception ex)
            {
                return ProcessResult<SubscribeMasterModel>.Fail("Exception", ex.Message);
            }
        }

        public async Task<ProcessResult<SubscribeRequestModel>> GetBySubscribeReqIdAsync(string operatorID, int subscribeReqId)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@SRequestId", subscribeReqId),

                };

                var data = await SqlService.ExecuteReaderAsync<SubscribeReqMaster>("Subscribe_Request_GetDetailsByID", parameters);

                var mappedClient = ClientMasterMapper.MapSubReqToModel(data);

                return ProcessResult<SubscribeRequestModel>.Success(mappedClient);
            }
            catch (Exception ex)
            {
                return ProcessResult<SubscribeRequestModel>.Fail("Exception", ex.Message);
            }
        }
    }
   
}
