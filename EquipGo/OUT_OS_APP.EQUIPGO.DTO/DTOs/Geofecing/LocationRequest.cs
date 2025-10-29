using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Geofecing
{
    public class LocationRequest
    {
        public string Serial { get; set; }       
        public double Latitude { get; set; }     
        public double Longitude { get; set; }    
        public DateTime Timestamp { get; set; }  
    }
}
