using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoSqlDataAccess
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Save Changes had been Made to Repositories
        /// </summary>
        void Save();
    }
}
