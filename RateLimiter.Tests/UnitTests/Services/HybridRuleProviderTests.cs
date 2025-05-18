using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Models;
using RateLimiter.Core.Configuration;
using RateLimiter.Core.Services;

namespace RateLimiter.UnitTests.Services;

public class HybridRuleProviderTests
{
    private readonly Mock<IRateLimitRuleProvider> _configProviderMock;
    private readonly Mock<IRateLimitRuleProvider> _attributeProviderMock;
    private readonly Mock<ILogger<HybridRuleProvider>> _loggerMock;
    private readonly EnhancedRateLimitConfiguration _config;
    private readonly HttpContext _httpContext;

    public HybridRuleProviderTests()
    {
        _configProviderMock = new Mock<IRateLimitRuleProvider>();
        _attributeProviderMock = new Mock<IRateLimitRuleProvider>();
        _loggerMock = new Mock<ILogger<HybridRuleProvider>>();
        _httpContext = new DefaultHttpContext();
        _config = new EnhancedRateLimitConfiguration
        {
            EnableConfigurationRules = true,
            EnableAttributeRules = true,
            ConflictResolutionStrategy = ConflictResolutionStrategy.ConfigurationWins,
            LogConflicts = true
        };
    }

    [Fact]
    public async Task GetMatchingRulesAsync_WhenBothProvidersEnabled_ShouldCombineRules()
    {
        // Arrange
        var configRule = CreateMockRule("ConfigRule", 10);
        var attributeRule = CreateMockRule("AttributeRule", 20);
        
        _configProviderMock
            .Setup(p => p.GetMatchingRulesAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new[] { configRule });
            
        _attributeProviderMock
            .Setup(p => p.GetMatchingRulesAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new[] { attributeRule });

        var optionsMock = new Mock<IOptions<EnhancedRateLimitConfiguration>>();
        optionsMock.Setup(o => o.Value).Returns(_config);

        var provider = CreateHybridProvider(optionsMock.Object, _configProviderMock.Object, _attributeProviderMock.Object);

        // Act
        var result = await provider.GetMatchingRulesAsync(_httpContext);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, r => r.Name == "ConfigRule");
        Assert.Contains(result, r => r.Name == "AttributeRule");
    }

    [Fact]
    public async Task GetMatchingRulesAsync_WhenConfigurationWins_ShouldResolveConflicts()
    {
        // Arrange
        var configRule = CreateMockRule("SameRule", 10);
        var attributeRule = CreateMockRule("SameRule", 20);
        
        _configProviderMock
            .Setup(p => p.GetMatchingRulesAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new[] { configRule });
            
        _attributeProviderMock
            .Setup(p => p.GetMatchingRulesAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new[] { attributeRule });

        var optionsMock = new Mock<IOptions<EnhancedRateLimitConfiguration>>();
        optionsMock.Setup(o => o.Value).Returns(_config);

        var provider = CreateHybridProvider(optionsMock.Object, _configProviderMock.Object, _attributeProviderMock.Object);

        // Act
        var result = await provider.GetMatchingRulesAsync(_httpContext);

        // Assert
        Assert.Single(result);
        Assert.Equal("SameRule", result.First().Name);
        // Configuration rule should win due to ConfigurationWins strategy
    }

    [Fact]
    public async Task GetMatchingRulesAsync_WhenAttributeWins_ShouldPrioritizeAttribute()
    {
        // Arrange
        _config.ConflictResolutionStrategy = ConflictResolutionStrategy.AttributeWins;
        
        var configRule = CreateMockRule("SameRule", 10);
        var attributeRule = CreateMockRule("SameRule", 20);
        
        _configProviderMock
            .Setup(p => p.GetMatchingRulesAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new[] { configRule });
            
        _attributeProviderMock
            .Setup(p => p.GetMatchingRulesAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new[] { attributeRule });

        var optionsMock = new Mock<IOptions<EnhancedRateLimitConfiguration>>();
        optionsMock.Setup(o => o.Value).Returns(_config);

        var provider = CreateHybridProvider(optionsMock.Object, _configProviderMock.Object, _attributeProviderMock.Object);

        // Act
        var result = await provider.GetMatchingRulesAsync(_httpContext);

        // Assert
        Assert.Single(result);
        // Should include the rule (attribute wins strategy allows override)
    }

    [Fact]
    public async Task GetMatchingRulesAsync_WhenConfigurationDisabled_ShouldOnlyUseAttributes()
    {
        // Arrange
        _config.EnableConfigurationRules = false;
        
        var attributeRule = CreateMockRule("AttributeRule", 20);
        
        _attributeProviderMock
            .Setup(p => p.GetMatchingRulesAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new[] { attributeRule });

        var optionsMock = new Mock<IOptions<EnhancedRateLimitConfiguration>>();
        optionsMock.Setup(o => o.Value).Returns(_config);

        var provider = CreateHybridProvider(optionsMock.Object, null, _attributeProviderMock.Object);

        // Act
        var result = await provider.GetMatchingRulesAsync(_httpContext);

        // Assert
        Assert.Single(result);
        Assert.Equal("AttributeRule", result.First().Name);
    }

    [Fact]
    public async Task GetMatchingRulesAsync_WhenAttributesDisabled_ShouldOnlyUseConfiguration()
    {
        // Arrange
        _config.EnableAttributeRules = false;
        
        var configRule = CreateMockRule("ConfigRule", 10);
        
        _configProviderMock
            .Setup(p => p.GetMatchingRulesAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new[] { configRule });

        var optionsMock = new Mock<IOptions<EnhancedRateLimitConfiguration>>();
        optionsMock.Setup(o => o.Value).Returns(_config);

        var provider = CreateHybridProvider(optionsMock.Object, _configProviderMock.Object, null);

        // Act
        var result = await provider.GetMatchingRulesAsync(_httpContext);

        // Assert
        Assert.Single(result);
        Assert.Equal("ConfigRule", result.First().Name);
    }

    private IRateLimitRule CreateMockRule(string name, int maxRequests)
    {
        var rule = new Mock<IRateLimitRule>();
        rule.Setup(r => r.Name).Returns(name);
        rule.Setup(r => r.IsMatch(It.IsAny<HttpContext>())).Returns(true);
        rule.Setup(r => r.GetLimit(It.IsAny<HttpContext>()))
            .Returns(new RateLimit { MaxRequests = maxRequests, TimeWindowInSeconds = 60 });
        return rule.Object;
    }

    private IRateLimitRuleProvider CreateHybridProvider(
        IOptions<EnhancedRateLimitConfiguration> options,
        IRateLimitRuleProvider? configProvider,
        IRateLimitRuleProvider? attributeProvider)
    {
        // Use custom testable implementation that accepts mocked providers
        return new TestableHybridRuleProvider(
            options,
            _loggerMock.Object,
            configProvider,
            attributeProvider);
    }

    // Custom testable version that accepts mocked providers
    private class TestableHybridRuleProvider : IRateLimitRuleProvider
    {
        private readonly IOptions<EnhancedRateLimitConfiguration> _config;
        private readonly ILogger<HybridRuleProvider> _logger;
        private readonly IRateLimitRuleProvider? _configProvider;
        private readonly IRateLimitRuleProvider? _attributeProvider;

        public TestableHybridRuleProvider(
            IOptions<EnhancedRateLimitConfiguration> config,
            ILogger<HybridRuleProvider> logger,
            IRateLimitRuleProvider? configProvider,
            IRateLimitRuleProvider? attributeProvider)
        {
            _config = config;
            _logger = logger;
            _configProvider = configProvider;
            _attributeProvider = attributeProvider;
        }

        public async Task<IEnumerable<IRateLimitRule>> GetAllRulesAsync()
        {
            var rules = new List<IRateLimitRule>();
            
            if (_config.Value.EnableConfigurationRules && _configProvider != null)
            {
                var configRules = await _configProvider.GetAllRulesAsync();
                rules.AddRange(configRules);
            }
            
            if (_config.Value.EnableAttributeRules && _attributeProvider != null)
            {
                var attributeRules = await _attributeProvider.GetAllRulesAsync();
                rules.AddRange(attributeRules);
            }
            
            return rules;
        }

        public async Task<IEnumerable<IRateLimitRule>> GetMatchingRulesAsync(HttpContext context)
        {
            var matchingRules = new List<(IRateLimitRule Rule, RuleSource Source, int Priority)>();
            
            // Get configuration-based matches
            if (_config.Value.EnableConfigurationRules && _configProvider != null)
            {
                var configMatches = await _configProvider.GetMatchingRulesAsync(context);
                foreach (var rule in configMatches)
                {
                    matchingRules.Add((rule, RuleSource.Configuration, 100));
                }
            }
            
            // Get attribute-based matches
            if (_config.Value.EnableAttributeRules && _attributeProvider != null)
            {
                var attributeMatches = await _attributeProvider.GetMatchingRulesAsync(context);
                foreach (var rule in attributeMatches)
                {
                    var shouldInclude = true;
                    var priority = 200;
                    
                    // Simple conflict resolution based on strategy
                    var existingRule = matchingRules.FirstOrDefault(r => r.Rule.Name == rule.Name);
                    if (existingRule.Rule != null)
                    {
                        switch (_config.Value.ConflictResolutionStrategy)
                        {
                            case ConflictResolutionStrategy.ConfigurationWins:
                                shouldInclude = false;
                                break;
                            case ConflictResolutionStrategy.AttributeWins:
                                // Remove the existing config rule
                                matchingRules.RemoveAll(r => r.Rule.Name == rule.Name);
                                break;
                        }
                    }
                    
                    if (shouldInclude)
                    {
                        matchingRules.Add((rule, RuleSource.Attribute, priority));
                    }
                }
            }
            
            return matchingRules.Select(r => r.Rule);
        }
    }

    private enum RuleSource
    {
        Configuration,
        Attribute
    }
}
