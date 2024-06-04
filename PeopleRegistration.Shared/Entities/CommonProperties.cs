using System.ComponentModel.DataAnnotations.Schema;

namespace PeopleRegistration.Shared.Entities
{
    public class CommonProperties
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
    }
}
