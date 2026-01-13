using Microsoft.Extensions.Logging;
using CalciAI.Net;
using System.Collections.Generic;

namespace CalciAI.Persistance
{
    public static class DuroPeProvider
    {
        public static ClientService GetDuroPe(ILogger logger, string operatorID)
        {
            HttpClientConfigWithEncrypt _configuration = FileConfigProvider.Load<HttpClientConfigWithEncrypt>("durope");

            string XopSecret = "lgk8sQVHZjvKvGeq5c0iIA==";
            string XopSecretValue = EncryptionDecryption.DecryptText(_configuration.BaseHeaders[XopSecret]);

            var config = new HttpClientConfig
            {
                Name = operatorID,
                BaseUrl = EncryptionDecryption.DecryptText(_configuration.BaseUrl), //"http://localhost:5000",
                TimeOut = _configuration.TimeOut,
                AuthRequired = false,
                BaseHeaders = new Dictionary<string, string> { { "X-OP-KEY", operatorID }, { "X-OP-SECRET", XopSecretValue } },
                ExtraParams = new Dictionary<string, object>()
            };

            var master = new ClientService(logger, config);

            return master;
        }
    }
}