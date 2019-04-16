using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BisAceAPIModels.Models
{
    public class BisCard
    {
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string PersonId { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }
        public string CardStartValidDate { get; set; }
        public string CardExpiryDate { get; set; }
        public List<string> AuthorizationIds { get; set; }
        public string AuthProfileId { get; set; }
    }
}