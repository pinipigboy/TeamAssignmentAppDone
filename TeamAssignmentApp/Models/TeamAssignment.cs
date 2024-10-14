using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamAssignmentApp.Models
{
    public class TeamAssignment
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z\s,.'-]+$", ErrorMessage = "Invalid characters in name list.")]
        public string? Names { get; set; }

        [Required]
        [Range(2, 10, ErrorMessage = "Team size must be between 2 and 10.")]
        public int TeamSize { get; set; }
        
        public List<Team>? Teams { get; set; } = new List<Team>();
    }

    public class Team
    {
        public string? TeamName { get; set; } = string.Empty;
        public List<string>? Members { get; set; } = new List<string>();
    }
}
