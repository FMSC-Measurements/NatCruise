using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Design.Models
{
    [Table("LK_FIA")]
    public class FIASpecies
    {
        public string FIACode { get; set; }

        public string CommonName { get; set; }

        public override string ToString()
        {
            return $"{FIACode} - {CommonName}";
        }
    }
}
