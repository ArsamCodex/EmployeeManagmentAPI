namespace EmployeeManagmentAPI.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        // Primary key for the project.

        public string Name { get; set; }
        // Name of the project.

        public string Description { get; set; }
        // Detailed description of the project.

        public DateTime StartDate { get; set; }
        // When the project starts.

        public DateTime? EndDate { get; set; }
        // Optional end date of the project (nullable, because the project might still be ongoing).

        public ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();
        // Navigation property: all users assigned to this project with their roles.
    }


}
/*
 * // Users
var alice = new ApplicationUser { Id = "emp1", FirstName = "Alice" };
var bob = new ApplicationUser { Id = "emp2", FirstName = "Bob" };

// Create a project
var project = new Project
{
    Name = "Website Redesign",
    Description = "Complete overhaul of the company website",
    StartDate = DateTime.Now
};

// Assign users to the project
project.ProjectAssignments.Add(new ProjectAssignment
{
    Project = project,
    ProjectId = project.ProjectId,
    User = alice,
    UserId = alice.Id,
    RoleInProject = "Frontend Developer",
    AssignedDate = DateTime.Now
});

project.ProjectAssignments.Add(new ProjectAssignment
{
    Project = project,
    ProjectId = project.ProjectId,
    User = bob,
    UserId = bob.Id,
    RoleInProject = "Backend Developer",
    AssignedDate = DateTime.Now
});

// Accessing project data
Console.WriteLine($"Project: {project.Name}");
foreach (var assignment in project.ProjectAssignments)
{
    Console.WriteLine($"{assignment.User.FirstName} is assigned as {assignment.RoleInProject}");
}
Project: Website Redesign
Alice is assigned as Frontend Developer
Bob is assigned as Backend Developer

*/