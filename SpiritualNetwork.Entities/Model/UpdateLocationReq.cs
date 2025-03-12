using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.Entities.Model
{
    public class UpdateLocationReq
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
