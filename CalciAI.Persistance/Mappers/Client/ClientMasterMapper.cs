
using CalciAI.Models.client;
using CalciAI.Persistance.Entities.Common;

namespace CalciAI.Persistance.Mappers.Client
{
    public static class ClientMasterMapper
    {
        public static CityMasterModel MapCityToModel(CityMaster doc)
        {
            if (doc == null)
            {
                return null;
            }

            var dest = new CityMasterModel
            {
                CityID = doc.CityID,
                CityName = doc.CityName
            };

            return dest;
        }

        public static ProductMasterModel MapProductToModel(ProductMaster doc)
        {
            if (doc == null)
            {
                return null;
            }

            var dest = new ProductMasterModel
            {
                ProductID = doc.ProductID,
                ProductName = doc.ProductName
            };

            return dest;
        }
    }
}
