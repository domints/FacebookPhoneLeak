namespace LeakSearchApp.Services
{
    public interface IHashService
    {
        byte[] GeneratePrivateHash(string publicHash);
    }
}