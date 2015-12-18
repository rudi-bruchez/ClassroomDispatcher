using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassroomDispatcher.Dispatcher
{
    public enum SqlVersion
    {
        v2005,
        v2008,
        v2008R2,
        v2012,
        v2014,
        v2016
    };

    public class SqlInstance
    {
        public SqlVersion SqlServerVersion { get; set; }
        public string InstanceName { get; set; }
    }

    public class Station
    {
        public List<SqlInstance> SqlInstances { get; set; }
        public string Server { get; set; }
        public string Participant { get; set; }
        public Task ThreadTask { get; set; }
    }
}
