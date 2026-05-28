namespace Catalog_Service.Features.RetailFeature.AlertThresholds;

public record UpsertAlertThresholdDto(string AlertType, decimal ThresholdValue, string? Description);
