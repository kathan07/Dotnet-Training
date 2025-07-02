using System;
using System.Collections.Generic;

namespace DBPractice2.Models;

public partial class Log
{
    public int LogId { get; set; }

    public string LogMessage { get; set; } = null!;

    public DateTime? LogDate { get; set; }
}
