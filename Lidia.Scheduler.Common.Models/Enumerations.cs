namespace Lidia.Scheduler.Common.Models
{
    // SERVICES
    public enum ServiceResponseTypes
    {
        Timeout = -999,

        Error = -99,

        Declined = -2,

        Pending = -1,

        Unknown = 0,

        Completed = 1,

        Redirected = 2,

        Approved = 3,

        Success = 4
    }
}
