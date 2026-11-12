# Weekly Employee Shift Scheduler

A scheduling application built with **C# and .NET Blazor**. Project made for managing weekly schedules at a small business.

## Features

- Add and remove employees.
- Assign a day, start time, and end time to each shift on a calendar.
- View shifts on an hourly timeline, so 10 AM and 11 AM appear at different vertical positions.
- Click an hour in the calendar to prefill the selected day and a one-hour shift; adjust the times in the form as needed.
- Prevent overlapping shifts for the same employee on the same day.
- View the current week and click between weeks.
- Set each day's required coverage hours (default: 10 AM–10 PM).
- Highlight uncovered time ranges in red, even when the day already has shifts.
- Display overlapping employees' shifts side by side.
- Calculate hours per employee, total assigned hours, and total uncovered hours.
- Automatically save changes to a local JSON file.

The app starts with four **fictional example employees** and four example shifts. You can remove them and enter your own data. The first run creates `schedule-data.json`; this file is excluded from Git.

## Run it

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).
2. Open a terminal in the `ShiftScheduler` folder.
3. Run:

   ```bash
   dotnet run
   ```

4. Open **http://localhost:5172** in browser (or use the local address printed in the terminal).

To stop it, press **Ctrl+C** in the terminal.

## How coverage gaps work

The application checks whether **at least one employee** is scheduled during each part of a day's required coverage window. It combines overlapping shifts before calculating gaps.

For example, with required coverage from 10:00 to 22:00:

- Alex works 10:00–16:00.
- Jordan works 15:00–19:00.
- Sam works 20:00–22:00.

The calendar highlights **19:00–20:00 in red**. Alex and Jordan's overlap doesn't count as an uncovered hour.

This version doesn't evaluate how many employees are needed simultaneously. It identifies periods with no employee scheduled at all.

## How conflict detection works

Before a new shift is saved, the application looks for existing shifts belonging to the **same employee on the same day**. Two shifts overlap when:

```csharp
newStart < existingEnd && newEnd > existingStart
```

For example, `10:00–16:00` conflicts with `14:00–18:00`, but not with `16:00–20:00`. The application rejects a conflicting shift and shows a warning.

## Project structure

| File | Purpose |
| --- | --- |
| `Models/ScheduleData.cs` | Defines employees, shifts, coverage windows, and the data file structure. |
| `Services/ScheduleStore.cs` | Detects scheduling conflicts and uncovered time ranges, and saves JSON. |
| `Components/Pages/Home.razor` | Implements the user interface and button/form actions. |
| `wwwroot/app.css` | Styles the interface and its mobile layout. |
| `Program.cs` | Starts the application and registers the schedule service. |

