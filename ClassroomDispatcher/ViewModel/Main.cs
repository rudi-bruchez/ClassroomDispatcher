using ClassroomDispatcher.Dispatcher;
using PachaDataManager.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ClassroomDispatcher.Extensions;

namespace ClassroomDispatcher.ViewModel
{
    public delegate void ShowMessage(string message);

    internal class ListViewLabel
    {
        public string ClassName { get; set; }
        public string Label { get; set; }
    }

    internal class Main: INotifyPropertyChanged
    {
        #region private and public members
        private ShowMessage _showmessage;
        private static readonly char[] NewLineChars = Environment.NewLine.ToCharArray();
        public SynchronizedObservableCollection<string> Messages { get; set; }
        public string ConnectionString { get; set; }
        public int Iterations { get; set; }
        public int Interval { get; set; }

        public static string TrimNewLines(string text)
        {
            return text.TrimEnd(NewLineChars);
        }

        private string[] _dispatchServers;
        private const string DefaultConnectionString = @"data source=RUDI-X230\EXPRESS2012;initial catalog=PachaDataFormation;User Id=sa;Password=sa;MultipleActiveResultSets=True;App=PachaDataManager";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string DispatcherServers {
            get
            {
                return _dispatchServers == null ? null : string.Join(Environment.NewLine, _dispatchServers);
            }
            set {
                if (value != null)
                    _dispatchServers = value.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
        }
        #endregion

        public Main()
        {
            _showmessage = new ShowMessage(AfficheMessage);
            Messages = new SynchronizedObservableCollection<string>();

            // connection string
            if (ConfigurationManager.ConnectionStrings != null && ConfigurationManager.ConnectionStrings["default"] != null)
            {
                var cns = ConfigurationManager.ConnectionStrings["default"].ConnectionString ?? DefaultConnectionString;
                ConnectionString = cns;
            }
            else
            {
                ConnectionString = DefaultConnectionString;
            }

            // liste des machines
            DispatcherServers = ConfigurationManager.AppSettings["machines"];
        }

        ~Main()
        {
            // connection string
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (ConfigurationManager.ConnectionStrings != null && ConfigurationManager.ConnectionStrings["default"] != null)
                config.ConnectionStrings.ConnectionStrings["default"].ConnectionString = ConnectionString;
            else
            {
                config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings("default", DefaultConnectionString));
            }

            // liste des machines
            if (config.AppSettings.Settings.AllKeys.Contains("machines"))
            {
                config.AppSettings.Settings["machines"].Value = DispatcherServers;
            }
            else
            {
                config.AppSettings.Settings.Add("machines", DispatcherServers);
            }

            config.Save();
        }

        internal void Dispatch(string lviName)
        {
            if (_dispatchServers == null)
            {
                MessageBox.Show("vous devez entrer une liste de serveurs", "Serveurs", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            var dispatcher = new SqlDispatcher(Iterations, Interval, _showmessage);

            foreach (var station in _dispatchServers.Select(s => new Station() { Server = s }))
            {
                dispatcher.Stations.Add(station);
            }

            var t = Type.GetType($"ClassroomDispatcher.Actions.{lviName}");
            //dispatcher.ExecuteAsync<typeof(t)>(ConnectionString);

            var executeAsync = typeof(SqlDispatcher).GetMethod("ExecuteAsync").MakeGenericMethod(t);
            executeAsync.Invoke(dispatcher, new object[]{ConnectionString});
        }

        internal List<ListViewLabel> GetActions()
        {
            var l = new List<ListViewLabel>();
            var actions = Assembly.GetExecutingAssembly().GetTypes().Where(t => string.Equals(t.Namespace, "ClassroomDispatcher.Actions", StringComparison.Ordinal));
            foreach (var t in actions)
            {
                try
                {
                    var customAttributeData = t.CustomAttributes.FirstOrDefault(ca => ca.AttributeType == typeof(Label));
                    if (customAttributeData != null)
                    {
                        var label = customAttributeData.ConstructorArguments[0].Value.ToString();
                        l.Add(new ListViewLabel() {
                            ClassName = t.Name,
                            Label = label
                        });
                    }
                }
                catch (Exception) { }
            }
            return l;
        }

        private void AfficheMessage(string message)
        {
            Messages.Add(message);
        }
    }
}
