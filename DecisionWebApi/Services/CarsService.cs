using System.Linq;
using DecisionWebApi.DbWorkers;
using DecisionWebApi.Models;
using Microsoft.Extensions.Configuration;

namespace DecisionWebApi.Services
{
    public class CarsService : Service<Car>
    {
        public CarsService(IDbWorker<Car> dbWorker) : base(dbWorker)
        {
        }
    }
}