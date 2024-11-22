using Oracle.ManagedDataAccess.Client;
using System.Data;
using WebApi.enums;
using WebApi.Models;
using WebApi.Utils;

namespace WebApi.Packages
{
    public interface IPKG_USERS
    {
        void AddUser(User user);
        bool GetUserByEmail(string email);
        bool GetUserByPersonalId(int personalId);
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


            byte[] avatarBytes = null;
            if (!string.IsNullOrEmpty(user.Avatar))
            {
                var base64Data = user.Avatar.Contains(",") // Get the Base64 part
                    ? user.Avatar.Split(',')[1]
                    : "";
                avatarBytes = Convert.FromBase64String(base64Data); // Convert to byte array
            }

            int roleId = RoleMapper.ToRoleId(user.Role);

            cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = user.Email;
            cmd.Parameters.Add("p_first_name", OracleDbType.Varchar2).Value = user.FirstName;
            cmd.Parameters.Add("p_last_name", OracleDbType.Varchar2).Value = user.LastName;
            cmd.Parameters.Add("p_personal_id", OracleDbType.Int32).Value = user.PersonalId;
            cmd.Parameters.Add("p_avatar", OracleDbType.Blob).Value = avatarBytes;
            cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = user.Password;
            cmd.Parameters.Add("p_role_id", OracleDbType.Int32).Value = roleId;

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public bool GetUserByEmail(string email)
        {
            OracleConnection conn = new()
            {
                ConnectionString = ConnStr
            };

            conn.Open();
            OracleCommand cmd = new()
            {
                Connection = conn,
                CommandText = "olerning.PKG_SANDRO_USERS.get_user_by_email",
                CommandType = CommandType.StoredProcedure,
            };

            cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = email;
            cmd.Parameters.Add("p_user", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            try
            {
                using OracleDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("An error occurred while getting the user: " + ex.Message);
            }

        }

        public bool GetUserByPersonalId(int personalId)
        {
            OracleConnection conn = new()
            {
                ConnectionString = ConnStr
            };

            conn.Open();
            OracleCommand cmd = new()
            {
                Connection = conn,
                CommandText = "olerning.PKG_SANDRO_USERS.get_user_by_personal_id",
                CommandType = CommandType.StoredProcedure,
            };

            cmd.Parameters.Add("p_personal_id", OracleDbType.Int32).Value = personalId;
            cmd.Parameters.Add("p_user", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            try
            {
                using OracleDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("An error occurred while getting the user: " + ex.Message);
            }

        }


    }
}
