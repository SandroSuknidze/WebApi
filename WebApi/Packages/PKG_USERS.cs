using Oracle.ManagedDataAccess.Client;
using System.Data;
using WebApi.Models;

namespace WebApi.Packages
{
    public interface IPKG_USERS
    {
        void AddUser(User user);
        User GetUserByEmail(string Email);
    }
    public class PKG_USERS : PKG_BASE, IPKG_USERS
    {
        public void AddUser(User user)
        {

            OracleConnection conn = new()
            {
                ConnectionString = ConnStr
            };

            conn.Open();

            OracleCommand cmd = new()
            {
                Connection = conn,
                CommandText = "olerning.PKG_SANDRO_USERS.create_user",
                CommandType = CommandType.StoredProcedure,
            };

            cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = user.Email;
            cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = user.Password;

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public User GetUserByEmail(string Email)
        {
            OracleConnection conn = new()
            {
                ConnectionString = ConnStr
            };
            User user;

            conn.Open();
            OracleCommand cmd = new()
            {
                Connection = conn,
                CommandText = "olerning.PKG_SANDRO_USERS.get_user_by_email",
                CommandType = CommandType.StoredProcedure,
            };

            cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = Email;
            cmd.Parameters.Add("p_user", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            
            try
            {
                using OracleDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    user = new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Email = reader.GetString(reader.GetOrdinal("email")),
                        Password = reader.GetString(reader.GetOrdinal("password")),
                        Role = reader.GetString(reader.GetOrdinal("role"))
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("An error occurred while getting the user: " + ex.Message);
            }

            return user;

        }
    }
}
