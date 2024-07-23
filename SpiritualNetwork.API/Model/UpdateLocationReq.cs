using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.API.Model
{
    public class UpdateLocationReq
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
