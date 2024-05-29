using System.ComponentModel;

namespace PeopleRegistration.Shared.Entities
{
    public class CommonProperties
    {
        [DefaultValue(typeof(Guid), "00000000-0000-0000-0000-000000000000")]
        public Guid Id { get; set; }
    }
}
