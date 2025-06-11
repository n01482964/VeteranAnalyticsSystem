CREATE TABLE [dbo].[Veterans] (
    [VeteranId]             NVARCHAR (64)  NOT NULL,
    [FirstName]             NVARCHAR (100) NOT NULL,
    [LastName]              NVARCHAR (100) NOT NULL,
    [Email]                 NVARCHAR (MAX) NOT NULL,
    [PhoneNumber]           NVARCHAR (MAX) NOT NULL,
    [HomeAddress]           NVARCHAR (200) NOT NULL,
    [City]                  NVARCHAR (100) NOT NULL,
    [State]                 NVARCHAR (50)  NOT NULL,
    [RelationshipStatus]    NVARCHAR (MAX) NOT NULL,
    [Gender]                NVARCHAR (20)  NOT NULL,
    [DateOfBirth]           DATETIME2 (7)  NOT NULL,
    [MilitaryServiceStatus] NVARCHAR (MAX) NOT NULL,
    [HighestRank]           NVARCHAR (MAX) NOT NULL,
    [StartOfService]        DATETIME2 (7)  NOT NULL,
    [EndOfService]          DATETIME2 (7)  NOT NULL,
    [BranchOfService]       NVARCHAR (MAX) NOT NULL,
    [NumberOfDeployments]   NVARCHAR (MAX) NOT NULL,
    [DeploymentDetails]     NVARCHAR (MAX) NOT NULL,
    [HealthConcerns]        NVARCHAR (MAX) NOT NULL,
    [AdditionalHealthInfo]  NVARCHAR (MAX) NOT NULL,
    [PhysicalLimitations]   NVARCHAR (MAX) NOT NULL,
    [EventId]               INT            NOT NULL,
    [StarRating]            INT            NULL,
    CONSTRAINT [PK_Veterans] PRIMARY KEY CLUSTERED ([VeteranId] ASC),
    CONSTRAINT [FK_Veterans_Events_EventId] FOREIGN KEY ([EventId]) REFERENCES [dbo].[Events] ([EventId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Veterans_EventId]
    ON [dbo].[Veterans]([EventId] ASC);

