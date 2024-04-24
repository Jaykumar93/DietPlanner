using Domain.Data;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserDetailRepository : IUserDetailRepository
    {
        private readonly Domain.Data.DietContext _context;

        public UserDetailRepository(Domain.Data.DietContext context)
        {
            _context = context;
        }

        

   

        public void AddUserDetail(TblUserDetail userDetail)
        {
            _context.TblUserDetails.Add(userDetail);
            _context.SaveChanges();
        }

        public TblUserDetail GetUserDetailByEmail(string email)
        {
            return _context.TblUserDetails.FirstOrDefault(TblUserDetail => TblUserDetail.Email == email);
        }
        public TblUserDetail GetUserDetailByUser(string Username)
        {
            return _context.TblUserDetails.FirstOrDefault(TblUserDetail => TblUserDetail.UserName == Username);
        }

        //string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

        public List<TblUserDetail> GetAllUserDetails()
        {
            List<TblUserDetail> userDetailList = new List<TblUserDetail>();

            string connectionString = "Server=5630-LAP-0793\\SQLEXPRESS01;Database=DietPlanningWebsite;Trusted_Connection=True;TrustServerCertificate=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "sp_GetAllUsers";
                SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                DataTable dtUsers = new DataTable();

                connection.Open();
                sqlDA.Fill(dtUsers);
                connection.Close();

                foreach (DataRow dr in dtUsers.Rows)
                {
                    userDetailList.Add(new TblUserDetail
                    {
                        UserName = Convert.ToString(dr["user_name"]),
                        FirstName = Convert.ToString(dr["first_name"]),
                        LastName = Convert.ToString(dr["last_name"]),
                        Email = Convert.ToString(dr["email"]),
                        ContactNumber = Convert.ToString(dr["contact_number"])
                        
                    });
                }
            }
            return userDetailList;
        }




    }
}
