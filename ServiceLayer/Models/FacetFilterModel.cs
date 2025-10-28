using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Models
{
    public class FacetFilterModel
    {
        public Guid FacetId { get; set; }
        public Guid facetValueId { get; set; }
    }
}
