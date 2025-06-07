CREATE TABLE [dbo].[Events] (
    [EventId]   INT            IDENTITY (1, 1) NOT NULL,
    [Location]  NVARCHAR (100) NOT NULL,
    [EventDate] DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED ([EventId] ASC)
);

