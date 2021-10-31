using BackgroundWorkerService;
using Business.Abstract;
using DataAccess.Concrete;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        ICurrencyService _currencyService;
        public IBackgroundTaskQueue _queue { get; }
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CurrenciesController(ICurrencyService currencyService, IBackgroundTaskQueue queue, IServiceScopeFactory serviceScopeFactory)
        {
            _currencyService = currencyService;
            _queue = queue;
            _serviceScopeFactory = serviceScopeFactory;
        }

        [HttpGet("get")]
        public IActionResult Get()
        {
            _queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                using (API_Implementation_Context db = new API_Implementation_Context())
                {
                    var scopedServices = scope.ServiceProvider;
                    if (DateTime.Now.Hour > 8 && DateTime.Now.Hour < 19)
                    {
                        XDocument tcmbdoviz = XDocument.Load("http://www.tcmb.gov.tr/kurlar/today.xml");
                        var usd = tcmbdoviz.Descendants("Currency").Where(x => x.LastAttribute.Value == "USD").FirstOrDefault();
                        var eur = tcmbdoviz.Descendants("Currency").Where(x => x.LastAttribute.Value == "EUR").FirstOrDefault();
                        var gbp = tcmbdoviz.Descendants("Currency").Where(x => x.LastAttribute.Value == "GBP").FirstOrDefault();
                        var chf = tcmbdoviz.Descendants("Currency").Where(x => x.LastAttribute.Value == "CHF").FirstOrDefault();
                        var kwd = tcmbdoviz.Descendants("Currency").Where(x => x.LastAttribute.Value == "KWD").FirstOrDefault();
                        var sar = tcmbdoviz.Descendants("Currency").Where(x => x.LastAttribute.Value == "SAR").FirstOrDefault();
                        var rub = tcmbdoviz.Descendants("Currency").Where(x => x.LastAttribute.Value == "RUB").FirstOrDefault();
                        List<Currency> currencyList = new List<Currency>()
                        {
                        new Currency(){CurrencyCode=usd.LastAttribute.Value,Rate=usd.Element("ForexBuying").Value,Time=DateTime.Now},
                        new Currency(){CurrencyCode=eur.LastAttribute.Value,Rate=eur.Element("ForexBuying").Value,Time=DateTime.Now},
                        new Currency(){CurrencyCode=gbp.LastAttribute.Value,Rate=gbp.Element("ForexBuying").Value,Time=DateTime.Now},
                        new Currency(){CurrencyCode=chf.LastAttribute.Value,Rate=chf.Element("ForexBuying").Value,Time=DateTime.Now},
                        new Currency(){CurrencyCode=kwd.LastAttribute.Value,Rate=kwd.Element("ForexBuying").Value,Time=DateTime.Now},
                        new Currency(){CurrencyCode=sar.LastAttribute.Value,Rate=sar.Element("ForexBuying").Value,Time=DateTime.Now},
                        new Currency(){CurrencyCode=rub.LastAttribute.Value,Rate=rub.Element("ForexBuying").Value,Time=DateTime.Now}
                        };

                        foreach (var item in currencyList)
                        {
                            _currencyService.Add(item);
                        }
                    }
                    await Task.Delay(TimeSpan.FromHours(1), token);

                }
            });
            return Ok("In progress..");
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var result = _currencyService.GetAll();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("getcurrency")]
        public IActionResult GetByCurrencyCode(string currencyCode)
        {
            var result = _currencyService.GetByCurrencyCode(currencyCode);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
