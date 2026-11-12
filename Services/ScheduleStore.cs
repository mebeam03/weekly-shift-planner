using System.Text.Json;
using ShiftScheduler.Models;

namespace ShiftScheduler.Services;

public sealed class ScheduleStore
{
    private readonly string _filePath;
    private readonly ScheduleData _data;
    private readonly object _lock = new();

    public ScheduleStore(string filePath)
    {
        _filePath = filePath;

        if (File.Exists(_filePath))
        {
            _data = JsonSerializer.Deserialize<ScheduleData>(
                File.ReadAllText(_filePath))
                ?? throw new InvalidDataException("Schedule file is empty.");
        }
        else
        {
            _data = CreateExample();
            Save();
        }
    }

    public List<Employee> GetEmployees()
    {
        lock (_lock)
            return _data.Employees.OrderBy(e => e.Name).ToList();
    }

    public List<ScheduledShift> GetWeek(DateOnly monday)
    {
        lock (_lock)
        {
            var names = _data.Employees.ToDictionary(e => e.Id, e => e.Name);
            return _data.Shifts
                .Where(s => s.Date >= monday && s.Date <= monday.AddDays(6))
                .Where(s => names.ContainsKey(s.EmployeeId))
                .OrderBy(s => s.Date).ThenBy(s => s.Start)
                .Select(s => new ScheduledShift(s.Id, s.EmployeeId,
                    names[s.EmployeeId], s.Date, s.Start, s.End))
                .ToList();
        }
    }

    // A coverage window is the period during which at least one employee
    // should be on duty. The default is 10 AM–10 PM, and it can be changed.
    public DailyCoverage GetCoverageWindow(DateOnly date)
    {
        lock (_lock)
        {
            var saved = _data.CoverageWindows.FirstOrDefault(d => d.Date == date);
            return saved is null
                ? new DailyCoverage { Date = date }
                : new DailyCoverage { Date = date, Start = saved.Start, End = saved.End };
        }
    }

    public string? SetCoverageWindow(DateOnly date, TimeOnly start, TimeOnly end)
    {
        if (end <= start)
            return "Coverage must end after it starts. Overnight hours are not supported.";

        lock (_lock)
        {
            var window = _data.CoverageWindows.FirstOrDefault(d => d.Date == date);
            if (window is null)
            {
                window = new DailyCoverage { Date = date };
                _data.CoverageWindows.Add(window);
            }

            window.Start = start;
            window.End = end;
            Save();
        }

        return null;
    }

    public List<CoverageGap> GetCoverageGaps(DateOnly date)
    {
        lock (_lock)
        {
            var window = _data.CoverageWindows.FirstOrDefault(w => w.Date == date)
                ?? new DailyCoverage { Date = date };

            var shifts = _data.Shifts
                .Where(s => s.Date == date && s.End > window.Start && s.Start < window.End)
                .OrderBy(s => s.Start).ToList();

            var gaps = new List<CoverageGap>();
            var coveredUntil = window.Start;

            foreach (var shift in shifts)
            {
                if (shift.Start > coveredUntil)
                    gaps.Add(new CoverageGap(coveredUntil, shift.Start));

                if (shift.End > coveredUntil)
                    coveredUntil = shift.End;

                if (coveredUntil >= window.End)
                    break;
            }

            if (coveredUntil < window.End)
                gaps.Add(new CoverageGap(coveredUntil, window.End));

            return gaps;
        }
    }

    public string? AddEmployee(string name)
    {
        name = name.Trim();
        if (name.Length is < 2 or > 50)
            return "Enter a name between 2 and 50 characters.";

        lock (_lock)
        {
            if (_data.Employees.Any(e => string.Equals(
                e.Name, name, StringComparison.OrdinalIgnoreCase)))
                return "That employee is already listed.";

            _data.Employees.Add(new Employee { Name = name });
            Save();
        }

        return null;
    }

    public string? AddShift(Guid employeeId, DateOnly date,
        TimeOnly start, TimeOnly end)
    {
        if (end <= start)
            return "End time must be later than start time.";

        lock (_lock)
        {
            if (!_data.Employees.Any(e => e.Id == employeeId))
                return "Select an employee first.";

            // Intervals overlap when each one starts before the other ends.
            bool overlaps = _data.Shifts.Any(s =>
                s.EmployeeId == employeeId && s.Date == date &&
                start < s.End && end > s.Start);

            if (overlaps)
                return "Scheduling conflict: this employee already works during that time.";

            _data.Shifts.Add(new WorkShift
            {
                EmployeeId = employeeId,
                Date = date,
                Start = start,
                End = end
            });
            Save();
        }

        return null;
    }

    public void RemoveShift(Guid id)
    {
        lock (_lock)
        {
            _data.Shifts.RemoveAll(s => s.Id == id);
            Save();
        }
    }

    public void RemoveEmployee(Guid id)
    {
        lock (_lock)
        {
            _data.Employees.RemoveAll(e => e.Id == id);
            _data.Shifts.RemoveAll(s => s.EmployeeId == id);
            Save();
        }
    }

    private void Save()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(_filePath, JsonSerializer.Serialize(_data, options));
    }

    private static ScheduleData CreateExample()
    {
        var monday = DateOnly.FromDateTime(DateTime.Today);
        int daysSinceMonday = ((int)monday.DayOfWeek + 6) % 7;
        monday = monday.AddDays(-daysSinceMonday);

        var people = new[] { "Alex Morgan", "Jordan Lee", "Taylor Smith", "Sam Rivera" }
            .Select(name => new Employee { Name = name }).ToList();

        return new ScheduleData
        {
            Employees = people,
            Shifts =
            [
                new() { EmployeeId = people[0].Id, Date = monday, Start = new(10, 0), End = new(16, 0) },
                new() { EmployeeId = people[1].Id, Date = monday, Start = new(16, 0), End = new(21, 0) },
                new() { EmployeeId = people[2].Id, Date = monday.AddDays(1), Start = new(11, 0), End = new(17, 0) },
                new() { EmployeeId = people[3].Id, Date = monday.AddDays(4), Start = new(16, 0), End = new(22, 0) },
            ]
        };
    }
}
