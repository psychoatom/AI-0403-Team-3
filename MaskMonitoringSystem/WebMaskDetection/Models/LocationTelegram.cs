using System;

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebMaskDetection.Models
{
    public class LocationTelegram
    {
        [Key]public int LT_Index { get; set; }
        public int LocationIndex { get; set; }

        public int ChatID { get; set; }

        public virtual LocationSetting LocationSetting { get; set; }
    }
}