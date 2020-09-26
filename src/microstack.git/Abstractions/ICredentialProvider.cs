namespace microstack.git.Abstractions
{
    public interface ICredentialProvider
    {
        void SetCredentials(string userName, string token);
        (string Username, string Token) GetCredentials();
    }
}