CREATE TABLE [dbo].[Volunteers] (
    [VolunteerId] INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]   NVARCHAR (MAX) NOT NULL,
    [LastName]    NVARCHAR (MAX) NOT NULL,
    [Email]       NVARCHAR (MAX) NOT NULL,
    [PhoneNumber] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Volunteers] PRIMARY KEY CLUSTERED ([VolunteerId] ASC)
);

