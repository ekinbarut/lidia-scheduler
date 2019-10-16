# LIDIA SCHEDULER


# What is it ?
[Lidia Scheduler](https://www.lidiacommerce.com/lidia-scheduler) is an application that automatically runs the tasks you need to run periodically. So, what are these jobs and how do they run?
Your projects may have tasks that you need to run continuously. Once a day, every hour or every minute... Or you may have applications to make your site more stable. These tasks can be unnecessary workload for you. You can manage any api service you will run on your site through [Lidia Scheduler](https://www.lidiacommerce.com/lidia-scheduler). In [Lidia Scheduler](https://www.lidiacommerce.com/lidia-scheduler), you must create a job to manage your jobs on interface. When creating a job, you must enter a time interval in Cron format. [Lidia Scheduler](https://www.lidiacommerce.com/lidia-scheduler) will run this job with Hangfire.

You can use different methods for your repeated jobs. But, [Lidia Scheduler](https://www.lidiacommerce.com/lidia-scheduler) has different capabilities than other methods. It has different capabilities and continues to be developed.

The API ,that you call in [Lidia Scheduler](https://www.lidiacommerce.com/lidia-scheduler) ,must have  a return value which [Lidia Scheduler](https://www.lidiacommerce.com/lidia-scheduler) can understand.


[Lidia Scheduler](https://www.lidiacommerce.com/lidia-scheduler) interprets that return value and can process according to the content. Existing [Lidia Scheduler](https://www.lidiacommerce.com/lidia-scheduler) capable of sending e-mail. E-mail notification can be used according to the return value of task.

[Lidia Scheduler](https://www.lidiacommerce.com/lidia-scheduler) consists of 4 main layers. There is a one to many relationship between these layer In this way, you can manage your different projects from the interface.

-Tenant,

-Application,

-Collection,

-Job

In addition, 3 different user roles can be create on [Lidia Scheduler](https://www.lidiacommerce.com/lidia-scheduler). These users have access to certain pages. Users are created exclusively for tenants. In this way, you can define different users for your projects.

-SystemAdministrator

-TenantAdministrator

-TenantUser

[Lidia Scheduler](https://www.lidiacommerce.com/lidia-scheduler) continues to be developed. Trigger feature will be available with the next version. Trigger feature can send mail or call api before job runs. After the job runs and ends, it can read the return value from api and send mail or call api.



## Lidia Commerce
Lidia Scheduler is a product of  [Lidia Commerce](https://www.lidiacommerce.com/products)
and is shared as open source. You can visit our website for other products.

## To start using Scheduler

Visit [Lidia Commerce](https://www.lidiacommerce.com/lidia-scheduler)
## Built with Hangfire
*This project developed with hangfire. Hangfire use GNU Licence.
For Permission [Hangfire](https://github.com/HangfireIO/Hangfire#license)

## License

[MIT](https://choosealicense.com/licenses/mit/)
