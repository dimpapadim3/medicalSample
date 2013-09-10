//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Common.Classes;

//namespace NoSqlDataAccess
//{
//    public static class Controller
//    {
//        private static Common.Classes.NoSqlConnectionInfo connectionInfo = null;

//        static Controller()
//        {
//            connectionInfo = connectionInfo = new Common.Classes.NoSqlConnectionInfo( "sqlsrv",
//                "senseapp", "sa", "123$Qwer");
//        }

//        public static Common.Classes.NoSqlConnectionInfo GetConnectionInfo()
//        {
//            return connectionInfo;
//        }

//        public static void RefreshSettings()
//        {
//            connectionInfo = new Common.Classes.NoSqlConnectionInfo("sqlsrv",
//                "senseapp", "sa", "123$Qwer");
            
//        }
//    }
//}
