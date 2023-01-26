using System;
using System.Linq;
using RateLimiter.Models;
using RateLimiter.Models.Enum;
using RateLimiter.Resource;
using RateLimiter.Storage;
using RateLimiter.Storage.Contracts;

namespace RateLimiter
{
    public class Controller
    {
        private readonly IRuleStorage _rulesStorage;

        private readonly IResourceManager _resourceManager1;
        private readonly IResourceManager _resourceManager2;
        private readonly IResourceManager _resourceManager3;
        
        public Controller(IResourceManager resourceManager1, 
            IResourceManager resourceManager2, IResourceManager resourceManager3, IRuleStorage rulesStorage)
        {
            _resourceManager1 = resourceManager1;
            _resourceManager2 = resourceManager2;
            _resourceManager3 = resourceManager3;
            _rulesStorage = rulesStorage;
        }

        public Responce GetResource1Data(string clientKey)
        {
            return BaseCall(clientKey, ResourceType.Res1, _resourceManager1.GetData);
        }

        public Responce GetResource2Data(string clientKey)
        {
            return BaseCall(clientKey, ResourceType.Res2, _resourceManager2.GetData);
        }
        
        public Responce GetResource3Data(string clientKey)
        {
            return BaseCall(clientKey, ResourceType.Res3, _resourceManager3.GetData);
        }

        private Responce BaseCall(string clientKey, ResourceType resource, Func<string> request)
        {
            var responce = new Responce();
            var clientStatistic = ClientStorage.Instance.RefreshClientStatistic(clientKey);
            var rules = _rulesStorage.GetRule(resource);
            if (!rules.All(r => r.ValidateRuleByStatistic(clientStatistic)))
            {
                responce.ResponceType = ResponceType.Fail;
            }
            else
            {
                responce.ResponceType = ResponceType.Success;
                responce.Data = request();
            }
            return responce;
        }
    }
}