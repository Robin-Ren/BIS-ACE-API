using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BisAceAPIModels.Models
{
    public class BisHttpResultBase
    {
        public bool IsSucceeded { get; set; }
        public string Remarks { get; set; }
    }
}