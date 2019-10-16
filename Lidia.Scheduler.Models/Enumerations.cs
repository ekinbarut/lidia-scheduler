namespace Lidia.Scheduler.Models
{
    // MISC
    public enum TagType
    {
        General = 0,

        Attribute = 1,

        Badge = 2,

        Group = 3,

        Administrative = 99
    }

    // JOBS
    public enum JobType
    {
        ExecutableFile = 1,

        APIFunction = 2
    }

    public enum JobActivityType
    {
        Exception = -99,

        Error = -1,


        Start = 1,

        End = 2
    }

}
