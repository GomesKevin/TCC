using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCC.BackEnd.API.Core.Models
{
    [Table(name:"TB_PRODUTO")]
    public class Produto
    {
        [Key]
        [Column(name:"CD_PRODUTO")]
        public int Codigo { get; set; }
        [Column(name: "DC_PRODUTO")]
        public string Descricao { get; set; }
    }
}