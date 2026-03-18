using Core.Application.Abstractions.Messaging;

namespace Building.Application.Dashboard;

public sealed record DashboardStatisticsQuery() : IQuery<DashboardStatisticsResponse>;
