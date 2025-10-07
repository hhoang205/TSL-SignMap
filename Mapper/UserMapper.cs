using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;

namespace WebAppTrafficSign.Mapper
{
    public static class UserMapper
    {
        public static UserDto toDto(this User user)
        {
            return new UserDto()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = int.TryParse(user.PhoneNumber, out var phoneNumber) ? phoneNumber : 0,
                Reputation = user.Reputation,
                RoleId = user.RoleId,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}



