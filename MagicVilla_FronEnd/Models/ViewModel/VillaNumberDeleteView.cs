using MagicVilla_FronEnd.Models.DTOS;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_FronEnd.Models.ViewModel
{
    public class VillaNumberUpdateView
    {
        public VillaNumberUpdateView()
        { 
            VillaNumber = new VillaNumberUpdateDto();
        }

        public VillaNumberUpdateDto VillaNumber { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> VillaList { get; set; }
    }
}
