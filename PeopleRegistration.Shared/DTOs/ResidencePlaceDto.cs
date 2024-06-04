using PeopleRegistration.Shared.Attributes;

namespace PeopleRegistration.Shared.DTOs
{
    public class ResidencePlaceDto
    {
        //[CityValidation]
        public string City { get; set; }
        //[StreetValidation]
        public string Street { get; set; }
        //[HouseNumberValidation]
        public string HouseNumber { get; set; }
        //[ApartmentValidation]
        public string? ApartmentNumber { get; set; }
        public ResidencePlaceDto() { }
    }
}
