
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreCodeCamp.Models
{
    // Representa o contrato da entidade.
    // Possui somente atributos que queira expor
    public class CampModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        // Serve de chave primária visível para o cliente.
        [Required]
        public string Moniker { get; set; } 
        public DateTime EventDate { get; set; } = DateTime.MinValue;
        [Range(1,100)]
        public int Length { get; set; } = 1;

        // Location
        /*
         * Ao se utilizar o nome da entidade como prefixo,
         * o AutoMapper irá realizar um bind automático, 
         * não sendo necessário criar uma LocationModel caso não queiramos retornar
         * todos os dados de Location associados a um camp
         */
        public string LocationVenueName { get; set; }
        public string LocationAddress1 { get; set; }
        public string LocationAddress2 { get; set; }
        public string LocationAddress3 { get; set; }
        public string LocationCityTown { get; set; }
        public string LocationStateProvince { get; set; }
        public string LocationPostalCode { get; set; }
        public string LocationCountry { get; set; }

        //Talk     
        public ICollection<TalkModel> Talks { get; set; }
    }
}