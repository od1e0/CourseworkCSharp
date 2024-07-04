using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminMap.Models;

namespace AdminMap.Services;

public interface IAttractionService
{
    Task<List<Attraction>> GetAllAttractions();
    Task AddAttraction(Attraction attraction);
    Task UpdateAttraction(Attraction attraction);
    Task DeleteAttraction(Attraction attraction);
}
