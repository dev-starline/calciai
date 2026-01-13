using CalciAI.Models.Admin;
using CalciAI.Models.client;
using CalciAI.Persistance.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalciAI.Persistance.Mappers.Admin
{
    public static class ClientMasterMapper
    {
        public static AddClientMasterModel MapToModel(ClientMaster doc)
        {
            if (doc == null)
            {
                return null;
            }

            var dest = new AddClientMasterModel
            {
                ClientMasterID = doc.ClientMasterID,
                ClientID = doc.ClientID,
                ClientName = doc.ClientName,
                Company = doc.Company,
                City = doc.City,
                Mobile = doc.Mobile,
                Start_Date = doc.Start_Date,
                End_Date = doc.End_Date,
                Status = doc.Status
            };

            return dest;
        }

        public static DomainMasterModel MapDomainToModel(DomainMaster doc)
        {
            if (doc == null)
            {
                return null;
            }

            var dest = new DomainMasterModel
            {
                DomainID = doc.DomainID,
                DomainName = doc.DomainName,
                URL = doc.URL,
                Fetch_Type = doc.Fetch_Type,
                Target_Point = doc.Target_Point,
                Target_Mode = doc.Target_Mode
            };

            return dest;
        }
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

        public static SubscribeMasterModel MapSubscribeToModel(SubscribeMaster doc)
        {
            if (doc == null)
            {
                return null;
            }

            var dest = new SubscribeMasterModel
            {
                SubscribeID = doc.SubscribeID,
                CityID = doc.CityID,
                CityName = doc.CityName,
                ProductID = doc.ProductID,
                ProductName = doc.ProductName,
                URLID = doc.URLID,
                URLName = doc.URLName,
                SelectedProduct = doc.SelectedProduct,
                GST = doc.GST
            };

            return dest;
        }

        public static SubscribeRequestModel MapSubReqToModel(SubscribeReqMaster doc)
        {
            if (doc == null)
            {
                return null;
            }

            var dest = new SubscribeRequestModel
            {
                SRequestID = doc.SRequestID,
                URLName = doc.URLName
            };

            return dest;
        }
    }
}
