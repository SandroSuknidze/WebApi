using Oracle.ManagedDataAccess.Client;
using System.Data;
using WebApi.Models;
using WebApi.Utils;

namespace WebApi.Packages
{
    public interface IPKG_CATEGORIES
    {
        List<Category> GetCategories();
    }

    public class PKG_CATEGORIES : PKG_BASE, IPKG_CATEGORIES
    {
        public List<Category> GetCategories()
        {
            OracleConnection conn = new()
            {
                ConnectionString = ConnStr
            };
            List<Category> categories = [];

            conn.Open();
            OracleCommand cmd = new()
            {
                Connection = conn,
                CommandText = "PKG_SANDRO_CATEGORIES.get_categories",
                CommandType = CommandType.StoredProcedure,
            };

            cmd.Parameters.Add("p_categories", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            try
            {
                using OracleDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Category category = new()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                    };
                    categories.Add(category);
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("An error occurred while getting the categories: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return categories;
        }


    }
}
