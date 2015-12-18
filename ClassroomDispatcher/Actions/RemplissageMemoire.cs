using System.Data;
using System.Data.SqlClient;
using ClassroomDispatcher.Extensions;

namespace ClassroomDispatcher.Actions
{
    [Label("Remplissage Mémoire")]
    internal class RemplissageMemoire : Dispatcher.SqlDispatchable
    {
        public RemplissageMemoire(string connectionString)
        : base(connectionString)
        { }

        public override void Execute(int iterations, int interval, byte etape = 1)
        {
            if (!ShowOk()) return;

            for (var i = 1; i <= iterations; i++)
            {
                var sql = string.Format(@"
	                DECLARE cur{0} CURSOR KEYSET
	                FOR SELECT Nom FROM Contact.Contact;

	                DECLARE @nom varchar(50)

	                OPEN cur{0}
	                FETCH NEXT FROM cur{0} INTO @nom
                ", i.ToString());
                    
                Servant.ExecuteNonQuery(sql);
                ShowMessage($"itération {i}");
                System.Threading.Thread.Sleep(interval);
            }
        }

    }
}
