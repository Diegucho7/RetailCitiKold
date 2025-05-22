using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegracionERP.Domain.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        public int id { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }
        public string? fullname { get; set; }
        public string? email { get; set; }
        public int? id_group { get; set; }
        public int? id_estado { get; set; }
        public string? phone_number { get; set; }
        public DateOnly? birth_date { get; set; }
        public string? image { get; set; }
        public int? id_person { get; set; }
        public string? expiration_date { get; set; }
        public string? themeApp { get; set; }
        public string? style { get; set; }
        public string? reportDesignerConfiguration { get; set; }
    }
}
