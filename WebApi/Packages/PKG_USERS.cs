using Mailjet.Client.Resources;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Transactions;
using WebApi.enums;
using WebApi.Models;
using WebApi.Services;
using WebApi.Utils;
using User = WebApi.Models.User;

namespace WebApi.Packages
{
    public interface IPKG_USERS
    {
        void AddUser(User user);
        bool CheckUserByEmail(string email);
        User? GetUserByEmail(string email);
        bool GetUserByPersonalId(long personalId);
        Task<bool> SendVerificationCode(string email);
        bool IsVerificationCodeValid(string email, string verificationCode);
    }
    public class PKG_USERS(
        IEmailService emailService
       ) : PKG_BASE, IPKG_USERS
    {
        private readonly IEmailService _emailService = emailService;

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


            byte[]? avatarBytes = null;
            if (!string.IsNullOrEmpty(user.Avatar))
            {
                var base64Data = user.Avatar.Contains(',') // Get the Base64 part
                    ? user.Avatar.Split(',')[1]
                    : "";
                avatarBytes = Convert.FromBase64String(base64Data); // Convert to byte array
            }

            int roleId = RoleMapper.ToRoleId(user.Role);

            cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = user.Email;
            cmd.Parameters.Add("p_first_name", OracleDbType.Varchar2).Value = user.FirstName;
            cmd.Parameters.Add("p_last_name", OracleDbType.Varchar2).Value = user.LastName;
            cmd.Parameters.Add("p_personal_id", OracleDbType.Int64).Value = user.PersonalId;
            cmd.Parameters.Add("p_avatar", OracleDbType.Blob).Value = avatarBytes;
            cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = user.Password;
            cmd.Parameters.Add("p_role_id", OracleDbType.Int32).Value = roleId;

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public bool CheckUserByEmail(string email)
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

        public User? GetUserByEmail(string Email)
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
                        FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                        LastName = reader.GetString(reader.GetOrdinal("last_name")),
                        PersonalId = reader.GetInt64(reader.GetOrdinal("personal_id")),
                        Avatar = reader.IsDBNull(reader.GetOrdinal("avatar")) ? null : Convert.ToBase64String((byte[])reader["avatar"]),
                        Role = RoleMapper.ToEnum(reader.GetInt32(reader.GetOrdinal("role_id"))),
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

        public bool GetUserByPersonalId(long personalId)
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

            cmd.Parameters.Add("p_personal_id", OracleDbType.Int64).Value = personalId;
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

        public async Task<bool> SendVerificationCode(string email)
        {
            int verificationCode = new Random().Next(100000, 999999);

            OracleConnection conn = new()
            {
                ConnectionString = ConnStr
            };

            try
            {
                await conn.OpenAsync();

                using var transaction = conn.BeginTransaction();

                try
                {
                    OracleCommand cmd = new()
                    {
                        Connection = conn,
                        CommandText = "olerning.PKG_SANDRO_USERS.create_email_verification",
                        CommandType = CommandType.StoredProcedure,
                        Transaction = transaction
                    };

                    cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = email;
                    cmd.Parameters.Add("p_verification_code", OracleDbType.Int32).Value = verificationCode;
                    cmd.Parameters.Add("p_code_sent_at", OracleDbType.Date).Value = DateTime.UtcNow;
                    cmd.Parameters.Add("p_expires_at", OracleDbType.Date).Value = DateTime.UtcNow.AddMinutes(2);

                    await cmd.ExecuteNonQueryAsync();

                    await _emailService.SendVerificationEmailAsync(email, verificationCode);

                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Database error: " + ex.Message, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception("Invalid operation: " + ex.Message, ex);
            }
            catch (TimeoutException ex)
            {
                throw new Exception("Operation timed out: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred: " + ex.Message, ex);
            }
            finally
            {
                await conn.CloseAsync();
            }

        }

        public bool IsVerificationCodeValid(string email, string verificationCode)
        {
            OracleConnection conn = new()
            {
                ConnectionString = ConnStr
            };

            conn.Open();

            OracleCommand cmd = new()
            {
                Connection = conn,
                CommandText = "olerning.PKG_SANDRO_USERS.is_verification_code_valid",
                CommandType = CommandType.StoredProcedure,
            };

            cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = email;
            cmd.Parameters.Add("p_verification_code", OracleDbType.Varchar2).Value = verificationCode;

            OracleParameter refCursorParam = cmd.Parameters.Add("p_is_valid", OracleDbType.RefCursor);
            refCursorParam.Direction = ParameterDirection.Output;

            try
            {
                using OracleDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            }
            catch (OracleException ex)
            {
                throw new Exception("Error while validating the verification code: " + ex.Message, ex);
            }
        }
    }
}
