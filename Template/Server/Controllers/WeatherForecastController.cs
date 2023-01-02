using Microsoft.AspNetCore.Mvc;
using Template.Core.Application.Contracts.Persistence;
using Template.Domain.Entities;
//using Template.Infrastructure.Persistance.Dapper;
using Template.Shared;

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
            //var customerRepo = _unitOfWork.CustomerRepository;
            var customer = new Customer { Id = 0, Name = "Carlos Alberto" };
            var customerRe = _unitOfWork.GetCustomerRepository();
            await customerRe.AddAsync(customer);
            //await customerRepository.AddAsync(customer);
            await _unitOfWork.Save();

            for (int i = 0; i < 20; i++)
            {
                try
                {
                    //await customerRepository.AddAsync(customer);
                    //await customerRepo.CustomImplementation(customer);
                    await customerRe.AddAsync(customer);
                    //customer.Name = String.Format("Customer #{0}", result);
                    //await customerRepository.UpdateAsync(customer);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    throw;
                }
            }
            await _unitOfWork.Save();

            //for (int i = 0; i < 20; i++)
            //{
            //    try
            //    {
            //        var customer = new Customer { Id = 0, Name = "Carlos Alberto", Middle = "TSTS" };
            //        var result = await customerRepository.AddAsync(customer);
            //        customer.Id = result;
            //        customer.Name = String.Format("Customer #{0}", result);
            //        await customerRepository.UpdateAsync(customer);
            //    }
            //    catch (Exception e)
            //    {
            //        _logger.LogError(e.Message);
            //        throw;
            //    }
            //}
            //await _unitOfWork.Save();
            //for (int i = 0; i < 20; i++)
            //{
            //    var customer = new Customer { Id = 0, Name = "Carlos Alberto" };
            //    var result = await customerRepository.AddAsync(customer);
            //    customer.Id = result;
            //    customer.Name = String.Format("Customer Second #{0}", result);
            //    await customerRepository.UpdateAsync(customer);
            //}
            //await _unitOfWork.Save();
            IEnumerable<Customer> response = new List<Customer>();
            for (int i = 0; i < 1; i++)
            {
                //response = await customerRepository.ListAllAsync();

            }
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