namespace ShiftScheduler.Models;

public sealed class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
}

public sealed class WorkShift
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
}

public sealed class ScheduleData
{
    public List<Employee> Employees { get; set; } = [];
    public List<WorkShift> Shifts { get; set; } = [];
    public List<DailyCoverage> CoverageWindows { get; set; } = [];
}

public sealed class DailyCoverage
{
    public DateOnly Date { get; set; }
    public TimeOnly Start { get; set; } = new(10, 0);
    public TimeOnly End { get; set; } = new(22, 0);
}

public sealed record CoverageGap(TimeOnly Start, TimeOnly End)
{
    public double Hours => (End - Start).TotalHours;
}

public sealed record ScheduledShift(
    Guid Id, Guid EmployeeId, string EmployeeName,
    DateOnly Date, TimeOnly Start, TimeOnly End)
{
    public double Hours => (End - Start).TotalHours;
}
