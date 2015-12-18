using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;

namespace ClassroomDispatcher.Dispatcher
{
    public class SqlDispatcher
    {
        public string Filename { get; set; }
        public int Iterations { get; private set; }
        public int Interval { get; private set; }
        public byte Etape { get; private set; }
        public List<Station> Stations { get; private set; }
        private ViewModel.ShowMessage _showMessage;

        public SqlDispatcher(int iterations, int interval, ViewModel.ShowMessage showMessage, byte etape = 1)
        {
            Stations = new List<Station>();
            //Stations = stations;
            Iterations = iterations;
            Interval = interval;
            Etape = etape;
            _showMessage = showMessage;
        }

        // une façon de faire, en passant un dérivé de SqlDispatchable en tant que type, à travers le mécanisme de générique et de ses contraintes
        // et en utilisant un Activator pour créer une instance avec un constructeur paramétré.
        // si il n'y a qu'un constructeur pas défaut, on peut utiliser la contrainte new()
        //public async void ExecuteAsync<Workload>(string connectionString) where Workload : SqlDispatchable
        public void ExecuteAsync<Workload>(string connectionString) where Workload : SqlDispatchable
        {
            if (Stations.Count == 0)
            {
                throw new NotImplementedException();
            }

            var cnb = new SqlConnectionStringBuilder(connectionString);
            foreach (var station in Stations)
            {
                cnb.DataSource = station.Server;
                cnb.IntegratedSecurity = false;
                cnb.MultipleActiveResultSets = false;
                cnb.UserID = "sa";
                cnb.Password = "sa";
                //var wl = new Workload(cnb.ConnectionString);
                var wl = (Workload)Activator.CreateInstance(typeof(Workload), cnb.ConnectionString);
                wl.ShowMessage = _showMessage;
                _showMessage(String.Format("Début de l'exécution du dispatch {0} sur le serveur {1}...", wl.GetType(), station.Server));
                Task.Run(() => wl.Execute(Iterations, Interval, Etape)); // peut-on récupérer l'adresse du thread ?
            }
        }

        public void GetServerList(string InstanceName = null)
        {
            Stations.Clear();

            using (var sources = SqlDataSourceEnumerator.Instance.GetDataSources())
            //using (var sources = SmoApplication.EnumAvailableSqlServers())
            {
                foreach (var r in sources.Rows)
                {
                    r.ToString();
                }
            }

            // à la main
            //var stations = from i in instances.Rows.AsQueryable()
            //               //group s by i.ServerName
            //               //select s.Key;
            //               select i;

            //foreach s in stations)
            //{
            //    // Instance.GetDataSources()
            //    var s = new Station();
            //    s.ServerName = s.ServerName; // peut être DBNull ???
            //                                 // s.InstanceName = instance.InstanceName; // peut être DBNull

            //    var i = from i in instances
            //            where i.ServerName == s.ServerName && (i.InstanceName == InstanceName ?? i.InstanceName)
            //            select new SqlInstance() { InstanceName = i.InstanceName };

            //    s.SqlInstances.AddRange(i);

            //    stations.Add(s);
            //}
        }

        public void LoadFile()
        {
            if (Filename == null)
            {
                throw new KeyNotFoundException();
            }
            // TODO - sérialiser en XML
        }

        public void SaveFile()
        {
            if (Filename == null)
            {
                throw new KeyNotFoundException();
            }
            // TODO - sérialiser en XML
        }
    }
}
