using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CateringBackend.CrossTests.Diets.Requests
{
    public class GetDietResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Calories { get; set; }
        public bool Vegan { get; set; }

    }
}
