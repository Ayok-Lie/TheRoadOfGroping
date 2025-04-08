using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using RoadOfGroping.Common.DependencyInjection;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Permission.Authorizations
{
    public class AppAuthorizationPolicyProvider : IAuthorizationPolicyProvider, ITransientDependency
    {
        private static readonly AsyncLock _mutex = new();
        private readonly AuthorizationOptions _authorizationOptions;

        public AppAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _backupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
            _authorizationOptions = options.Value;
        }

        private DefaultAuthorizationPolicyProvider _backupPolicyProvider { get; }

        public async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName is null) throw new ArgumentNullException(nameof(policyName));

            using (await _mutex.LockAsync())
            {
                var policy = await _backupPolicyProvider.GetPolicyAsync(policyName);
                if (policy is not null)
                {
                    return policy;
                }

                if (PermissionAuthorizeAttribute.TryGetPermissions(policyName, out var permissions))
                {
                    var builder = new AuthorizationPolicyBuilder();
                    builder.RequirePermissions(permissions);
                    policy = builder.Build();
                    _authorizationOptions.AddPolicy(policyName, policy);

                    return policy;
                }
            }

            return null;
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return _backupPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return _backupPolicyProvider.GetFallbackPolicyAsync();
        }
    }
}