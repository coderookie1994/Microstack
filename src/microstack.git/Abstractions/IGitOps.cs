using System.Threading.Tasks;

namespace microstack.git
{
    public interface IGitOps
    {
        Task Pull(string gitPath);
        string Clone(string remote, string branch, string gitBranchName);
        Task PullInTemp(string gitRoot, string branchName);
    }
}