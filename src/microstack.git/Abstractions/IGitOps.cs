using System.Threading.Tasks;

namespace microstack.git
{
    public interface IGitOps
    {
        Task Pull(string gitPath);
    }
}