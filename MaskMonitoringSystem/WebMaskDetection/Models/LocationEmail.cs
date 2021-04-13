using System;

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebMaskDetection.Models
{
    public class LocationEmail
    {
        [Key]public int LE_Index { get; set; }
        [Display(Name = "Location")]
        public int LocationIndex { get; set; }

        public string Email { get; set; }

        public virtual LocationSetting LocationSetting { get; set; }
    }
}