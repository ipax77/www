using System.Collections.Generic;
using System.Threading.Tasks;
using WorldWideWalk.Models;

namespace WorldWideWalk
{
    public interface IRestService
    {
        Task<WalkAppModel> GetWalk(string guid = "7A40C465-BDC8-4373-B6BE-6E49C10D5ECA");
        Task<WwwFeedback> SubmitRun(EntityRunFormData data);
        Task SubmitDebugData(Run run);
        Task<Run> GetDebugData();
    }
}