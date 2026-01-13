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
    public interface IUserMasterService : IService
    {


        Task<ProcessResult<AddClientMasterModel>> GetByClientIdAsync(string operatorID, int ClientMasterID);
        Task<ProcessResult<DomainMasterModel>> GetByDomainIdAsync(string operatorID, int domainId);

        //Task<ProcessResult<CityMasterModel>> GetByCityIdAsync(string operatorID, int ClientMasterID);
        //Task<ProcessResult<ProductMasterModel>> GetByProductIdAsync(string operatorID, int domainId);
    }
    public class UserMasterService : IUserMasterService
    {
        public UserMasterService()
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

        public async Task<ProcessResult<DomainMasterModel>> GetByDomainIdAsync(string operatorID, int domainId)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@DomainID", domainId),

                };

                var data = await SqlService.ExecuteReaderAsync<DomainMaster>("DomainMaster_GetDomainByID", parameters);

                var mappedClient = ClientMasterMapper.MapDomainToModel(data);

                return ProcessResult<DomainMasterModel>.Success(mappedClient);
            }
            catch (Exception ex)
            {
                return ProcessResult<DomainMasterModel>.Fail("Exception", ex.Message);
            }
        }
        //public async Task<ProcessResult<CityMasterModel>> GetByCityIdAsync(string operatorID, int ClientMasterID)
        //{
        //    try
        //    {
        //        SqlParameter[] parameters =
        //        {
        //            new SqlParameter("@CityID", ClientMasterID),

        //        };

        //        var data = await SqlService.ExecuteReaderAsync<CityMaster>("City_GetDetailsByID", parameters);

        //        var mappedClient = ClientMasterMapper.MapCityToModel(data);

        //        return ProcessResult<CityMasterModel>.Success(mappedClient);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ProcessResult<CityMasterModel>.Fail("Exception", ex.Message);
        //    }
        //}

        //public async Task<ProcessResult<ProductMasterModel>> GetByProductIdAsync(string operatorID, int domainId)
        //{
        //    try
        //    {
        //        SqlParameter[] parameters =
        //        {
        //            new SqlParameter("@ProductID", domainId),

        //        };

        //        var data = await SqlService.ExecuteReaderAsync<ProductMaster>("Product_GetDomainByID", parameters);

        //        var mappedClient = ClientMasterMapper.MapProductToModel(data);

        //        return ProcessResult<ProductMasterModel>.Success(mappedClient);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ProcessResult<ProductMasterModel>.Fail("Exception", ex.Message);
        //    }
        //}
    }
   
}
