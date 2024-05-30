using PeopleRegistration.Shared.Attributes;

namespace PeopleRegistration.Shared.Entities
{
    public class ResidencePlace : CommonProperties
    {
        [CityValidation]
        public string City { get; set; }
        [StreetValidation]
        public string Street { get; set; }
        [HouseNumberValidation]
        public string HouseNumber { get; set; }
        [ApartmentValidation]
        public string? ApartmentNumber { get; set; }
        public virtual PersonInformation PersonInformation { get; set; }
        public ResidencePlace() { }
    }
}
