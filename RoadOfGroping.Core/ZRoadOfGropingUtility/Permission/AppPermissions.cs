namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Permission
{
    public static class AppPermissions
    {
        public const string GroupName = "App";

        public static class User
        {
            public const string Default = GroupName + ".User";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }
    }
}