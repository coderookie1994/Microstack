namespace Microstack.Git.Abstractions
{
    public interface ICredentialProvider
    {
        void SetCredentials(string userName, string token, string email);
        (string Username, string Token, string Email) GetCredentials();
    }
}