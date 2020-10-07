using System.Threading.Tasks;

namespace Microstack.Git.Abstractions
{
    public interface IGitOps
    {
        Task Pull(string gitPath);
        string Clone(string remote, string branch, string gitBranchName);
        Task PullInTemp(string gitRoot, string branchName);
    }
}