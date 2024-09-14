using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Permission
{
    public class DataSeeds
    {
        private static readonly Dictionary<string, string[]> _seeds = new()
        {
            ["NonPermission"] = Array.Empty<string>(),
            ["DefaultPermission"] = new[]
            {
                AppPermissions.User.Default
            },
            ["CreatePermission"] = new[]
            {
                AppPermissions.User.Default,
                AppPermissions.User.Create
            },
            ["UpdatePermission"] = new[]
            {
                AppPermissions.User.Default,
                AppPermissions.User.Update
            },
            ["DeletePermission"] = new[]
            {
                AppPermissions.User.Default,
                AppPermissions.User.Delete
            },
            ["CreateAndUpdatePermission"] = new[]
            {
                AppPermissions.User.Default,
                AppPermissions.User.Create,
                AppPermissions.User.Update,
            },
            ["CreateAndDeletePermission"] = new[]
            {
                AppPermissions.User.Default,
                AppPermissions.User.Create,
                AppPermissions.User.Delete,
            },
            ["Admin"] = new[]
            {
                AppPermissions.User.Default,
                AppPermissions.User.Create,
                AppPermissions.User.Update,
                AppPermissions.User.Delete
            }
        };

        public string[] GetUserPermissions(string userId)
        {
            if (userId is not null && _seeds.TryGetValue(userId, out var permissions))
            {
                return permissions;
            }

            return Array.Empty<string>();
        }
    }
}