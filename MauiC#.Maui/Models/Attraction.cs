using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiC_.Maui.Models
{
    public class Attraction
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Views { get; set; }
        public string ThreeModelUrl { get; set; }

        public void IncrementViews()
        {
            Views++;
        }
    }
}
