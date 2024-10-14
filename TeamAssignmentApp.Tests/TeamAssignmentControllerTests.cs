using NUnit.Framework;
using TeamAssignmentApp.Controllers;
using TeamAssignmentApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace TeamAssignmentApp.Tests
{
    [TestFixture]
    public class TeamAssignmentControllerTests
    {
        private TeamAssignmentController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new TeamAssignmentController();

            // Copy the animals.txt file to the test output directory
            var sourceFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../../TeamAssignmentApp/wwwroot/animals.txt");
            var targetFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "wwwroot/animals.txt");

            var targetDirectory = Path.GetDirectoryName(targetFilePath);
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            if (File.Exists(sourceFilePath) && !File.Exists(targetFilePath))
            {
                File.Copy(sourceFilePath, targetFilePath);
            }
        }

        [Test]
        public void CreateTeams_ValidInput_ShouldAssignTeams()
        {
            var model = new TeamAssignment
            {
                Names = "Alice\nBob\nCharlie\nDavid",
                TeamSize = 2
            };

            var result = _controller.CreateTeams(model) as ViewResult;
            var assignedModel = result.Model as TeamAssignment;

            Assert.NotNull(assignedModel.Teams);
            Assert.AreEqual(2, assignedModel.Teams.Count);
            Assert.AreEqual(2, assignedModel.Teams[0].Members.Count);
            Assert.AreEqual(2, assignedModel.Teams[1].Members.Count);
        }

        [Test]
        public void AssignTeamNames_ValidTeams_ShouldAssignUniqueNames()
        {
            var model = new TeamAssignment
            {
                Teams = new List<Team>
                {
                    new Team { Members = new List<string> { "Alice", "Bob" } },
                    new Team { Members = new List<string> { "Charlie", "David" } }
                }
            };

            var result = _controller.AssignTeamNames(model) as ViewResult;
            var assignedModel = result.Model as TeamAssignment;

            Assert.NotNull(assignedModel.Teams);
            Assert.IsNotEmpty(assignedModel.Teams[0].TeamName);
            Assert.IsNotEmpty(assignedModel.Teams[1].TeamName);
            Assert.AreNotEqual(assignedModel.Teams[0].TeamName, assignedModel.Teams[1].TeamName);
        }

        [Test]
        public void CreateTeams_InvalidName_ShouldReturnIndexView()
        {
            var model = new TeamAssignment
            {
                Names = "Alice!@#\nBob",
                TeamSize = 2
            };

            _controller.ModelState.AddModelError("Names", "Invalid characters in name list.");

            var result = _controller.CreateTeams(model) as ViewResult;

            Assert.AreEqual("Index", result.ViewName);
        }

        [Test]
        public void CreateTeams_InvalidTeamSize_ShouldReturnIndexView()
        {
            var model = new TeamAssignment
            {
                Names = "Alice\nBob",
                TeamSize = 11
            };

            _controller.ModelState.AddModelError("TeamSize", "Team size must be between 2 and 10.");

            var result = _controller.CreateTeams(model) as ViewResult;

            Assert.AreEqual("Index", result.ViewName);
        }
    }
}
