using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Design.Models
{
    [Table("Reports")]
    public class Reports
    {
        public string ReportID { get; set; }

        public bool? Selected { get; set; }

        public string Title { get; set; }
    }
}