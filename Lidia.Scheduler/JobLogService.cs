using Lidia.Framework.Models;
using Lidia.Framework.Models.Responses;
using Lidia.Scheduler.DataAccess;
using Lidia.Scheduler.Interfaces;
using Lidia.Scheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Lidia.Scheduler
{
    public class JobLogService : IJobLogService
    {
        #region [ Implementation of IService ]

        public ServiceResponse<JobLogEntry> Create(JobLogEntry model)
        {
            using (var bo = new JobLogEntryDAO())
            {
                // Set the missing fields
                model.Created = DateTime.UtcNow;

                //(model as JobLogEntry).Customer.ContactInformation.ForEach(ci => ci.);
                int result = bo.Insert(model as JobLogEntry);
                if (result > 0)
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Success,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>() { model }
                    };
                }
                else
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Error,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>() { model }
                    };
                }
            }
        }

        public ServiceResponse<JobLogEntry> Update(JobLogEntry model)
        {
            using (var bo = new JobLogEntryDAO())
            {
                int result = bo.Update(model as JobLogEntry);
                if (result > 0)
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Success,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>() { model }
                    };
                }
                else
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Error,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>() { model }
                    };
                }
            }
        }

        public ServiceResponse<JobLogEntry> Delete(JobLogEntry model)
        {
            using (var bo = new JobLogEntryDAO())
            {
                bool isDeleted = bo.Delete(model as JobLogEntry, true);
                if (isDeleted)
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Success,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>() { model }
                    };
                }
                else
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Error,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>() { model }
                    };
                }
            }
        }

        public ServiceResponse<JobLogEntry> Delete(Expression<Func<JobLogEntry, bool>> predicate)
        {
            using (var bo = new JobLogEntryDAO())
            {
                var model = bo.Find(predicate);

                bool isDeleted = bo.Delete(model as JobLogEntry, true);
                if (isDeleted)
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Success,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>() { model }
                    };
                }
                else
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Error,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>() { model }
                    };
                }
            }
        }

        public ServiceResponse<JobLogEntry> Find(Expression<Func<JobLogEntry, bool>> predicate)
        {
            using (var bo = new JobLogEntryDAO())
            {
                JobLogEntry result = bo.Find(predicate) as JobLogEntry;

                if (result != null)
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Success,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>() { result }
                    };
                }
                else
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Error,
                        Source = ServiceResponseSources.MsSQL
                    };
                }
            }
        }

        public ServiceResponse<JobLogEntry> FindIncluding(Expression<Func<JobLogEntry, bool>> predicate, params Expression<Func<JobLogEntry, object>>[] includes)
        {
            using (var bo = new JobLogEntryDAO())
            {
                var result = bo.Find(predicate, includes);

                if (result != null)
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Success,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>() { result }
                    };
                }
                else
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Error,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>() { }
                    };
                }
            }
        }

        public ServiceResponse<JobLogEntry> FindIncluding(Expression<Func<JobLogEntry, bool>> predicate, params object[] includes)
        {
            using (var bo = new JobLogEntryDAO())
            {
                var result = bo.Find(predicate, includes);

                if (result != null)
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Success,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>() { result }
                    };
                }
                else
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Error,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>() { }
                    };
                }
            }
        }

        public ServiceResponse<JobLogEntry> Get(Expression<Func<JobLogEntry, bool>> predicate)
        {
            using (var bo = new JobLogEntryDAO())
            {
                var results = bo.GetList(predicate);

                if (results != null && results.Any())
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Success,
                        Source = ServiceResponseSources.MsSQL,
                        Result = results
                    };
                }
                else
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Error,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>()
                    };
                }
            }
        }

        public ServiceResponse<JobLogEntry> GetIncluding(Expression<Func<JobLogEntry, bool>> predicate, params Expression<Func<JobLogEntry, object>>[] includes)
        {
            using (var bo = new JobLogEntryDAO())
            {
                var results = bo.GetList(predicate, includes);

                if (results != null && results.Any())
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Success,
                        Source = ServiceResponseSources.MsSQL,
                        Result = results
                    };
                }
                else
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Error,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>()
                    };
                }
            }
        }

        public ServiceResponse<JobLogEntry> GetIncluding(Expression<Func<JobLogEntry, bool>> predicate, params object[] includes)
        {
            using (var bo = new JobLogEntryDAO())
            {
                var results = bo.GetList(predicate, includes);

                if (results != null && results.Any())
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Success,
                        Source = ServiceResponseSources.MsSQL,
                        Result = results
                    };
                }
                else
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Error,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>()
                    };
                }
            }
        }

        public ServiceResponse<JobLogEntry> GetAll()
        {
            using (var bo = new JobLogEntryDAO())
            {
                List<JobLogEntry> result = bo.GetAll().ToList();
                if (result.Any())
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Success,
                        Source = ServiceResponseSources.MsSQL,
                        Result = result as List<JobLogEntry>,

                    };
                }
                else
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Error,
                        Source = ServiceResponseSources.MsSQL
                    };
                }
            }
        }

        public ServiceResponse<JobLogEntry> GetAllIncluding(params Expression<Func<JobLogEntry, object>>[] includes)
        {
            using (var bo = new JobLogEntryDAO())
            {
                var result = bo.GetAllIncluding(includes).ToList();

                if (result.Any())
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Success,
                        Source = ServiceResponseSources.MsSQL,
                        Result = result as List<JobLogEntry>,

                    };
                }
                else
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Error,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>(),
                    };
                }
            }
        }

        public ServiceResponse<JobLogEntry> GetAllIncluding(params object[] includes)
        {
            using (var bo = new JobLogEntryDAO())
            {
                var result = bo.GetAll(includes).ToList();

                if (result.Any())
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Success,
                        Source = ServiceResponseSources.MsSQL,
                        Result = result as List<JobLogEntry>,

                    };
                }
                else
                {
                    return new ServiceResponse<JobLogEntry>()
                    {
                        Type = ServiceResponseTypes.Error,
                        Source = ServiceResponseSources.MsSQL,
                        Result = new List<JobLogEntry>(),
                    };
                }
            }
        }

        #endregion
    }
}