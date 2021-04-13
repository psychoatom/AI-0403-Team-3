using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebMaskDetection.Models
{
    public class IncidentReport
    {
        [Key] public int IncidentIndex { get; set; }
        public int LocationIndex { get; set; }
        public DateTime IncidentDateTime { get; set; }
        public string Note { get; set; }
        
        public virtual LocationSetting LocationSetting { get; set; }
    }
}