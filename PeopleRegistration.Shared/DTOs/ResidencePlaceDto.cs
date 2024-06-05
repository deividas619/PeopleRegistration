using PeopleRegistration.Shared.Attributes;
using System.Text.Json.Serialization;

namespace PeopleRegistration.Shared.DTOs
{
    public class ResidencePlaceDto
    {
        //[CityValidation]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string City { get; set; }
        //[StreetValidation]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Street { get; set; }
        //[HouseNumberValidation]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string HouseNumber { get; set; }
        //[ApartmentValidation]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? ApartmentNumber { get; set; }
        public ResidencePlaceDto() { }
    }
}
