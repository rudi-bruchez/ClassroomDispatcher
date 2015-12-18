using System.Data;
using System.Data.SqlClient;
using ClassroomDispatcher.Extensions;

namespace ClassroomDispatcher.Actions
{
    [Label("Remplissage Plan Cache")]
    public class FillPlanCache : Dispatcher.SqlDispatchable
    {
        //private string _connectionString { get; private set; }

        public FillPlanCache(string connectionString)
        : base(connectionString)
        { }

        // merci d'éviter un nombre trop grand d'itérations
        // 1 itération = 1 thread

        public override void Execute(int iterations, int interval, byte etape = 1)
        {
            if (!ShowOk()) return;

            var rd = Servant.ExecuteReader(new SqlCommand("SELECT DISTINCT Nom FROM Contact.Contact") {CommandType = CommandType.Text});

            while (rd.Read())
            {
                var sql = $"SELECT * FROM Contact.Contact WHERE nom = '{rd.GetString(0).Replace("'", "''")}';";
                Servant.ExecuteReader(new SqlCommand(sql) {CommandType = CommandType.Text});
            }
        }
    }
}
