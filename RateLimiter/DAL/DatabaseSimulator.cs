using RateLimiter.BLL;
using RateLimiter.Entities;
using RateLimiter.Entities.Rules;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.DAL
{
    //simulate database
    public static class DatabaseSimulator
    {
        public static List<RequestModel> Requests = new List<RequestModel>
        {
            new RequestModel { ClientToken = "token1" },
            new RequestModel { ClientToken = "token2" },
            new RequestModel { ClientToken = "token3" }
        };

        public static List<CountryModel> Countries = new List<CountryModel>
        {
            new CountryModel { CountryId = 1, CountryName = "USA" },
            new CountryModel { CountryId = 2, CountryName = "EU" },
            new CountryModel { CountryId = 3, CountryName = "Belarus" },
            new CountryModel { CountryId = 4, CountryName = "Russia" }
        };

        public static List<PeriodModel> Periods = new List<PeriodModel>
        {
            new PeriodModel { PeriodId = 1, StartPeriod = DateTime.Now, EndPeriod = DateTime.Now.AddMinutes(1) },
            new PeriodModel { PeriodId = 2, StartPeriod = DateTime.Now, EndPeriod = DateTime.Now.AddSeconds(30) },
            new PeriodModel { PeriodId = 3, StartPeriod = DateTime.Now, EndPeriod = DateTime.Now.AddDays(1) },
            new PeriodModel { PeriodId = 2, StartPeriod = DateTime.Now, EndPeriod = DateTime.Now.AddSeconds(5) }
        };

        public static List<object> Rules = new List<object>
        {
            new CountryRuleModel { RuleId = 1, RuleType = RuleTypesEnum.Country, CountrId = 1 },
            new LastCallRuleModel { RuleId = 2, RuleType = RuleTypesEnum.LastCall, Limits = new LimitPeriodModel { DaysLimit = 1 } },
            new LastCallForCountryRuleModel { RuleId = 3, RuleType = RuleTypesEnum.LastCallForCountry, Limits = new LimitPeriodModel { HoursLimit = 1 }, CountryId = 2 },
            new RequestCountForPeriodRuleModel { RuleId = 4, RuleType = RuleTypesEnum.Period, Period = Periods.ElementAt(2), MaxRequestsCount = 1 },
            new RequestCountRuleModel { RuleId = 5, RuleType = RuleTypesEnum.RequestCount, MaxRequestsCount = 1 },
            new RequestCountForCountryRuleModel { RuleId = 6, RuleType = RuleTypesEnum.RequestCountForCountry, CountryId = 3, MaxRequestsCount = 1 },
            new RequestCountForCountryForPeriodRuleModel { RuleId = 7, RuleType = RuleTypesEnum.RequestCountForCountryForPeriod, CountryId = 1, MaxRequestsCount = 2, Period = Periods.ElementAt(2) },
            
            new CountryRuleModel { RuleId = 8, RuleType = RuleTypesEnum.Country, CountrId = 1 },
            new RequestCountForPeriodRuleModel { RuleId = 9, RuleType = RuleTypesEnum.Period, Period = Periods.ElementAt(3), MaxRequestsCount = 3 },
            
            new CountryRuleModel { RuleId = 10, RuleType = RuleTypesEnum.Country, CountrId = 2 },
            new PassedTimeRuleModel { RuleId = 11, RuleType = RuleTypesEnum.LastCall, PassedTime = new LimitPeriodModel { MinutesLimit = 1 } }

        };

        public static List<ClientModel> Clients = new List<ClientModel>
        {
            new ClientModel { Country = Countries.ElementAt(0), LastRequestId = 0, LastCallDate = null, Token = "token1", Rules = Rules.Take(1).ToList() },
            new ClientModel { Country = Countries.ElementAt(1), LastRequestId = 0, LastCallDate = null, Token = "token2", Rules = Rules.Take(1).ToList() },

            new ClientModel { Country = Countries.ElementAt(0), LastRequestId = 0, LastCallDate = null, Token = "token3", Rules = Rules.Skip(1).Take(1).ToList() },

            new ClientModel { Country = Countries.ElementAt(1), LastRequestId = 0, LastCallDate = null, Token = "token4", Rules = Rules.Skip(2).Take(1).ToList() },
            new ClientModel { Country = Countries.ElementAt(1), LastRequestId = 0, LastCallDate = null, Token = "token5", Rules = Rules.Skip(2).Take(1).ToList() },

            new ClientModel { Country = Countries.ElementAt(2), LastRequestId = 0, LastCallDate = null, Token = "token6", Rules = Rules.Skip(3).Take(1).ToList() },
            new ClientModel { Country = Countries.ElementAt(2), LastRequestId = 0, LastCallDate = null, Token = "token7", Rules = Rules.Skip(3).Take(1).ToList() },

            new ClientModel { Country = Countries.ElementAt(2), LastRequestId = 0, LastCallDate = null, Token = "token8", Rules = Rules.Skip(4).Take(1).ToList() },
            new ClientModel { Country = Countries.ElementAt(2), LastRequestId = 0, LastCallDate = null, Token = "token9", Rules = Rules.Skip(4).Take(1).ToList() },

            new ClientModel { Country = Countries.ElementAt(1), LastRequestId = 0, LastCallDate = null, Token = "token10", Rules = Rules.Skip(5).Take(1).ToList() },
            new ClientModel { Country = Countries.ElementAt(2), LastRequestId = 0, LastCallDate = null, Token = "token11", Rules = Rules.Skip(5).Take(1).ToList() },

            new ClientModel { Country = Countries.ElementAt(0), LastRequestId = 0, LastCallDate = null, Token = "token12", Rules = Rules.Skip(6).Take(1).ToList() },

            new ClientModel { Country = Countries.ElementAt(0), LastRequestId = 0, LastCallDate = null, Token = "token13", Rules = Rules.Skip(7).Take(2).ToList() },
            new ClientModel { Country = Countries.ElementAt(1), LastRequestId = 0, LastCallDate = null, Token = "token14", Rules = Rules.Skip(9).Take(2).ToList() }
        };

        public static List<ClientRequestModel> ClientRequests = new List<ClientRequestModel>();
    }
}
