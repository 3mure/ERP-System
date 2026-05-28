using Catalog_Service.Entities;

namespace Catalog_Service.Features.BannersFeature.UpdateBanner;

public record UpdateBannerRequest(
    string Title,
    string? TitleAr,
    string? Subtitle,
    string? SubtitleAr,
    string DesktopImageUrl,
    string? MobileImageUrl,
    string? CtaText,
    string? CtaTextAr,
    string? CtaLink,
    BannerPosition Position,
    int SortOrder,
    DateTime ValidFrom,
    DateTime ValidUntil,
    bool IsActive
);
