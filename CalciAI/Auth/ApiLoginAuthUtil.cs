using CalciAI.Extentions;
using CalciAI.Helpers;
using CalciAI.Models;
using JWT.Algorithms;
using JWT.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalciAI.Auth
{
    public static class ApiLoginAuthUtil
    {
        public static ProcessResult<ApiLoginTokenData> DecodeJwtToken(string token, string secret, bool verify = true)
        {
            try
            {
                var secretKeyBytes = Encoding.UTF8.GetBytes(secret);

                var builder = new JwtBuilder();
                var algorithm = new HMACSHA256Algorithm();

                var jwt = builder.WithAlgorithm(algorithm)
                                 .WithSecret(secretKeyBytes)
                                 .WithVerifySignature(verify)
                                 .Decode<Dictionary<string, object>>(token);

                return ProcessResult<ApiLoginTokenData>.Success(new ApiLoginTokenData
                {
                    Username = jwt["sub"].ToString(),
                    Role = jwt["role"].ToString(),
                    Operator = jwt["iss"].ToString(),
                    IpAddress = jwt["ip"].ToString()
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"INVALID: {token}. ERROR: {e.Message}");
                return ProcessResult<ApiLoginTokenData>.Fail(new Error("Token", e.Message));
            }
        }

        public static string EncodeToken(ApiLoginTokenData payload, string secret)
        {
            var secretKeyBytes = Encoding.UTF8.GetBytes(secret);

            var builder = new JwtBuilder();
            var algorithm = new HMACSHA256Algorithm();

            var token = builder.WithAlgorithm(algorithm)
                               .WithSecret(secretKeyBytes)
                               .AddClaim("sub", payload.Username)
                               .AddClaim("role", payload.Role)
                               .AddClaim("iss", payload.Operator)
                               .AddClaim("ip", payload.IpAddress)
                               .Encode();
            return token;
        }
    }
}
