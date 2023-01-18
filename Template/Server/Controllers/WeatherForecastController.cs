
using Core.Contracts.Persistence;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;


namespace Template.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IUnitOfWork unitOfWorkEntity)
        {
            _logger = logger;
            _unitOfWork = unitOfWorkEntity;
        }

        //[HttpGet]
        //public async Task<IEnumerable<Customer>> Get()
        //{
        //    return new IEnumerable<Customer>();
        //}
    }
}

//var filter = new QueryFilter
//{
//    Conditions = new List<QueryCondition>
//                {
//                    new QueryCondition
//                    {
//                        Column = "MyColumn",
//                        Value = "MyValue"

//                    }
//                },
//    OrderByColumns = new List<OrderByColumn>
//                {
//                    new OrderByColumn
//                    {
//                        Column = "MyColumn"
//                    }
//                },
//    Limit = 10,
//    Offset = 0
//};