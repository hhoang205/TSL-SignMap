using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;

namespace WebAppTrafficSign.Mapper
{
    public static class TrafficSignMapper
    {
        public static TrafficSignDto toDto(this TrafficSign sign)
        {
            return new TrafficSignDto()
            {
                Id = sign.Id,
                Type = sign.Type,
                Latitude = sign.Location.Y,
                Longitude = sign.Location.X,
                Status = sign.Status,
                ImageUrl = sign.ImageUrl,
                ValidFrom = sign.ValidFrom,
                ValidTo = sign.ValidTo == DateTime.MaxValue ? null : sign.ValidTo
            };
        }
    }
}



