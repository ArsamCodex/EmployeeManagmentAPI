namespace EmployeeManagmentAPI.Models
{
    public class PerformanceReview
    {
        public int PerformanceReviewId { get; set; }
        // Primary key, uniquely identifies each performance review.

        public string UserId { get; set; }
        // Foreign key: the employee who is being reviewed.

        public ApplicationUser User { get; set; }
        // Navigation property: gives access to the employee’s full data.

        public DateTime ReviewDate { get; set; }
        // The date when the review was done.

        public string ReviewerId { get; set; }
        // The Id of the manager who performed the review.

        public string Comments { get; set; }
        // Any feedback or remarks about the employee's performance.

        public int Score { get; set; }
        // Numeric performance score, e.g., 1-10.
    }


}
/*
 * // Assuming we have an employee and a manager
var employee = new ApplicationUser { Id = "emp123", FirstName = "Alice", LastName = "Smith" };
var manager = new ApplicationUser { Id = "mgr456", FirstName = "Bob", LastName = "Johnson" };

// Create a performance review
var review = new PerformanceReview
{
    UserId = employee.Id,      // Employee being reviewed
    User = employee,
    ReviewDate = DateTime.Now,
    ReviewerId = manager.Id,   // Manager who reviewed
    Comments = "Excellent teamwork and punctuality",
    Score = 9
};

// Access data
Console.WriteLine($"{review.User.FirstName} was reviewed by manager ID {review.ReviewerId}");
Console.WriteLine($"Score: {review.Score}/10, Comments: {review.Comments}");
*/