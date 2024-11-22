using WebApi.enums;

namespace WebApi.Utils
{
    public static class RoleMapper
    {
        public static UserRole ToEnum(int roleId) => roleId switch
        {
            1 => UserRole.Admin,
            2 => UserRole.Doctor,
            3 => UserRole.User,
            _ => throw new ArgumentException("Invalid role ID")
        };

        public static int ToRoleId(UserRole role) => role switch
        {
            UserRole.Admin => 1,
            UserRole.Doctor => 2,
            UserRole.User => 3,
            _ => throw new ArgumentException("Invalid role")
        };
    }
}
