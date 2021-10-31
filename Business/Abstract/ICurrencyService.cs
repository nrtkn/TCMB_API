using Core.DataAccess;
using Core.Utilities.Results;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface ICurrencyService 
    {
        IDataResult<List<Currency>> GetAll();
        IDataResult<List<Currency>> GetByCurrencyCode(string currencyCode);
        IResult Add(Currency currency);
    }
}
