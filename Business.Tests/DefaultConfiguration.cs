using System;
using System.Configuration;
using Common.Classes;

namespace Business.Tests
{
    public class DefaultTestConfiguration : Common.IConfiguration
    {

        public string NoSqlDataConnectionString
        {
            get
            {
                //return "mongodb://sensecore.cloudapp.net/SenseDB";
                //return Common.Constants.MONGO_CONNECT/ION_STRING_DEV;
                return "mongodb://localhost/SenseDB";
                //return "mongodb://zewa.edea.gr/SenseDB";
                //return "mongodb://sqlsrv/SenseDB";
                //return "mongodb://zewa.edea.gr/SenseDB_CloudMirror";
                //return "mongodb://sensecore.cloudapp.net/SenseDB";
                //return "mongodb://sqlsrv/SenseDB" ;
            }
        }

        public string SqlDataConnectionString
        {
            get
            {
                throw new NotImplementedException("Sql server ConnectionString");
            }
        }
        public SqlConnectionInfo SqlConnectionInfo
        {
            get
            {
                var server = ConfigurationManager.AppSettings["SqlServer"];
                var db = ConfigurationManager.AppSettings["SqlDatabase"];
                var user = ConfigurationManager.AppSettings["SqlUser"];
                var pass = ConfigurationManager.AppSettings["SqlPassword"];
                return new SqlConnectionInfo(server, db, user, pass);
            }

        }

        public double GrasphZonesAlpha { get; private set; }
    }
}