# Weekly Planner

A C# scheduling application designed to help small businesses manage employee shifts and identify gaps in scheduled coverage.

The project was developed using .NET and Blazor, with a focus on applying object-oriented programming and scheduling logic to a practical business problem.

## Features

- **Weekly Calendar:** View employee shifts on an interactive, time-based calendar.
- **Shift Management:** Add and remove employees and assign shifts.
- **Conflict Detection:** Prevent overlapping shifts for the same employee.
- **Coverage Tracking:** Highlight hours without scheduled coverage in red.
- **Hours Calculation:** Automatically calculate scheduled hours for each employee.
- **Data Storage:** Save employee and scheduling information locally using JSON.

## Technologies Used

- C#
- .NET 10
- Blazor
- HTML/CSS
- JSON

## How It Works

The application allows users to define required coverage hours for each day and assign employees to shifts.

Scheduled shifts appear on the weekly calendar, while uncovered hours are highlighted in red. Users can select a time slot directly from the calendar to begin assigning a shift.

The application also checks for overlapping assignments and calculates total scheduled hours.

Coverage tracking currently identifies periods when no employees are scheduled, rather than evaluating minimum staffing requirements.

## Running the Application

1. Install the .NET 10 SDK.
2. Download or clone this repository.
3. Open the project folder in a terminal.
4. Run `dotnet run`.
5. Open `http://localhost:5172` in your browser.
