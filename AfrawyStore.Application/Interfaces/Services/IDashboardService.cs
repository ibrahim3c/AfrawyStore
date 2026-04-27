using AfrawyStore.Application.DTOs;

namespace AfrawyStore.Application.Interfaces.Services;

public interface IDashboardService
{
    /// <summary>
    /// Returns all data needed to render the dashboard:
    /// summary totals, 7-day chart data, latest sales, and low-stock alerts.
    /// </summary>
    Task<DashboardViewModel> GetDashboardDataAsync();
}
