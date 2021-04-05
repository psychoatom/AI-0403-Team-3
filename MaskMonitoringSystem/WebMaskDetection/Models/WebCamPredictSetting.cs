using System;

using System.ComponentModel.DataAnnotations;

namespace WebMaskDetection.Models
{
    public class WebCamPredictSetting
    {
        [Key]public int wp_index { get; set; }
        public int WebCamInterval { get; set; }

        public decimal PredictionThreshold { get; set; }
    }
}