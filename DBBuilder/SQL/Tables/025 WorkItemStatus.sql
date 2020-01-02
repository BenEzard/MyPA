CREATE TABLE [WorkItemStatus] (
    WorkItemStatus_ID   INTEGER PRIMARY KEY AUTOINCREMENT,
    StatusLabel VARCHAR (20) NOT NULL,
	IsConsideredActive BIT NOT NULL,
	IsDefault BIT NOT NULL,
	DisplayOrder INT NOT NULL,
	DeletionDateTime DATETIME NULL,
	UNIQUE(IsConsideredActive, IsDefault, DeletionDateTime) ON CONFLICT FAIL
);

CREATE INDEX IDX_Status ON [WorkItemStatus](StatusLabel);

INSERT INTO [WorkItemStatus] (StatusLabel, IsConsideredActive, IsDefault, DisplayOrder) 
VALUES		('Active', 1, 1, 10),
			('Awaiting Feedback', 1, 0, 20),
			('Completed', 0, 1, 30),
			('Cancelled', 0, 0, 40);