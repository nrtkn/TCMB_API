using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    public class CurrencyManager : ICurrencyService
    {
        ICurrencyDAL _currencyDAL;

        public CurrencyManager(ICurrencyDAL currencyDAL)
        {
            _currencyDAL = currencyDAL;
        }

        public IResult Add(Currency currency)
        {
            _currencyDAL.Add(currency);
            
            return new SuccessResult();
        }

        public IDataResult<List<Currency>> GetByCurrencyCode(string currencyCode)
        {

            var result = _currencyDAL.GetAll(x => x.CurrencyCode == currencyCode);
            return new SuccessDataResult<List<Currency>>(result);
        }

        public IDataResult<List<Currency>> GetAll()
        {
            return new SuccessDataResult<List<Currency>>(_currencyDAL.GetAll());
        }
    }
}
