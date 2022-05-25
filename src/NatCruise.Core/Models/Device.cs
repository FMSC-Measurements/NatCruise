using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Models
{
    public class Device
    {
        public string CruiseID { get; set; }
        public string DeviceID { get; set; }
        public string Name { get; set; }

        [Field(Name = nameof(LastModified), PersistanceFlags = PersistanceFlags.Never)]
        public string LastModified { get; set; }
    }
}