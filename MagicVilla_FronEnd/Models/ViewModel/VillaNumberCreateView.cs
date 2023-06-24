using MagicVilla_FronEnd.Models.DTOS;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_FronEnd.Models.ViewModel
{
    public class VillaNumberCreateView
    {
        public VillaNumberCreateView()
        { 
            VillaNumber = new VillaNumberCreateDto();
        }

        public VillaNumberCreateDto VillaNumber { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> VillaList { get; set; }
    }
}
