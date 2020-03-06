using FMSC.ORM.EntityModel.Attributes;

namespace FScruiser.Models
{
    public class Device
    {
        public string DeviceID { get; set; }
        public string Name { get; set; }

        [Field(Name = nameof(LastModified), PersistanceFlags = PersistanceFlags.Never)]
        public string LastModified { get; set; }
    }
}