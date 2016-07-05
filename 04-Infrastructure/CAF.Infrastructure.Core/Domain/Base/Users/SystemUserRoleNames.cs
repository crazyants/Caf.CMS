
namespace CAF.Infrastructure.Core.Domain.Users
{
    public static partial class SystemUserRoleNames
    {
		/// <remarks>codehint: sm-add</remarks>
        public static string SuperAdministrators { get { return "SuperAdmin"; } }

        public static string Administrators { get { return "Admin"; } }
        
        public static string ForumModerators { get { return "ForumModerators"; } }

        public static string Registered { get { return "Registered"; } }

        public static string Guests { get { return "Guests"; } }
    }
}