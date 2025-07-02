using System;
using System.Collections.Generic;

namespace DBPractice2.Models;

public partial class HighScorer
{
    public int StudentId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int? CourseId { get; set; }

    public int? Score { get; set; }
}
