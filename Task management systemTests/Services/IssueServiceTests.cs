using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;
using Task_management_system.Models;


namespace Task_management_system.Services.Tests
{
    [TestClass]
    public class IssueServiceTests
    {
        private Mock<Context> _contextMock;
        private IssueService _issueService;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _contextMock = new Mock<Context>(options);
            _issueService = new IssueService(_contextMock.Object);
        }

        [TestMethod]
        public void CreateIssue_WithValidData_ReturnsSuccessMessage()
        {
            // Arrange
            var issue = new Issue
            {
                AssigneeId = "1",
                AssignedТoId = "2",
                Subject = "Test Issue",
                ProjectId = 1,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };

            // Create mock objects for the users and project
            var user1 = new ApplicationUser { Id = "1" };
            var user2 = new ApplicationUser { Id = "2" };
            var project = new Project
            {
                ProjectId = 1,
                ProjectName = "Test Project",
                ProjectDescription = "This is a test project",
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2022, 2, 1),
                Issues = new List<Issue>(),
                ProjectOwner = new ApplicationUser(),
                ProjectParticipants = new List<ApplicationUserProject>(),
                ProjectType = ""
            };

            // Set up the mock context with the mock objects and options for in-memory database
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _contextMock = new Mock<Context>(options);
            _contextMock.Setup(x => x.Users.Find("1")).Returns(user1);
            _contextMock.Setup(x => x.Users.Find("2")).Returns(user2);
            _contextMock.Setup(x => x.Projects.Find(1)).Returns(project);

            // Set up the Issues DbSet of the mock context with an empty list of issues
            _contextMock.Setup(x => x.Issues).Returns(new Mock<DbSet<Issue>>().Object);

            // Create an instance of the IssueService with the mock context
            _issueService = new IssueService(_contextMock.Object);

            // Act
            var result = _issueService.CreateIssue(issue);

            // Assert
            Assert.AreEqual("Успешно създадена задача!", result);
            _contextMock.Verify(x => x.SaveChanges(), Times.Once);
        }


        [TestMethod]
        public void CreateIssue_WithInvalidData_ReturnsErrorMessage()
        {
            // Arrange
            var issue = new Issue
            {
                AssigneeId = "1",
                AssignedТoId = "2",
                Subject = "",
                ProjectId = 1
            };
            _contextMock.Setup(x => x.Issues).Returns(new Mock<DbSet<Issue>>().Object);

            // Act
            var result = _issueService.CreateIssue(issue);

            // Assert
            Assert.AreEqual("Неуспешно създаване на задача. Моля, опитайте отново.", result);
            _contextMock.Verify(x => x.SaveChanges(), Times.Never);


        }

        // Add more test cases for different scenarios as needed
    }
}