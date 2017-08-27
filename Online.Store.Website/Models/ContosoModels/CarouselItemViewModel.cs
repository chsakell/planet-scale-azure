using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online.Store.Website.Models
{
    public class CarouselItemViewModel
    {
      
        public string ItemHeading { get; set; }
      
        public string ItemDescription { get; set; }

        /// <summary>
        /// This property tells where to redirect on image click.
        /// </summary>
        public string RedirectUrl { get; set; }
       
        public string ImageUrl { get; set; }

        public string AlternateImageUrl { get; set; }
        /// <summary>
        /// This is used to change the class name of the particular division.
        /// </summary>
        public string DivisionClassName { get; set; }

    }
}