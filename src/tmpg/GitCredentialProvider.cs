using Microstack.Git.Abstractions;

namespace Microstack.Git
{
    public class GitCredentialProvider : ICredentialProvider
    {
        private string Username { get; set; }
        private string Token { get; set; }
        private string Email { get; set; }
        public (string Username, string Token, string Email) GetCredentials() => (Username, Token, Email);

        public void SetCredentials(string userName, string token, string email)
        {
            Username = userName;
            Token = token;
            Email = email;
        }
    }
}