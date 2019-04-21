using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BisAceAPIModels
{
    public class BisCardAuthorization
    {
        [Required]
        public string CardNumber { get; set; }
        public string CardStartValidDate { get; set; }
        public string CardExpiryDate { get; set; }
        public List<ACEAuthorizations> Authorizations { get; set; }
    }
}
