using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BisAceAPIModels
{
    public class BisCardAuthorization
    {
        public string CardNumber { get; set; }
        public string PersonId { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }
        public string CardStartValidDate { get; set; }
        public string CardExpiryDate { get; set; }
        public List<ACEAuthorizations> Authorizations { get; set; }
    }
}
