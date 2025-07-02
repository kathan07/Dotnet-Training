using System;
using System.Collections.Generic;

namespace DBPractice2.Models;

public partial class Assessment
{
    public int AssessmentId { get; set; }

    public int? StudentId { get; set; }

    public int? CourseId { get; set; }

    public int? Score { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Student? Student { get; set; }
}
