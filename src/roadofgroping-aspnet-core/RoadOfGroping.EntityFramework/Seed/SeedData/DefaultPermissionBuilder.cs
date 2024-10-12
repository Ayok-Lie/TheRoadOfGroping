using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RoadOfGroping.Common.Consts;
using RoadOfGroping.Core.Permissions.Entity;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Autofac;

namespace RoadOfGroping.EntityFramework.Seed.SeedData
{
    public class DefaultPermissionBuilder
    {
        private readonly RoadOfGropingDbContext _context;

        public DefaultPermissionBuilder(RoadOfGropingDbContext dbContext)
        {
            _context = dbContext;
        }

        public void Create()
        {
            CreateDefaultPermission();
        }

        private void CreateDefaultPermission()
        {
            var dbList = _context.PermissionOriginal.IgnoreQueryFilters()
                .Where(r => r.IsSystem && !r.IsDeleted)
                .ToList();
            var seedDataList = GetDynamicPermissionList(dbList);
            // 种子数据为空 退出
            if (seedDataList.Count == 0)
            {
                return;
            }
            else
            {
                _context.PermissionOriginal.AddRange(seedDataList);
            }

            _context.SaveChanges();
        }

        private List<PermissionOriginal> GetDynamicPermissionList(List<PermissionOriginal> dbList)
        {
            List<PermissionOriginal> list = new();
            var appFolder = IOCManager.GetService<IWebHostEnvironment>();
            try
            {
                var path = Path.Join(appFolder.WebRootPath, "ConfigFiles", "DynamicPermissions", RoadOfGropingConst.DynamicPermisisonFileName);
                if (File.Exists(path))
                {
                    using (var streamReader = new StreamReader(path))
                    {
                        var jsonStr = streamReader.ReadToEnd();
                        var dataList = JsonConvert.DeserializeObject<List<SeedDataToJosnDto>>(jsonStr);

                        foreach (var item in dataList)
                        {
                            if (!dbList.Any(r => r.Code == item.Code))
                            {
                                PermissionOriginal entity = new()
                                {
                                    DisplayName = item.DisplayName,
                                    Code = item.Code,
                                    ParentCode = item.ParentCode,
                                    IsSystem = true,
                                    IsDeleted = false,
                                    CreationTime = DateTime.Now,
                                    CreatorId = null,
                                    Sort = item.Sort
                                };
                                list.Add(entity);

                                PermissionRoleRelation permissionRole = new()
                                {
                                    PermissionCode = item.Code,
                                    IsGranted = true,
                                    RoleId = RoadOfGropingConst.DefaultRoleId
                                };
                                _context.PermissionRoleRelation.Add(permissionRole);
                            }
                        }
                    }
                }
                return list;
            }
            catch (KeyNotFoundException e)
            {
                return null;
            }
        }

        public class SeedDataToJosnDto
        {
            //
            // 摘要:
            //     权限名称
            public string DisplayName { get; set; }

            //
            // 摘要:
            //     权限编码
            public string Code { get; set; }

            //
            // 摘要:
            //     父级权限编码
            public string? ParentCode { get; set; }

            //
            // 摘要:
            //     是否是系统
            public bool IsSystem { get; set; }

            //
            // 摘要:
            //     排序
            public int Sort { get; set; }
        }
    }
}