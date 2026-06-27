using MediatR;

namespace Catalog_Service.Features.SalesAnalyticsFeature.GetRushHourAnalysis
{
    public record GetRushHourAnalysisQuery() : IRequest<List<RushHourPointDto>>;

}
