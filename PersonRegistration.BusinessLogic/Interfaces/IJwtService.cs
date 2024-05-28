namespace PersonRegistration.BusinessLogic.Interfaces
{
    public interface IJwtService
    {
        public string GetJwtToken(string username);
    }
}
