using Microsoft.AspNetCore.Mvc;
using TeamAssignmentApp.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

namespace TeamAssignmentApp.Controllers
{
    public class TeamAssignmentController : Controller
    {
        private static readonly string TeamNamesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "animals.txt");

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateTeams(TeamAssignment model)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(model.Names))
            {
                model.Teams = AssignTeams(model.Names.Split('\n').ToList(), model.TeamSize);
                return View("Teams", model);
            }
            return View("Index");
        }

        [HttpPost]
        public IActionResult AssignTeamNames(TeamAssignment model)
        {
            if (model.Teams != null && model.Teams.Any())
            {
                foreach (var team in model.Teams)
                {
                    if (team.Members == null)
                    {
                        team.Members = new List<string>();
                    }
                    if (string.IsNullOrEmpty(team.TeamName))
                    {
                        team.TeamName = string.Empty;
                    }
                }

                var teamNames = System.IO.File.ReadAllLines(TeamNamesFilePath).ToList();
                if (teamNames.Count < model.Teams.Count)
                {
                    ModelState.AddModelError("Teams", "Not enough unique team names available.");
                    return View("Teams", model);
                }

                model.Teams = AssignRandomTeamNames(model.Teams, teamNames);
                return View("Teams", model);
            }
            return View("Index");
        }

        private List<Team> AssignTeams(List<string> names, int teamSize)
        {
            var random = new Random();
            var shuffledNames = names.OrderBy(x => random.Next()).ToList();
            var teams = new List<Team>();

            for (int i = 0; i < shuffledNames.Count; i += teamSize)
            {
                var team = new Team
                {
                    Members = shuffledNames.Skip(i).Take(teamSize).ToList()
                };
                teams.Add(team);
            }

            return teams;
        }

        private List<Team> AssignRandomTeamNames(List<Team> teams, List<string> teamNames)
        {
            var random = new Random();
            var assignedNames = new HashSet<string>();

            foreach (var team in teams)
            {
                string teamName;
                do
                {
                    teamName = teamNames[random.Next(teamNames.Count)];
                } while (assignedNames.Contains(teamName));

                assignedNames.Add(teamName);
                team.TeamName = teamName;
            }

            return teams;
        }
    }
}
