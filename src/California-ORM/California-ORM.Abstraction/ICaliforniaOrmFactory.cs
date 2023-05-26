using System;
using System.Data;
using System.Threading.Tasks;

namespace California_ORM.Abstraction
{
    public interface ICaliforniaOrmFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }
    

}
