using System;
using System.Collections.Generic;
using Model.View;

namespace DataAccess.Interfaces
{
    public interface IZonesRepository :IRepositoryBase<Zone>
    {
        List<Zone> GetZonesForCoreTeamUser(long userId, long masterUserId);
    }
}