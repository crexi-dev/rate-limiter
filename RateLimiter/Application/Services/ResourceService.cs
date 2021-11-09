using System;
using AutoMapper;
using RateLimiter.Application.AccessRestriction.Authorization;
using RateLimiter.Application.AccessRestriction.Rule;
using RateLimiter.Application.Exception;
using RateLimiter.Data.Repository;
using RateLimiter.Domain.Entities;
using RateLimiter.ViewModels;

namespace RateLimiter.Application.Services
{
    /*
        Note: Depending on the nature of resources, it might make sense to break this up into separate domains.
        For purposes of demo, resources A and B are both accessed through the same service, repo, etc.
    */

    /// <inheritdoc />
    public class ResourceService : IResourceService
    {
        private readonly IRateRuleEngineService _rateRuleEngineService;
        private readonly IResourceRepository _resourceRepository;
        private readonly IResourceAccessRepository _resourceAccessRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rateRuleEngineService"></param>
        /// <param name="resourceRepository"></param>
        /// <param name="resourceAccessRepository"></param>
        /// <param name="mapper"></param>
        public ResourceService(IRateRuleEngineService rateRuleEngineService, IResourceRepository resourceRepository, IResourceAccessRepository resourceAccessRepository, IMapper mapper)
        {
            _rateRuleEngineService = rateRuleEngineService;
            _resourceRepository = resourceRepository;
            _resourceAccessRepository = resourceAccessRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public TViewModel Get<TResource, TViewModel>(int id, IUserToken userToken) where TResource : IResource, new()
                                                                                                                      where TViewModel : IResourceReadViewModelBase
        {
            IRuleEngineExecutionResult accessResult = _rateRuleEngineService.Execute<TResource>(id, userToken);

            if (accessResult.IsSuccess)
            {
                _resourceAccessRepository.Add<TResource>(id, userToken.UserId);

                TResource? r = (TResource?)_resourceRepository.Get<TResource>(id);

                if (r != null)
                {
                    var viewModel = _mapper.Map<TViewModel>(r);
                    viewModel.Retrieved = DateTime.Now;

                    return viewModel;
                }

                throw new ApplicationException($"No resource found for id {id}");
            }

            throw new RateLimitException();
        }

    }
}
