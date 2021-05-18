using System.Collections.Generic;
using System.Threading.Tasks;
using WorldWideWalk.Models;

namespace WorldWideWalk
{
    public interface IRestService
    {
        Task SubmitDebugData(Run run);
        Task<Run> GetDebugData();
    }
}