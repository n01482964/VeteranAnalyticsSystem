CREATE TABLE [dbo].[Surveys] (
    [Id]                    INT            IDENTITY (1, 1) NOT NULL,
    [EventId]               INT            NOT NULL,
    [SurveyType]            INT            NOT NULL,
    [SubmissionDate]        DATETIME2 (7)  NULL,
    [Email]                 NVARCHAR (256) NULL,
    [SelfIdentifier]        NVARCHAR(MAX) NULL,
    [EmotionalConnection]   NVARCHAR(MAX) NULL,
    [ConflictResolution]    NVARCHAR(MAX) NULL,
    [PastStruggles]         NVARCHAR(MAX) NULL,
    [ComfortZone]           NVARCHAR(MAX) NULL,
    [Rating]                NVARCHAR(MAX) NULL,
    [ExperienceRating]      NVARCHAR(MAX) NULL,
    [LifeImpact]            NVARCHAR(MAX) NULL,
    [Recommendation]        NVARCHAR(MAX) NULL,
    [Feedback]              NVARCHAR(MAX) NULL,
    [ResponsesJson]         NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Surveys] PRIMARY KEY CLUSTERED ([Id] ASC)
);

