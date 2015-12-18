using System;
using ClassroomDispatcher.Extensions;

namespace ClassroomDispatcher.Actions
{
    [Label("Auto Close")]
    public class AutoClose: Dispatcher.SqlDispatchable
    {
        public AutoClose(string connectionString)
        : base(connectionString)
        { }

        /// <summary>
        /// Les effet de l'option AUTO_CLOSE sur une base de données
        /// </summary>
        public override void Execute(int iterations, int interval, byte etape = 1)
        {
            if (!ShowOk()) return;

            // TODO - enlever le pool de connexions
            var qry = @"
                USE Master; 
                ALTER DATABASE PachaDataFormation SET SINGLE_USER WITH ROLLBACK IMMEDIATE; 
                ALTER DATABASE PachaDataFormation SET AUTO_CLOSE ON WITH NO_WAIT;
                ALTER DATABASE PachaDataFormation SET MULTI_USER; 
            ";
            Servant.ExecuteNonQuery(qry);

            // alimentation en boucle
            for (var i = 0; i <= iterations; i++)
            {
                qry = "USE Master; USE PachadataFormation;";
                Servant.ExecuteNonQuery(qry);
            }
        }
    }
}
