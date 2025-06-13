namespace WorkshopManager.DTOs;

public interface IReportService
{
    Task<IEnumerable<ReportRowDto>> GetAsync(int? customerId,
        int? vehicleId,
        int? year,
        int? month);

    Task<byte[]> ExportPdfAsync(IEnumerable<ReportRowDto> rows,
        string  title);
}