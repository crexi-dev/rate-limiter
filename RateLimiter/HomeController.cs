using System;
using System.Collections.Generic;
using RateLimiter.Constants;
using RateLimiter.Infrastructure;
using RateLimiter.Interfaces;
using RateLimiter.Models;

namespace RateLimiter
{
	public class HomeController
	{
		private readonly Dictionary<string, List<ILimiterRule>> _clientRules;
		private readonly RequestStorage _requestStorage = new ();
		private readonly IService _service;

		public HomeController(Dictionary<string, List<ILimiterRule>> clientRules, IService service)
		{
			_clientRules = clientRules;
			_service = service;
		}

		public ResponseModel Index(string key)
		{
			return ServiceWrapper(key, () => _service.GetServiceData());
		}

		private ResponseModel ServiceWrapper(string key, Func<string> func)
		{
			var validateResult = Validate(key);

			return validateResult.IsValid
				? new ResponseModel {IsSuccess = true, Data = func()}
				: new ResponseModel {IsSuccess = false, Error = validateResult.ErrorMessage};
		}

		private ValidateModel Validate(string key)
		{
			if (!_clientRules.ContainsKey(key))
			{
				return new ValidateModel
				{
					IsValid = false,
					ErrorMessage = string.Format(ErrorMessages.NoRulesError, key)
				};
			}

			var requests = _requestStorage.GetRequestByKey(key);
			return RequestValidator.ValidateRequest(requests, _clientRules[key]);
		}
	}
}