using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TblConsultation
{
    public Guid ConsultationId { get; set; }

    public Guid UserProfileId { get; set; }

    public Guid ExpertProfileId { get; set; }

    public string? ConsultantAvailabilty { get; set; }

    public DateTime AppointmentDateTime { get; set; }

    public TimeOnly AppointmentDuration { get; set; }

    public virtual TblProfileDetail ExpertProfile { get; set; } = null!;

    public virtual TblProfileDetail UserProfile { get; set; } = null!;
}
