
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
            var customerRepository = _unitOfWork.GetRepository<Customer>();
            var addressRepository = _unitOfWork.GetRepository<Address>();
            var customer = new Customer { Id = 0, Name = "Carlos Alberto", CreatedAt = DateTime.Now };
            var customerId = await customerRepository.AddAsync(customer);
            var address = new Address { Id = 1, Name = "Address1", CustomerId = customerId };
            customer.Id = customerId;
            customer.Name = String.Format("Customer #{0}", customerId);
            await customerRepository.UpdateAsync(customer);
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

            var response = await customerRepository.GetAllAsynch();
            var customerAddress = await addressRepository.GetAllAsynch();
            for (int i = 0; i < response.Count; i++)
            {
                response[i].Address = customerAddress.FirstOrDefault();
            }
            //for (int i = 0; i < customerAddress.Count; i++)
            //{
            //    if(response[i].Address.Id == customerId)
            //    {
            //        response[i].Address = cus
            //    }
            //}

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