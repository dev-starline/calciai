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

        Task<ProcessResult<AddClientMasterModel>> GetByClientIdAsync(string operatorID, int ClientMasterID);
        Task<ProcessResult<ModelList<DashboardModel>>> GetClientDashboardAsync(string userName);

        Task<ProcessResult<ModelList<CityMasterModel>>> GetAllCityByClientIDAsync(string userName);
        //Task<ProcessResult<CityMasterModel>> GetAllCityByClientIDAsync(string userName);
        Task<ProcessResult<CityMasterModel>> GetByCityIdAsync(string operatorID, int cityId);


        Task<ProcessResult<ModelList<ProductMasterModel>>> GetAllProductIdByClientIDAsync(string userName);
        //Task<ProcessResult<ProductMasterModel>> GetAllProductIdByClientIDAsync(string userName);
        Task<ProcessResult<ProductMasterModel>> GetByProductIdAsync(string operatorID, int prouctId);

        Task<ProcessResult<ModelList<SubscribeMasterModel>>> GetAllSubscribeByClientIdAsync(string userName);
      //  Task<ProcessResult<SubscribeMasterModel>> GetAllSubscribeByClientIdAsync(string userName);

        Task<ProcessResult<SubscribeMasterModel>> GetBySubscribeIdAsync(string operatorID, int subscribeId);


        Task<ProcessResult<ModelList<SubscribeRequestModel>>> GetAllSubscribeReqByClientIdAsync(string userName);
      //  Task<ProcessResult<SubscribeRequestModel>> GetAllSubscribeReqByClientIdAsync(string userName);

        Task<ProcessResult<SubscribeRequestModel>> GetBySubscribeReqIdAsync(string operatorID, int subscribeReqId);
    }
    public class UserCLMasterService : IUserCLMasterService
    {
        public UserCLMasterService()
        {

        }

        public async Task<ProcessResult<AddClientMasterModel>> GetByClientIdAsync(string operatorID, int ClientMasterID)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ClientMasterID", ClientMasterID),

                };

                var data = await SqlService.ExecuteReaderAsync<ClientMaster>("ClientMaster_GetDetailsByID", parameters);

                var mappedClient = ClientMasterMapper.MapToModel(data);

                return ProcessResult<AddClientMasterModel>.Success(mappedClient);
            }
            catch (Exception ex)
            {
                return ProcessResult<AddClientMasterModel>.Fail("Exception", ex.Message);
            }
        }

        public async Task<ProcessResult<ModelList<DashboardModel>>> GetClientDashboardAsync(string userName)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@userName", userName),

                };

                var data = await SqlService.ExecuteReaderListAsync<DashboardMaster>("ClientDashboard_GetCountByClientID", parameters);

                var mappedData = data.Select(x => ClientMasterMapper.MapDashboardToModel(x));
                // var mappedClient = ClientMasterMapper.MapCityToModel(data);
                return ProcessResult<ModelList<DashboardModel>>.Success(new ModelList<DashboardModel>(mappedData));

            }
            catch (Exception ex)
            {
                return ProcessResult<ModelList<DashboardModel>>.Fail("Exception", ex.Message);
            }
        }

        public async Task<ProcessResult<ModelList<CityMasterModel>>> GetAllCityByClientIDAsync(string userName)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@userName", userName),

                };

                var data = await SqlService.ExecuteReaderListAsync<CityMaster>("City_GetAllCityByDetailsByClientID", parameters);

                var mappedData = data.Select(x => ClientMasterMapper.MapCityToModel(x));
                // var mappedClient = ClientMasterMapper.MapCityToModel(data);
                return ProcessResult<ModelList<CityMasterModel>>.Success(new ModelList<CityMasterModel>(mappedData));
               
            }
            catch (Exception ex)
            {
                return ProcessResult<ModelList<CityMasterModel>>.Fail("Exception", ex.Message);
            }
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

       

        public async Task<ProcessResult<ModelList<ProductMasterModel>>> GetAllProductIdByClientIDAsync(string userName)
        {
            try
            {
                SqlParameter[] parameters =
                {
                     new SqlParameter("@userName", userName),

                };

                var data = await SqlService.ExecuteReaderListAsync<ProductMaster>("Product_GetAllProductDetailsByClientID", parameters);
                var mappedClient = data.Select(x => ClientMasterMapper.MapProductToModel(x));
                return ProcessResult<ModelList<ProductMasterModel>>.Success(new ModelList<ProductMasterModel>(mappedClient));
            }
            catch (Exception ex)
            {
                return ProcessResult<ModelList<ProductMasterModel>>.Fail("Exception", ex.Message);
               
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

        public async Task<ProcessResult<ModelList<SubscribeMasterModel>>> GetAllSubscribeByClientIdAsync(string userName)
        //  public async Task<ProcessResult<SubscribeMasterModel>> GetAllSubscribeByClientIdAsync(string userName)
        {
            try
            {
                SqlParameter[] parameters =
                {
                     new SqlParameter("@userName", userName),

                };

                var data = await SqlService.ExecuteReaderListAsync<SubscribeMaster>("Subscribe_GetSubscribeDetailsByClientID", parameters);
                var mappedClient = data.Select(x => ClientMasterMapper.MapSubscribeToModel(x));
                return ProcessResult<ModelList<SubscribeMasterModel>>.Success(new ModelList<SubscribeMasterModel>(mappedClient));
            }
            catch (Exception ex)
            {
                return ProcessResult<ModelList<SubscribeMasterModel>>.Fail("Exception", ex.Message);

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

        public async Task<ProcessResult<ModelList<SubscribeRequestModel>>> GetAllSubscribeReqByClientIdAsync(string userName)
        // public async Task<ProcessResult<SubscribeRequestModel>> GetAllSubscribeReqByClientIdAsync(string userName)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@userName", userName),

                };

                var data = await SqlService.ExecuteReaderListAsync<SubscribeReqMaster>("Subscribe_Request_GetDetailsByClientID", parameters);
                var mappedClient = data.Select(x => ClientMasterMapper.MapSubReqToModel(x));
                return ProcessResult<ModelList<SubscribeRequestModel>>.Success(new ModelList<SubscribeRequestModel>(mappedClient));
            }
            catch (Exception ex)
            {
                return ProcessResult<ModelList<SubscribeRequestModel>>.Fail("Exception", ex.Message);

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
