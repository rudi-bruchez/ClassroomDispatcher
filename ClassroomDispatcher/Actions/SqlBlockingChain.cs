using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ClassroomDispatcher.Extensions;

namespace ClassroomDispatcher.Actions
{
    [Label("Chaîne de blocage")]
    internal class SqlBlockingChain : ClassroomDispatcher.Dispatcher.SqlDispatchable
    {
        //private string _connectionString { get; private set; }

        public SqlBlockingChain(string connectionString)
        : base(connectionString) { }

        // merci d'éviter un nombre trop grand d'itérations
        // 1 itération = 1 thread

        public override void Execute(int iterations, int interval, byte etape = 1)
        {
            if (!ShowOk()) return;

            int contactId;
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = Servant.Connection;
                cmd.CommandText = "SELECT TOP 1 ContactId FROM Contact.Contact";
                cmd.CommandType = System.Data.CommandType.Text;
                contactId = (int)cmd.ExecuteScalar();

                cmd.CommandText = $"BEGIN TRAN; UPDATE Contact.Contact SET Nom = REVERSE(Nom) WHERE ContactId = {contactId}";
                cmd.ExecuteNonQuery();
            }

            RunQueries(iterations, interval, contactId);
        }

        private async void RunQueries(int iterations, int interval, int contactId)
        {
            for (var i = 0; i <= iterations; i++)
		    {
                await RunQuery(contactId);
            }
        }

        private async Task RunQuery(int contactId)
        {
            using (var cn = new SqlConnection(base.ConnectionString))
            {
                cn.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = $"SELECT Nom FROM Contact.Contact WHERE ContactId = {contactId}";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteScalar();
                }
                cn.Close();
            }
        }
    }

}
