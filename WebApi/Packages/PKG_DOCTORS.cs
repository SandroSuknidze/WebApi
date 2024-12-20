using Oracle.ManagedDataAccess.Client;
using System.Data;
using WebApi.DTO;
using WebApi.Models;
using WebApi.Utils;

namespace WebApi.Packages
{
    public interface IPKG_DOCTORS
    {
        void AddDoctor(Doctor doctor);
        List<DoctorCategoriesDTO> GetDoctors(string? name, string? category);
    }
    public class PKG_DOCTORS : PKG_BASE, IPKG_DOCTORS
    {
        public void AddDoctor(Doctor doctor)
        {

            OracleConnection conn = new()
            {
                ConnectionString = ConnStr
            };

            conn.Open();

            OracleCommand cmd = new()
            {
                Connection = conn,
                CommandText = "olerning.PKG_SANDRO_DOCTORS.create_doctor",
                CommandType = CommandType.StoredProcedure,
            };


            byte[]? avatarBytes = null;
            if (!string.IsNullOrEmpty(doctor.Avatar))
            {
                var base64Data = doctor.Avatar.Contains(',') // Check if it's base64 data with header
                    ? doctor.Avatar.Split(',')[1] // Get the base64 part
                    : doctor.Avatar;
                avatarBytes = Convert.FromBase64String(base64Data); // Convert to byte array
            }

            int roleId = RoleMapper.ToRoleId(doctor.Role);

            cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = doctor.Email;
            cmd.Parameters.Add("p_first_name", OracleDbType.Varchar2).Value = doctor.FirstName;
            cmd.Parameters.Add("p_last_name", OracleDbType.Varchar2).Value = doctor.LastName;
            cmd.Parameters.Add("p_personal_id", OracleDbType.Int64).Value = doctor.PersonalId;
            cmd.Parameters.Add("p_avatar", OracleDbType.Blob).Value = avatarBytes;
            cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = doctor.Password;
            cmd.Parameters.Add("p_role_id", OracleDbType.Int32).Value = roleId;

            cmd.Parameters.Add("p_bio", OracleDbType.Clob).Value = doctor.Bio;
            cmd.Parameters.Add("p_category_id", OracleDbType.Int64).Value = doctor.CategoryId;

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public List<DoctorCategoriesDTO> GetDoctors(string? name, string? category)
        {
            using OracleConnection conn = new()
            {
                ConnectionString = ConnStr
            };

            conn.Open();

            using OracleCommand cmd = new()
            {
                Connection = conn,
                CommandText = "olerning.PKG_SANDRO_DOCTORS.get_doctors",
                CommandType = CommandType.StoredProcedure,
            };

            cmd.Parameters.Add("p_doctors", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_name", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(name) ? DBNull.Value : (object)name;
            cmd.Parameters.Add("p_category_name", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(category) ? DBNull.Value : (object)category;

            List<DoctorCategoriesDTO> doctors = new();

            try
            {
                using OracleDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DoctorCategoriesDTO doctor = new()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                        LastName = reader.GetString(reader.GetOrdinal("last_name")),
                        Avatar = Convert.ToBase64String((byte[])reader["avatar"]),
                        CategoryId = reader.GetInt32(reader.GetOrdinal("category_id")),
                        CategoryName = reader.GetString(reader.GetOrdinal("category_name")),
                        Rating = reader.GetDecimal(reader.GetOrdinal("rating")),
                    };
                    doctors.Add(doctor);
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("An error occurred while getting the doctors: " + ex.Message);
            }

            return doctors;
        }

    }
}
