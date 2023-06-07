using MagicVilla_API.Models.DTOS;

namespace MagicVilla_API.Data
{
    public class VillaStore
    {
        public static List<VillaDto> villaList = new List<VillaDto> {
                new VillaDto { Id = 1, Name = "Indigo", Sqft = 500, Occupancy = 6},
                new VillaDto { Id = 2, Name = "Decamerun", Sqft = 300, Occupancy = 4}
        };
    }
}
