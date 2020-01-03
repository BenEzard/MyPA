 CREATE TABLE WorkItem ( 
    WorkItem_ID INTEGER PRIMARY KEY AUTOINCREMENT,
    TaskTitle VARCHAR(50) NOT NULL,
	TaskDescription VARCHAR(8000) NULL,
    CreationDateTime DATETIME NOT NULL DEFAULT(datetime('now','localtime')),
    ModificationDateTime DATETIME NULL,
	DeletionDateTime DATETIME NULL
);



