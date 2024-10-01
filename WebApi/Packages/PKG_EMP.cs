using System.Data;
using WebApi.Models;
using Oracle.ManagedDataAccess.Client;

namespace WebApi.Packages
{
    public class PKG_EMP : PKG_BASE
    {
        public void AddEmployee(Employee employee)
        {

            OracleConnection conn = new()
            {
                ConnectionString = ConnStr
            };

            conn.Open();

            OracleCommand cmd = new()
            {
                Connection = conn,
                CommandText = "olerning.PKG_SANDRO_EMP.save_employee",
                CommandType = CommandType.StoredProcedure,
            };

            cmd.Parameters.Add("p_first_name", OracleDbType.Varchar2).Value = employee.FirstName;
            cmd.Parameters.Add("p_last_name", OracleDbType.Varchar2).Value = employee.LastName;
            cmd.Parameters.Add("p_position", OracleDbType.Varchar2).Value = employee.Positon;

            cmd.ExecuteNonQuery();


            conn.Close();
        }
    }
}
