**Project Description**
This adapter was developed for a customer who needed to transfer files independent from any choice of platform. Much concern for security, along with already invested infrastructure, where the main reasons for choosing Sftp before other protocols like Ftps.

_Blogical.Shared.Adapters.Sftp_ utilizes an open source library for the actual communication with the sftp/ssh server. The library is developed by Tamir Gal. For more information: [http://www.tamirgal.com/home/dev.aspx?Item=SharpSsh](http://www.tamirgal.com/home/dev.aspx?Item=SharpSsh).

The adapter has implemented a more sophisticated scheduler for setting up the polling interval. Rather than stating this property in milliseconds, you’re able to set a timely, daily, weekly or even a monthly scheduled polling interval. This part of the code comes from the Biztalk Scheduled Task Adapter project wich you can find out more about here: [http://biztalkscheduledtask.codeplex.com/](http://biztalkscheduledtask.codeplex.com/).

**+Features+**
* **Receive port load balancing** 
* **Scheduled polling interval**
* **Notify on "Empty Batch" message** - the adapter will submit an error message if the receive endpoint didn’t pick up any files. This property is disabled by default.
* **Publickey authentication** - (RSA, DSA)
* **Sftp connection pooling**
* **Host throttling** - restrict number of open connections to any sftp host

+Finally, you may use this adapter as you like, but please keep me posted for bugs you fix. This way my former customer will benefit from sharing this adapter with the open source community.+

**For more information: [http://blogical.se/blogs/mikael](http://blogical.se/blogs/mikael)**

**For more information about varius ftp protocols: [http://blogical.se/blogs/johan/archive/2008/02/03/what-kind-of-ftp-did-you-say-you-used.aspx](http://blogical.se/blogs/johan/archive/2008/02/03/what-kind-of-ftp-did-you-say-you-used.aspx)**

**See also Blogical Database Caching Lookup Functoids: [http://www.codeplex.com/BlogicalFunctoids](http://www.codeplex.com/BlogicalFunctoids)**
