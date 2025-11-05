using UserService.Models;
using UserService.DTOs;

namespace UserService.Mapper
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
                PhoneNumber = user.PhoneNumber,
                Reputation = user.Reputation,
                RoleId = user.RoleId,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Password = user.Password,
                Firstname = user.Firstname,
                Lastname = user.Lastname
            };
        }
    }
}

