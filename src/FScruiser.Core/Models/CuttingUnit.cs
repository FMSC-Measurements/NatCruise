using FMSC.ORM.EntityModel;
using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FScruiser.Models
{
    [Table("CuttingUnit")]
    public class CuttingUnit : INPC_Base
    {
        [Field("Code")]
        public string Code { get; set; }

        [Field("Description")]
        public string Description { get; set; }

        [Field("Area")]
        public string Area { get; set; }

        public override string ToString()
        {
            return $"{Code}: {Description} Area: {Area}";
        }
    }
}