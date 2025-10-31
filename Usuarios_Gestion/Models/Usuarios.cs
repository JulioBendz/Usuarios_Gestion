using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Usuarios_Gestion.Models
{
    public class Usuarios
    {
        [Key]
        public int Codigo { get; set; }

        [Required, MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Contrasena { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        public DateTime Fecha { get; set; }

        // Propiedad C# renombrada a CodigoArea, mapeada a la columna SQL "Codigo_Area"
        [Column("Codigo_Area")]
        public int CodigoArea { get; set; }

        [ForeignKey(nameof(CodigoArea))]
        public Area? Area { get; set; }
    }
}
