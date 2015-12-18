using System;
using System.Data;
using System.Data.SqlClient;
using ClassroomDispatcher.Extensions;

namespace ClassroomDispatcher.Actions
{
    [Label("Attentes ADO")]
    public class AsyncNetworkIo : Dispatcher.SqlDispatchable
    {
        public AsyncNetworkIo(string connectionString)
        : base(connectionString)
        { }

        // merci d'éviter un nombre trop grand d'itérations
        // 1 itération = 1 thread

        public override void Execute(int iterations, int interval, byte etape = 1)
        {
            if (!ShowOk()) return;

            var rd = Servant.ExecuteReader(new SqlCommand("SELECT DISTINCT * FROM Contact.Contact") {CommandType = CommandType.Text});
            while (rd.Read())
            {
                var nom = rd.GetInt32(0);
                System.Threading.Thread.Sleep(interval);
            }
            base.ShowMessage($"exécution de AsyncNetworkIo terminée sur serveur {base.Server}");
        }
    }
}
