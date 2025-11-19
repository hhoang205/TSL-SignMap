using UserService.Models;
using UserService.DTOs;

namespace UserService.Mapper
{
    public static class UserMapper
    {
        public static UserDto toDto(this User user)
        {
            // Map RoleId to Role string: 0 = User, 1 = Staff, 2 = Admin
            string role = user.RoleId switch
            {
                1 => "Staff",
                2 => "Admin",
                _ => "User"
            };

            return new UserDto()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Reputation = user.Reputation,
                RoleId = user.RoleId,
                Role = role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Password = user.Password,
                Firstname = user.Firstname,
                Lastname = user.Lastname
            };
        }
    }
}

