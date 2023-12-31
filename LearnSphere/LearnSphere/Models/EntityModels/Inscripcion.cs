﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LearnSphere.Models.EntityModels
{
    public class Inscripcion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Fecha_Inscripcion { get; set; }
       
      
        public string Id_Curso { get; set; }

        
        public Curso? Curso { get; set; }

        public int IdUsuario { get; set; }

       
        public Usuario? Usuario { get; set; }

        public List<Calificacion> Calificaciones { get; set; }


    }
}
