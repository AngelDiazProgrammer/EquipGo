using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipGoAgent.Dtos
{
    public class LocationRequestDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Serial { get; set; } = string.Empty;
    }
}
