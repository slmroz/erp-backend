using ERP.Model.Abstractions;

namespace ERP.Infrastructure.Time;
internal sealed class Clock : IClock
{
    public DateTime Current() => DateTime.UtcNow;
}