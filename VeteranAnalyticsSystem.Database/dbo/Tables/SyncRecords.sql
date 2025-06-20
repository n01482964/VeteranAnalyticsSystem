CREATE TABLE [dbo].[SyncRecords]
(
	[Id]			INT NOT NULL IDENTITY(1,1),
	[TimeStamp]		DATETIME NOT NULL,
	[SyncType]		INT NOT NULL, 
    CONSTRAINT [PK_SyncRecord] PRIMARY KEY ([Id]),
)
