CREATE TABLE WorkItemStatusEntry (
    WorkItemStatusEntry_ID  INTEGER  PRIMARY KEY AUTOINCREMENT,
    WorkItem_ID             INTEGER  REFERENCES WorkItem (WorkItem_ID),
    WorkItemStatus_ID       INTEGER  REFERENCES [WorkItemStatus] (WorkItemStatus_ID),
	CompletionAmount        INTEGER NOT NULL DEFAULT 0, 
    CreationDateTime        DATETIME NOT NULL DEFAULT(CURRENT_TIMESTAMP),
	ModificationDateTime    DATETIME NULL,
	DeletionDateTime        DATETIME NULL 
);

