using System;

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebMaskDetection.Models
{
    public class LocationSetting
    {
        [Key]public int locationIndex { get; set; }
        public string Location { get; set; }

        public virtual List<IncidentReport> IncidentReport { get; set; }
        public virtual List<LocationEmail> LocationEmails { get; set; }
        public virtual List<LocationTelegram> LocationTelegrams { get; set; }
    }
}