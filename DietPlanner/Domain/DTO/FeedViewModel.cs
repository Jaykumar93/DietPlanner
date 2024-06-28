using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class FeedViewModel
    {
        public string UserName { get; set; }

        public string PostContent { get; set; } = null!;

        public Guid? MealPlanId { get; set; }

        public DateTime CreatedDateTime { get; set; }

       
    }
}
