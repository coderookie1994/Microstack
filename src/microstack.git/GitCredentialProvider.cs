using Microstack.Git.Abstractions;

namespace Microstack.Git
{
    public class GitCredentialProvider : ICredentialProvider
    {
        private string Username { get; set; }
        private string Token { get; set; }
        public (string Username, string Token) GetCredentials() => (Username, Token);

        public void SetCredentials(string userName, string token)
        {
            Username = userName;
            Token = token;
        }
    }
}