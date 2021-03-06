USE [master]
GO
/****** Object:  Database [questionnaire]    Script Date: 2016.09.09 周五 11:13:19 ******/
CREATE DATABASE [questionnaire]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'questionnaire', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\questionnaire.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'questionnaire_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\questionnaire_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [questionnaire] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [questionnaire].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [questionnaire] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [questionnaire] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [questionnaire] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [questionnaire] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [questionnaire] SET ARITHABORT OFF 
GO
ALTER DATABASE [questionnaire] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [questionnaire] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [questionnaire] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [questionnaire] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [questionnaire] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [questionnaire] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [questionnaire] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [questionnaire] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [questionnaire] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [questionnaire] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [questionnaire] SET  DISABLE_BROKER 
GO
ALTER DATABASE [questionnaire] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [questionnaire] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [questionnaire] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [questionnaire] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [questionnaire] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [questionnaire] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [questionnaire] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [questionnaire] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [questionnaire] SET  MULTI_USER 
GO
ALTER DATABASE [questionnaire] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [questionnaire] SET DB_CHAINING OFF 
GO
ALTER DATABASE [questionnaire] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [questionnaire] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [questionnaire]
GO
/****** Object:  Table [dbo].[answer]    Script Date: 2016.09.09 周五 11:13:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[answer](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[answer] [nvarchar](max) NULL,
	[belong_user] [nvarchar](50) NULL,
	[belong_q] [int] NULL,
 CONSTRAINT [PK__answer__3213E83F4EA43C29] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Class_Teacher_Info]    Script Date: 2016.09.09 周五 11:13:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Class_Teacher_Info](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ClassNumber] [varchar](20) NULL,
	[Classname] [varchar](50) NOT NULL,
	[Classperiod] [varchar](50) NULL,
	[Classtime] [varchar](50) NULL,
	[Major] [varchar](50) NULL,
	[Projectname] [varchar](100) NULL,
	[Projectkey] [varchar](20) NULL,
	[Projecttype] [varchar](20) NULL,
	[Studenttype] [varchar](20) NULL,
	[Number] [int] NULL,
	[Teachername] [varchar](20) NULL,
	[Teacherposition] [varchar](50) NULL,
	[Workplace] [varchar](100) NULL,
	[Email] [varchar](50) NULL,
	[Phone] [varchar](20) NULL,
	[Telephone] [varchar](20) NULL,
	[Other] [varchar](50) NULL,
	[Teacherinfo] [varchar](max) NULL,
	[Completed] [int] NULL,
	[State] [varchar](10) NULL,
	[Teachereva] [varchar](10) NULL,
	[Classinfo] [varchar](max) NULL,
	[Classeva] [varchar](10) NULL,
	[Teacherbirthday] [varchar](20) NULL,
	[Teacheridentity] [varchar](20) NULL,
	[Bank] [varchar](50) NULL,
	[Bankcard] [char](19) NULL,
	[Thirdpay] [varchar](500) NULL,
	[Classeva_xianxia] [varchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ClassNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[question]    Script Date: 2016.09.09 周五 11:13:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[question](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[introduction] [nvarchar](max) NULL,
	[type] [int] NULL,
	[options] [nvarchar](max) NULL,
	[belong_qn] [int] NULL,
	[star_target] [nvarchar](max) NULL,
 CONSTRAINT [PK__question__3213E83F1EB1ECE8] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[questionnaire]    Script Date: 2016.09.09 周五 11:13:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[questionnaire](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](200) NULL,
	[introduction] [nvarchar](max) NULL,
	[is_open] [int] NULL,
	[publish_code] [nvarchar](50) NULL,
 CONSTRAINT [PK__question__3213E83F24830F3C] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[teacher]    Script Date: 2016.09.09 周五 11:13:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[teacher](
	[id] [int] NOT NULL,
	[name] [nvarchar](max) NULL,
	[position] [nvarchar](max) NULL,
	[workplace] [nvarchar](max) NULL,
	[email] [nvarchar](max) NULL,
	[phone] [nvarchar](max) NULL,
	[telephone] [nvarchar](max) NULL,
	[other] [nvarchar](max) NULL,
	[info] [nvarchar](max) NULL,
	[birthday] [nvarchar](max) NULL,
	[idnumber] [nvarchar](max) NULL,
	[bank] [nvarchar](max) NULL,
	[bankcard] [nvarchar](max) NULL,
	[thirdpay] [nvarchar](max) NULL,
 CONSTRAINT [PK_teacher] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[user]    Script Date: 2016.09.09 周五 11:13:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](20) NULL,
	[pw] [nvarchar](20) NULL,
	[type] [int] NULL,
 CONSTRAINT [PK__user__3214EC07D575BF78] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
USE [master]
GO
ALTER DATABASE [questionnaire] SET  READ_WRITE 
GO
