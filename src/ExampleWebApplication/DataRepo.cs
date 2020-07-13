using System.Collections.Generic;

namespace ExampleWebApplication
{
    public interface IConnectionManager
    {
        string GetConnectionString();
    }

    public class ConnectionManager : IConnectionManager
    {
        public string GetConnectionString()
        {
            return "some string";
        }
    }

    public interface IDataRepo
    {
        IEnumerable<string> GetNames();
    }

    public class DataRepo : IDataRepo
    {
        private readonly string _connectionString;

        public DataRepo(IConnectionManager connectionManager)
        {
            _connectionString = connectionManager.GetConnectionString();
        }

        public IEnumerable<string> GetNames()
        {
            return new List<string> { "marcus", "king" };
        }
    }

    public class DataRepoOther : IDataRepo
    {
        private readonly string _connectionString;

        public DataRepoOther(IConnectionManager connectionManager)
        {
            _connectionString = connectionManager.GetConnectionString();
        }

        public IEnumerable<string> GetNames()
        {
            return new List<string> { "nik", "clarkson" };
        }
    }
}
