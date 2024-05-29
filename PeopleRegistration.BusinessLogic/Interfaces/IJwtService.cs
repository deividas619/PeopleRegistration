namespace PeopleRegistration.BusinessLogic.Interfaces
{
    public interface IJwtService
    {
        public string GetJwtToken(string username);
    }
}
