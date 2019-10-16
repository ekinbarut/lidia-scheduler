using Lidia.Framework.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lidia.Scheduler.Interfaces
{
    public interface IService<T> where T: class
    {
        ServiceResponse<T> Create(T model);

        ServiceResponse<T> Update(T model);

        ServiceResponse<T> Delete(T model);

        ServiceResponse<T> Delete(Expression<Func<T, bool>> predicate);

        ServiceResponse<T> Find(Expression<Func<T, bool>> predicate);

        ServiceResponse<T> FindIncluding(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        ServiceResponse<T> FindIncluding(Expression<Func<T, bool>> predicate, params object[] includes);

        ServiceResponse<T> Get(Expression<Func<T, bool>> predicate);

        ServiceResponse<T> GetIncluding(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        ServiceResponse<T> GetIncluding(Expression<Func<T, bool>> predicate, params object[] includes);

        ServiceResponse<T> GetAll();

        ServiceResponse<T> GetAllIncluding(params Expression<Func<T, object>>[] includes);

        ServiceResponse<T> GetAllIncluding(params object[] includes);
    }
}
