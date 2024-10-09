using Oracle.ManagedDataAccess.Client;
using System.Data;
using WebApi.Models;

namespace WebApi.Packages
{

    public interface IPKG_CARDS
    {
        public List<Card> GetCards();
        public void DeleteCard(int id);
        public Card GetCardById(int id);
        public void CreateCard(Card card);
        public void UpdateCard(Card card);
    }
    public class PKG_CARDS : PKG_BASE, IPKG_CARDS
    {
        public List<Card> GetCards()
        {
            List<Card> cards = [];    
            OracleConnection conn = new()
            {
                ConnectionString = ConnStr
            };

            conn.Open();

            OracleCommand cmd = new()
            {
                Connection = conn,
                CommandText = "olerning.PKG_SANDRO_CARDS.get_cards",
                CommandType = CommandType.StoredProcedure,
            };

            cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Card card = new()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    City = reader.GetString(reader.GetOrdinal("city")),
                    State = reader.GetString(reader.GetOrdinal("state")),
                    Photo = Convert.ToBase64String((byte[])reader["photo"]),
                    AvailableUnits = reader.GetInt32(reader.GetOrdinal("available_units")),
                    Wifi = reader.GetBoolean(reader.GetOrdinal("wifi")),
                    Laundry = reader.GetBoolean(reader.GetOrdinal("laundry")),
                    Pdf = reader["pdf"] == DBNull.Value ? null : Convert.ToBase64String((byte[])reader["pdf"]),
                };

                cards.Add(card);

            }

            reader.Close();
            cmd.Dispose();
            conn.Close();

            return cards;
        }

        public void DeleteCard(int id)
        {
            using OracleConnection conn = new(ConnStr);
            conn.Open();

            using OracleCommand cmd = new();
            cmd.Connection = conn;
            cmd.CommandText = "olerning.PKG_SANDRO_CARDS.delete_card";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Int32)).Value = id;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (OracleException ex)
            {
                if (ex.Number == 20001)
                {
                    throw new KeyNotFoundException($"Card with ID {id} not found.");
                }
                else
                {
                    throw new Exception("An error occurred while deleting the card: " + ex.Message);
                }
            }
        }

        public Card GetCardById(int id)
        {
            Card card;

            using OracleConnection con = new(ConnStr);
            con.Open();

            using OracleCommand cmd = new();
            cmd.Connection = con;
            cmd.CommandText = "olerning.PKG_SANDRO_CARDS.get_card";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Int32)).Value = id;
            cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            try
            {
                using OracleDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    card = new Card
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        City = reader.GetString(reader.GetOrdinal("city")),
                        State = reader.GetString(reader.GetOrdinal("state")),
                        Photo = Convert.ToBase64String((byte[])reader["photo"]),
                        AvailableUnits = reader.GetInt32(reader.GetOrdinal("available_units")),
                        Wifi = reader.GetBoolean(reader.GetOrdinal("wifi")),
                        Laundry = reader.GetBoolean(reader.GetOrdinal("laundry")),
                        Pdf = reader["pdf"] == DBNull.Value ? null :Convert.ToBase64String((byte[])reader["pdf"]),
                    };
                }
                else
                {
                    throw new KeyNotFoundException($"Card with ID {id} not found.");
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("An error occurred while getting the card: " + ex.Message);
            }

            return card;

        }

        public void CreateCard(Card card)
        {
            using OracleConnection con = new(ConnStr);
            con.Open();

            using OracleCommand cmd = new();
            cmd.Connection = con;
            cmd.CommandText = "olerning.PKG_SANDRO_CARDS.create_card";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new OracleParameter("p_name", OracleDbType.Varchar2)).Value = card.Name;
            cmd.Parameters.Add(new OracleParameter("p_city", OracleDbType.Varchar2)).Value = card.City;
            cmd.Parameters.Add(new OracleParameter("p_state", OracleDbType.Varchar2)).Value = card.State;
            byte[] photoBytes = null;
            if (!string.IsNullOrEmpty(card.Photo))
            {
                var base64Data = card.Photo.Split(',')[1]; // Get the Base64 part
                photoBytes = Convert.FromBase64String(base64Data); // Convert to byte array
            }
            cmd.Parameters.Add(new OracleParameter("p_photo", OracleDbType.Blob)).Value = photoBytes;
            cmd.Parameters.Add(new OracleParameter("p_available_units", OracleDbType.Int32)).Value = card.AvailableUnits;
            cmd.Parameters.Add(new OracleParameter("p_wifi", OracleDbType.Int32)).Value = card.Wifi ? 1 : 0;
            cmd.Parameters.Add(new OracleParameter("p_laundry", OracleDbType.Int32)).Value = card.Laundry ? 1 : 0;
            byte[] pdfBytes = null;
            if (!string.IsNullOrEmpty(card.Pdf))
            {
                var base64Data = card.Pdf.Split(',')[1];
                pdfBytes = Convert.FromBase64String(base64Data);
            }
            cmd.Parameters.Add(new OracleParameter("p_pdf", OracleDbType.Blob)).Value = pdfBytes;

            cmd.ExecuteNonQuery();
        }


        public void UpdateCard(Card card)
        {
            using OracleConnection con = new(ConnStr);
            con.Open();

            using OracleCommand cmd = new();
            cmd.Connection = con;
            cmd.CommandText = "olerning.PKG_SANDRO_CARDS.update_card";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Int32)).Value = card.Id;
            cmd.Parameters.Add(new OracleParameter("p_name", OracleDbType.Varchar2)).Value = card.Name;
            cmd.Parameters.Add(new OracleParameter("p_city", OracleDbType.Varchar2)).Value = card.City;
            cmd.Parameters.Add(new OracleParameter("p_state", OracleDbType.Varchar2)).Value = card.State;
            byte[] photoBytes = null;
            if (!string.IsNullOrEmpty(card.Photo))
            {
                var base64Data = card.Photo.Split(',')[1]; // Get the Base64 part
                photoBytes = Convert.FromBase64String(base64Data); // Convert to byte array
            }
            cmd.Parameters.Add(new OracleParameter("p_photo", OracleDbType.Blob)).Value = photoBytes;
            cmd.Parameters.Add(new OracleParameter("p_available_units", OracleDbType.Int32)).Value = card.AvailableUnits;
            cmd.Parameters.Add(new OracleParameter("p_wifi", OracleDbType.Int32)).Value = card.Wifi ? 1 : 0;
            cmd.Parameters.Add(new OracleParameter("p_laundry", OracleDbType.Int32)).Value = card.Laundry ? 1 : 0;
            byte[] pdfBytes = null;
            if (!string.IsNullOrEmpty(card.Pdf))
            {
                var base64Data = card.Pdf.Split(',')[1]; // Get the Base64 part
                pdfBytes = Convert.FromBase64String(base64Data); // Convert to byte array
            }
            cmd.Parameters.Add(new OracleParameter("p_pdf", OracleDbType.Blob)).Value = pdfBytes;

            cmd.ExecuteNonQuery();
        }

    }
}
