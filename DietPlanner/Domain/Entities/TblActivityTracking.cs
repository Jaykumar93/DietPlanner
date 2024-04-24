using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TblActivityTracking
{
    public Guid ActivityId { get; set; }

    public Guid ProfileId { get; set; }

    public string? ActivityType { get; set; }

    public DateTime ActivityStartDatetime { get; set; }

    public DateTime ActivityEndDatetime { get; set; }

    public string ActivityIntensity { get; set; } = null!;

    public int CalorieBurned { get; set; }

    public virtual TblProfileDetail Profile { get; set; } = null!;
}
