namespace panelApi.Repository.IRepository
{
    public interface ITokenGenerator
    {
        string GetToken(int Id, string roleName);
    }
}
