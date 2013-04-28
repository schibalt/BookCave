
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 04/25/2013 16:19:49
-- Generated from EDMX file: C:\Users\tiliska\Documents\My Received Files\Projects\BookCave\Bookcave.Service.App\BookcaveModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [bookcave];
GO
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
    [Isbn13] bigint  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Author] nvarchar(max)  NOT NULL,
    [Isbn] nvarchar(max)  NOT NULL,
    [Publisher] nvarchar(max)  NOT NULL,
    [PageCount] smallint  NULL,
    [DocType] nvarchar(max)  NOT NULL,
    [Series] nvarchar(max)  NOT NULL,
    [Summary] nvarchar(max)  NOT NULL,
    [Awards] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ContentRecords'
CREATE TABLE [dbo].[ContentRecords] (
    [Isbn13] bigint  NOT NULL,
    [ScGrdLower] nchar(1)  NULL,
    [ScGrdHigher] smallint  NOT NULL,
    [BnLower] tinyint  NOT NULL,
    [BnHigher] tinyint  NOT NULL
);
GO

-- Creating table 'RatingRecords'
CREATE TABLE [dbo].[RatingRecords] (
    [Isbn13] bigint  NOT NULL,
    [BnAvg] float  NULL
);
GO

-- Creating table 'SkillRecords'
CREATE TABLE [dbo].[SkillRecords] (
    [ScGrdEquiv] float  NULL,
    [Dra] nvarchar(max)  NULL,
    [LexScore] smallint  NOT NULL,
    [GuideRead] nvarchar(max)  NULL,
    [Isbn13] bigint  NOT NULL,
    [LexCode] nvarchar(max)  NULL,
    [LexUpdate] datetime  NOT NULL
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