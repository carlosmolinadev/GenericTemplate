
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

        [HttpGet]
        public async Task<IEnumerable<Customer>> Get()
        {
            var customerRepository = _unitOfWork.GetRepository<CustomerDTO>();
            var addressRepository = _unitOfWork.GetRepository<Address>();
            var customer = new CustomerDTO { Id = 0, Name = "Carlos Alberto", CreatedAt = DateTime.Now };
            customer.Id = await customerRepository.AddAsync(customer);
            var address = new Address { Id = 1, Name = "Address1", CustomerId = customer.Id };

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    await addressRepository.AddAsync(address);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    throw;
                }
            }
            var filter = new QueryFilter
            {
                Conditions = new List<QueryCondition>
                {
                    new QueryCondition
                    {
                        Column = "customer_id",
                        Value = 69

                    }
                },
                //OrderByColumns = new List<OrderByColumn>
                //{
                //    new OrderByColumn
                //    {
                //        Column = "MyColumn"
                //    }
                //},
                //Limit = 10,
                //Offset = 0
            };
            var address1 = await addressRepository.GetByIdAsync(1);
            var customer1 = await customerRepository.GetByIdAsync(69);
            var addresses = await addressRepository.GetFilteredAsync(filter);
            var response = await customerRepository.GetAllAsync();
            

            await _unitOfWork.Save();

            return response.ToArray();
            

            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = Random.Shared.Next(-20, 55),
            //    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            //})
            //.ToArray();
        }
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