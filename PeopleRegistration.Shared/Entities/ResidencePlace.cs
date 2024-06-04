namespace PeopleRegistration.Shared.Entities
{
    public class ResidencePlace : CommonProperties
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string? ApartmentNumber { get; set; }
        public virtual PersonInformation PersonInformation { get; set; }
        public ResidencePlace() { }
    }
}
