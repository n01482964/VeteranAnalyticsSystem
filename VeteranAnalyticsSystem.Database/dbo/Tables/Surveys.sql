CREATE TABLE [dbo].[Surveys] (
    [SurveyId]       INT            IDENTITY (1, 1) NOT NULL,
    [Email]          NVARCHAR (256) NOT NULL,
    [EventId]        INT            NOT NULL,
    [SurveyType]     INT            NOT NULL,
    [SubmissionDate] DATETIME2 (7)  NOT NULL,
    [Responses]      NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Surveys] PRIMARY KEY CLUSTERED ([SurveyId] ASC)
);

