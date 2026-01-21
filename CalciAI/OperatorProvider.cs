using CalciAI.Caching;
using CalciAI.Models;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace CalciAI
{
    public static class OperatorProvider
    {
        private static readonly ILogger _logger = ApplicationLogging.CreateLogger("OperatorProvider");

        private static readonly ConcurrentDictionary<string, OperatorDetail> _detailMap = new();

        private static readonly ConcurrentDictionary<string, (string, string)> _hostMap = new();

        public static bool IsMaster { get; private set; }

        private static Dictionary<string, dynamic> Map(OperatorDetail detail)
        {
            var dict = new Dictionary<string, dynamic>
            {
                { OperatorDetail.KEY_CODE, detail.Code },
                { OperatorDetail.KEY_SECRET, detail.Secret},
                { OperatorDetail.KEY_PSECRET, detail.PartnerSecret},
                { OperatorDetail.KEY_IS_PRIMARY, detail.IsPrimary},
                { OperatorDetail.KEY_PRIMARY_DOMAIN, detail.PrimaryDomain},
                { OperatorDetail.KEY_IS_ACTIVE, detail.IsActive }
            };

            if (detail.OtherDomains != null && detail.OtherDomains.Length > 0)
            {
                dict.Add(OperatorDetail.KEY_OTHER_DOMAIN, string.Join(",", detail.OtherDomains));
            }

            return dict;
        }

        public static OperatorUserId GetSystemUser()
        {
            var user = new OperatorUserId
            {
                Username = "SYSTEM",
                //OperatorId = GetMasterKey()
            };

            return user;
        }

        public static async Task<OperatorDetail> Get(string code)
        {
            var keys = await RedisStore.RedisCache.HashGetAllAsync($"op.{code}");
            return Cast(keys);
        }

        private static OperatorDetail Cast(HashEntry[] entries)
        {
            var detail = new OperatorDetail();

            foreach (var item in entries)
            {
                switch (item.Name)
                {
                    case OperatorDetail.KEY_CODE:
                        detail.Code = item.Value;
                        break;
                    case OperatorDetail.KEY_SECRET:
                        detail.Secret = item.Value;
                        break;
                    case OperatorDetail.KEY_PSECRET:
                        detail.PartnerSecret = item.Value;
                        break;
                    case OperatorDetail.KEY_IS_PRIMARY:
                        detail.IsPrimary = (bool)item.Value;
                        break;
                    case OperatorDetail.KEY_PRIMARY_DOMAIN:
                        detail.PrimaryDomain = item.Value;
                        break;
                    case OperatorDetail.KEY_OTHER_DOMAIN:
                        detail.OtherDomains = item.Value.ToString().Split(",");
                        break;
                    case OperatorDetail.KEY_IS_ACTIVE:
                        detail.IsActive = (bool)item.Value;
                        break;
                }
            }

            if (detail.OtherDomains == null)
            {
                detail.OtherDomains = Array.Empty<string>();
            }

            return string.IsNullOrEmpty(detail.Code) ? null : detail;
        }

        public static async Task AddOrUpdateOperator(OperatorDetail detail)
        {
            var dictionary = Map(detail);
            var docId = $"op.{detail.Code}";

            await RedisStore.RedisCache.UpdateHashAsync(docId, dictionary);
        }

        public static async Task AddOrUpdateToRedis(string docId, object model)
        {
            var dict = new Dictionary<string, dynamic>();

            Type type = model.GetType();
            PropertyInfo[] props = type.GetProperties();

            foreach (var prop in props)
            {
                dict.Add(prop.Name, prop.GetValue(model));
            }

            await RedisStore.RedisCache.UpdateHashAsync(docId, dict);
        }

        public static ISet<string> GetOperatorIds()
        {
            return _detailMap.Keys.ToHashSet();
        }

        public static string[] GetOperatorsArray()
        {
            return _detailMap.Keys.ToArray();
        }

        public static bool ValidateOperator(string operatorId, string Secret)
        {
            if (!_detailMap.TryGetValue(operatorId, out OperatorDetail operatorDetail))
            {
                return false;
            }

            return Secret == operatorDetail.Secret;
        }

        public static bool ValidatePartner(string operatorId, string Secret)
        {
            if (!_detailMap.TryGetValue(operatorId, out OperatorDetail operatorDetail))
            {
                return false;
            }

            return Secret == operatorDetail.PartnerSecret;
        }

        public static string GetSecretByKey(string operatorId)
        {
            if (!_detailMap.TryGetValue(operatorId, out OperatorDetail operatorDetail))
            {
                return string.Empty;
            }

            return operatorDetail.Secret;
        }

        public static string GetOperatorByDomain(string host)
        {
            if (!_hostMap.TryGetValue(host, out (string, string) tupple))
            {
                return string.Empty;
            }

            return tupple.Item1;
        }

        public static string GetSecretByDomain(string host)
        {
            if (!_hostMap.TryGetValue(host, out (string, string) tupple))
            {
                return string.Empty;
            }

            return tupple.Item2;
        }

        public static string[] GetUrlsForCors()
        {

            var domains = new List<string>
            {
                $"https://tradeprint.in",
                $"http://localhost:3001"
            };

            foreach (var item in _hostMap.Keys)
            {
                domains.Add($"http://calci.{item}");
            }

            //var domains = new List<string>
            //{

            //     $"http://calci.tradeprint.in",
            //      $"https://moctl.com",
            //       $"https://moctl.com",
            //    $"https://moctl.com", 
            //    $"https://client.moctl.com",
            //    $"https://designgopanels.net",
            //    $"https://client.designgopanels.net",
            //    $"https://pay.designgopanels.net",
            //    $"https://tradeprint.in",
            //    $"https://madmin.metaodds.net",
            //    $"https://admin.metaodds.xyz"
            //};

            //foreach (var item in _hostMap.Keys)
            //{
            //    domains.Add($"https://admin.{item}");
            //    domains.Add($"https://dealer.{item}");
            //    domains.Add($"https://client.{item}");
            //    domains.Add($"https://wclient.{item}");
            //    domains.Add($"https://skt.{item}");
            //    domains.Add($"https://sglr.{item}");
            //}

            return domains.ToArray();
        }

        public static string GetS2S(string operatorId)
        {
            if (!_detailMap.TryGetValue(operatorId, out OperatorDetail detail))
            {
                return null;
            }

            return $"https://api.{detail.PrimaryDomain}:6003";
        }

        public static string GetBhmUrl()
        {
            if (!_detailMap.TryGetValue("BHM", out OperatorDetail detail))
            {
                return null;
            }

            return $"https://api-dev.{detail.PrimaryDomain}";
        }

        public static string GetAdmin(string operatorId)
        {
            if (!_detailMap.TryGetValue(operatorId, out OperatorDetail detail))
            {
                return null;
            }

            return $"https://admin.{detail.PrimaryDomain}";
        }

        //For getting master domain key
        public static string GetMasterKey()
        {
            return _detailMap.Values.FirstOrDefault(x => x.IsPrimary).Code;
        }

        //For CalciAI we need S2S urls for all non primary operators
        public static string[] GetS2SUrls()
        {
            return _detailMap.Values.Where(x => !x.IsPrimary).Select(x => $"https://s2s-dev.{x.PrimaryDomain}").ToArray();
        }

        public static async Task Refresh(bool scanDisabledOperator = false)
        {
            IsMaster = RedisStore.RedisCache.StringGet("IS_MASTER") == "1";

            using var scope = _logger.BeginScope("Operator Cache Refresh");

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var keys = await RedisStore.RedisCache.ScanKeys("op.*");

            var detailMap = new Dictionary<string, OperatorDetail>();
            var secretMap = new Dictionary<string, (string, string)>();

            foreach (var key in keys)
            {
                var entries = await RedisStore.RedisCache.HashGetAllAsync(key);

                var detail = Cast(entries);

                if (detail == null || (!scanDisabledOperator && !detail.IsActive))
                {
                    continue;
                }

                detailMap.Add(detail.Code, detail);

                secretMap.Add(detail.PrimaryDomain, (detail.Code, detail.Secret));

                foreach (var domain in detail.OtherDomains)
                {
                    if (string.IsNullOrEmpty(domain))
                    {
                        continue;
                    }

                    secretMap.Add(domain, (detail.Code, detail.Secret));
                }

            }

            foreach (var item in _detailMap.Keys.Except(detailMap.Keys).ToArray())
            {
                _detailMap.TryRemove(item, out _);
            }

            foreach (var detail in detailMap.Values)
            {
                if (_detailMap.TryGetValue(detail.Code, out OperatorDetail operatorDetail))
                {
                    _detailMap.TryUpdate(detail.Code, detail, operatorDetail);
                }
                else
                {
                    _detailMap.TryAdd(detail.Code, detail);
                }
            }

            foreach (var item in _hostMap.Keys.Except(secretMap.Keys).ToArray())
            {
                _hostMap.TryRemove(item, out _);
            }

            foreach (var domain in secretMap.Keys)
            {
                if (_hostMap.TryGetValue(domain, out (string, string) oldValue))
                {
                    _hostMap.TryUpdate(domain, secretMap[domain], oldValue);
                }
                else
                {
                    _hostMap.TryAdd(domain, secretMap[domain]);
                }
            }

            watch.Stop();

            _logger.LogInformation("Operator Refresh Processed: {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
        }
    }
}
