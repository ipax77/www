using System.Threading.Tasks;

namespace WorldWideWalk
{
    public interface ILocationConsent
    {
        Task<bool> GetLocationConsent();
    }
}
