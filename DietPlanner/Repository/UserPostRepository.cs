using Domain.Data;
using Domain.DTO;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserPostRepository : IUserPostRepository
    {
        private readonly DietContext _context;

        public UserPostRepository(DietContext context)
        {
            _context = context;
        }

        public void CreateNewMessage(string userName, string userMessage)
        {
            var userid = _context.TblUserDetails.Where(user => user.UserName == userName).Select(user => user.UserId).FirstOrDefault();
            var profileid = _context.TblProfileDetails.Where(profile => profile.UserId == userid).Select(profile => profile.ProfileId).FirstOrDefault();

             _context.TblUserPosts.AddAsync(new TblUserPost
            {
                ProfileId = profileid,
                PostContent = userMessage,
                CreatedDateTime = DateTime.Now,
/*                MealPlanId = MealPlanId ?? null
*/            });

             _context.SaveChangesAsync();
        }
    }
}
