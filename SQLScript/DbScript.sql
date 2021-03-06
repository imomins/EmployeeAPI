USE [master]
GO
/****** Object:  Database [EmployeeRecords]    Script Date: 9/30/2021 6:03:08 PM ******/
CREATE DATABASE [EmployeeRecords]
Go 
USE [EmployeeRecords]
GO
/****** Object:  Table [dbo].[Employee]    Script Date: 9/30/2021 6:03:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[Id] [varchar](50) NOT NULL,
	[FirstName] [varchar](50) NULL,
	[MiddleName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Employee] ([Id], [FirstName], [MiddleName], [LastName]) VALUES (N'E001', N'John', N'R', N'Brain')
GO
INSERT [dbo].[Employee] ([Id], [FirstName], [MiddleName], [LastName]) VALUES (N'E002', N'', N'', N'')
GO
/****** Object:  StoredProcedure [dbo].[spEmployee]    Script Date: 9/30/2021 6:03:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<mominul,,mom.ruet@gmail.com>
-- Create date: <30-Sep-2021>
-- Description:	<EMployee CRUD Stored Procedure>
-- =============================================
CREATE PROCEDURE [dbo].[spEmployee] 
	 @q int=0,@Id varchar(50)=null, @FirstName varchar(50)=null, 
	 @MiddleName varchar(50)=null, @LastName varchar(50)=null
AS
BEGIN
	 
	SET NOCOUNT ON;
	if(@q=0) begin
		select  Id, FirstName, MiddleName, LastName 
		from [dbo].[Employee]
	end
	else if(@q=1) begin
		select  Id, FirstName, MiddleName, LastName 
		from [dbo].[Employee]
		where Id=@Id
	end
	else if(@q=2) begin
		if not exists(select Id from [dbo].[Employee] where Id=@Id) begin
			 insert into [dbo].[Employee](Id, FirstName, MiddleName, LastName)
			 values(@Id, @FirstName, @MiddleName, @LastName)
		end
		else begin
			update [dbo].[Employee]
			SET FirstName=@FirstName, MiddleName=@MiddleName, LastName=@LastName
			where Id=@Id
		end
	end
	else if(@q=3) begin
		  delete from [dbo].[Employee]
		  where Id=@Id
	end
	 
END
GO
USE [master]
GO
ALTER DATABASE [EmployeeRecords] SET  READ_WRITE 
GO
