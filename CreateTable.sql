/*
Table [dbo].[SftpWorkingProcess]
The SftpWorkingProcess table is used for load balancing between multiple hosts. The first node to pick up a file
will "check-out" the file by inserting a row to the SftpWorkingProcess table. When the file is removed from its 
original location, the CheckInFile method in the Blogical.Shared.Adapters.Sftp.DataBaseHelper class is called. This 
will remove the file entry in the table.

To enable load balancing:
1. Create a new database, and run the script below to create the SftpWorkingProcess table.
2. Edit the BTSNTSvc.exe.config file (default C:\Program Files\Microsoft BizTalk Server 2006) and add the connection
   string AT THE END OF THE FILE. Add it at the top of the config file, will cause an error, and the BizTalk host
   won't start. 
   
   Sample:
   <connectionStrings>
		<add name="Blogical.Shared.Adapters.Sftp" 
			connectionString="Data Source=localhost;Initial Catalog=Blogical;Integrated Security=SSPI;" 
			providerName="System.Data.SqlClient"/>
   </connectionStrings>
   
   Change the "Initial Catalog" to whatever you named your database, and make sure the process running your
   host instance, has sufficient rights to the table.
   
3. Create a new Receive Location (or open an existing one), and enable the UseLoadBalancing property.

*/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SftpWorkingProcess](
	[ID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_SftpWorkingProcess_ID]  DEFAULT (newid()),
	[URI] [varchar](255) NOT NULL,
	[Node] [varchar](255) NOT NULL,
	[FileName] [varchar](255) NOT NULL,
	[Timestamp] [datetime] NOT NULL CONSTRAINT [DF_SftpWorkingProcess_Timestamp]  DEFAULT (getdate()),
 CONSTRAINT [PK_SftpWorkingProcess] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF

