using PlatformService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.SyncDataServices.Http
{
    public interface IHttpDeletingDataClient
    {
        Task DeleteUserData(SingleStringClass obj);
    }
}
