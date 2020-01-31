CREATE TABLE WorkItemDueDate ( 
    WorkItemDueDate_ID  INTEGER PRIMARY KEY AUTOINCREMENT, 
    WorkItem_ID         INTEGER       REFERENCES WorkItems (WorkItem_ID), 
    DueDateTime         DATETIME      NOT NULL,
    ChangeReason        VARCHAR (500) NULL,
    CreationDateTime    DATETIME NOT NULL DEFAULT(CURRENT_TIMESTAMP),
    ModificationDateTime    DATETIME NULL, 
	DeletionDateTime    DATETIME NULL 
);

CREATE INDEX IDX_WorkItemDueDate ON [WorkItemDueDate](WorkItem_ID);