using Microsoft.Extensions.Logging;
using CalciAI.Net;
using System.Collections.Generic;

namespace CalciAI.Persistance
{
    public static class ClientProvider
    {
        public static ClientService GetClient(ILogger logger, string operatorId)
        {
            var config = new HttpClientConfig
            {
                Name = operatorId,
                BaseUrl = OperatorProvider.GetS2S(operatorId),
                TimeOut = 5,
                AuthRequired = false,
                BaseHeaders = new Dictionary<string, string> { { "X-OP-KEY", operatorId }, { "X-OP-SECRET", OperatorProvider.GetSecretByKey(operatorId) } },
                ExtraParams = new Dictionary<string, object>()
            };

            var client = new ClientService(logger, config);

            return client;
        }
    }
}