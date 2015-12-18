using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassroomDispatcher.Outils;

namespace ClassroomDispatcher.Dispatcher
{
    public abstract class SqlDispatchable
    {
        protected Outils.Zephyr Servant { get; set; }
        public string ConnectionString { get; }
        public ViewModel.ShowMessage ShowMessage { protected get; set; }
        protected string Server => (new SqlConnectionStringBuilder(ConnectionString)).DataSource;

        protected SqlDispatchable(string connectionString)
        {
            ConnectionString = (new SqlConnectionStringBuilder(connectionString) {MultipleActiveResultSets = true}).ConnectionString;
            Servant = new Zephyr(ConnectionString);
        }

        public abstract void Execute(int iterations, int interval, byte etape = 1);

        protected bool ShowOk()
        {
            if (Server != null)
            {
                ShowMessage($"exécution de {GetType().Name} sur serveur {Server}");
                return true;
            }
            else
            {
                ShowMessage($"ERREUR d'exécution de {GetType().Name} sur [{ConnectionString}]");
                return false;
            }

        }
    }
}
