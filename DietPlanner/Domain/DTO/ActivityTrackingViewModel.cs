using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class ActivityTrackingViewModel
    {
        public Guid ActivityId { get; set; }

        public Guid ProfileId { get; set; }

        public string Email { get; set; }
        public string? ActivityType { get; set; }

        public DateTime ActivityStartDatetime { get; set; }

        public DateTime ActivityEndDatetime { get; set; }

        public ActivityIntensityType ActivityIntensity { get; set; }

        public int CalorieBurned { get; set; }

        public enum ActivityIntensityType
        {
            None,
            Beginner,
            Intermediate,
            Advance,
            Pro
        }
    }
    
}
