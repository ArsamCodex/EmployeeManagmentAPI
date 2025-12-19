namespace EmployeeManagmentAPI.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        // Primary key for the attendance record

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        // Links this attendance record to the user

        public DateTime Date { get; set; }
        // The specific date of this attendance record

        public DateTime CheckIn { get; set; }
        // The time the user checked in

        public DateTime? CheckOut { get; set; }
        // The time the user checked out; nullable in case they haven't checked out yet

        public string Status { get; set; }
        // Status for that day, e.g., "Present", "Absent", "Late"
    }


}


/*
 *The purpose of the Attendance class is to track and manage the daily attendance of employees in your application. It serves as a record for each user, showing when they checked in, checked out, and their attendance status for a particular day.

Key purposes in detail:

Record Check-In and Check-Out Times

Stores the exact time a user starts and ends their workday.

Helps calculate work hours or identify if a user is late or left early.

Track Daily Presence

The Status property (Present, Absent, Late) gives a quick overview of whether the employee was present and punctual.

Link Attendance to a User

UserId and User link the attendance record to a specific ApplicationUser, so you know which employee the record belongs to.

Support Payroll and HR Management

Attendance data can be used to determine pay, calculate overtime, or monitor employee performance.

Historical Reference

Maintains a history of employee attendance over time, which can be useful for reporting, audits, or

var attendance1 = new Attendance
{
    AttendanceId = 1,
    UserId = "user123",
    User = new ApplicationUser { UserName = "alice" },
    Date = new DateTime(2025, 12, 15),
    CheckIn = new DateTime(2025, 12, 15, 9, 5, 0), // 9:05 AM
    CheckOut = new DateTime(2025, 12, 15, 17, 0, 0), // 5:00 PM
    Status = "Present"
};



Attendance Record:
- User: alice
- Date: 15/12/2025
- Check In: 09:05
- Check Out: 17:00
- Status: Present
Example Use Case:

An employee checks in at 9:05 AM. The system creates an Attendance record with CheckIn = 9:05, Status = "Present".

If the employee forgets to check out, CheckOut remains null until the end of the day.

HR can later generate a report showing all late arrivals, absences, or total hours worked.
*/
