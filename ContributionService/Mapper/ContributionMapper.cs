using ContributionService.Models;
using ContributionService.DTOs;

namespace ContributionService.Mapper
{
    public static class ContributionMapper
    {
        public static ContributionDto toDto(this Contribution contribution)
        {
            return new ContributionDto()
            {
                Id = contribution.Id,
                UserId = contribution.UserId,
                SignId = contribution.SignId > 0 ? (int?)contribution.SignId : null,
                Action = contribution.Action,
                Description = contribution.Description ?? string.Empty,
                ImageUrl = contribution.ImageUrl ?? string.Empty,
                Status = contribution.Status,
                CreatedAt = contribution.CreatedAt,
                Type = contribution.Type ?? string.Empty,
                Latitude = contribution.Latitude,
                Longitude = contribution.Longitude
            };
        }
    }
}

