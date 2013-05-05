
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 05/04/2013 20:01:00
-- Generated from EDMX file: C:\Users\tiliska\Documents\Visual Studio 2012\Projects\BookCave\Bookcave.Service.App\Entities\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
--USE [bookcave];
--GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_BookRating]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RatingRecords] DROP CONSTRAINT [FK_BookRating];
GO
IF OBJECT_ID(N'[dbo].[FK_BookSkill]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SkillRecords] DROP CONSTRAINT [FK_BookSkill];
GO
IF OBJECT_ID(N'[dbo].[FK_ContentBook]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContentRecords] DROP CONSTRAINT [FK_ContentBook];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[BookRecords]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BookRecords];
GO
IF OBJECT_ID(N'[dbo].[ContentRecords]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContentRecords];
GO
IF OBJECT_ID(N'[dbo].[RatingRecords]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RatingRecords];
GO
IF OBJECT_ID(N'[dbo].[SkillRecords]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SkillRecords];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'BookRecords'
CREATE TABLE [dbo].[BookRecords] (
    [Isbn13] nvarchar(13)  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Author] nvarchar(max)  NULL,
    [Isbn] nvarchar(max)  NULL,
    [Publisher] nvarchar(max)  NOT NULL,
    [PageCount] smallint  NULL,
    [DocType] nvarchar(max)  NULL,
    [Series] nvarchar(max)  NULL,
    [Awards] nvarchar(max)  NULL,
    [Summary] nvarchar(max)  NULL
);
GO

-- Creating table 'ContentRecords'
CREATE TABLE [dbo].[ContentRecords] (
    [Isbn13] nvarchar(13)  NOT NULL,
    [ScholasticGradeLower] nvarchar(2)  NULL,
    [ScholasticGradeHigher] tinyint  NULL,
    [BarnesAgeYoung] tinyint  NULL,
    [BarnesAgeOld] tinyint  NULL,
    [CommonSensePause] tinyint  NULL,
    [CommonSenseOn] tinyint  NULL,
    [CommonSenseNoKids] bit  NULL,
    [AverageContentAge] float  NULL
);
GO

-- Creating table 'RatingRecords'
CREATE TABLE [dbo].[RatingRecords] (
    [Isbn13] nvarchar(13)  NOT NULL,
    [BarnesAvg] float  NULL
);
GO

-- Creating table 'SkillRecords'
CREATE TABLE [dbo].[SkillRecords] (
    [ScholasticGrade] float  NULL,
    [Dra] nvarchar(5)  NULL,
    [LexScore] smallint  NULL,
    [GuidedReading] nvarchar(1)  NULL,
    [Isbn13] nvarchar(13)  NOT NULL,
    [LexCode] nvarchar(max)  NULL,
    [LexUpdate] datetime  NULL,
    [AverageSkillAge] float  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Isbn13] in table 'BookRecords'
ALTER TABLE [dbo].[BookRecords]
ADD CONSTRAINT [PK_BookRecords]
    PRIMARY KEY CLUSTERED ([Isbn13] ASC);
GO

-- Creating primary key on [Isbn13] in table 'ContentRecords'
ALTER TABLE [dbo].[ContentRecords]
ADD CONSTRAINT [PK_ContentRecords]
    PRIMARY KEY CLUSTERED ([Isbn13] ASC);
GO

-- Creating primary key on [Isbn13] in table 'RatingRecords'
ALTER TABLE [dbo].[RatingRecords]
ADD CONSTRAINT [PK_RatingRecords]
    PRIMARY KEY CLUSTERED ([Isbn13] ASC);
GO

-- Creating primary key on [Isbn13] in table 'SkillRecords'
ALTER TABLE [dbo].[SkillRecords]
ADD CONSTRAINT [PK_SkillRecords]
    PRIMARY KEY CLUSTERED ([Isbn13] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Isbn13] in table 'RatingRecords'
ALTER TABLE [dbo].[RatingRecords]
ADD CONSTRAINT [FK_BookRating]
    FOREIGN KEY ([Isbn13])
    REFERENCES [dbo].[BookRecords]
        ([Isbn13])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Isbn13] in table 'SkillRecords'
ALTER TABLE [dbo].[SkillRecords]
ADD CONSTRAINT [FK_BookSkill]
    FOREIGN KEY ([Isbn13])
    REFERENCES [dbo].[BookRecords]
        ([Isbn13])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Isbn13] in table 'ContentRecords'
ALTER TABLE [dbo].[ContentRecords]
ADD CONSTRAINT [FK_ContentBook]
    FOREIGN KEY ([Isbn13])
    REFERENCES [dbo].[BookRecords]
        ([Isbn13])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
