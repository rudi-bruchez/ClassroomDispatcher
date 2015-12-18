using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Xml;

namespace ClassroomDispatcher.Outils
{
    public class Zephyr
    {
        public string ConnectionString { get; private set; }
        public bool Connected => (Connection.State == System.Data.ConnectionState.Open);
        public SqlConnection Connection { get; }

        public Zephyr(string connectionString)
        {
            ConnectionString = connectionString;
            Connection = new SqlConnection(ConnectionString);
            try
            {
                Connection.Open();

            }
            catch (Exception e)
            {
                MessageBox.Show($"erreur à la connexion : {e.Message}", "Connexion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        ~Zephyr()
        {
            if (Connection.State != System.Data.ConnectionState.Closed)
            {
                try
                {
                    Connection.Close();
                }
                finally
                {
                    Connection.Dispose();
                }
            }
        }

        public void ExecuteNonQuery(string qry)
        {
            using (var cmd = new SqlCommand(qry, Connection))
            {
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }

        internal SqlDataReader ExecuteReader(SqlCommand cmd)
        {
            cmd.Connection = Connection;
            try
            {
                return cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                MessageBox.Show($"erreur à l'exécution : {e.Message}", "Exécution", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        internal XmlReader ExecuteXmlReader(SqlCommand cmd)
        {
            cmd.Connection = Connection;
            try
            {
                return cmd.ExecuteXmlReader();
            }
            catch (Exception e)
            {
                MessageBox.Show($"erreur à l'exécution : {e.Message}", "Exécution", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

        }

        internal void FreeProcCache()
        {

        }
    }
}
