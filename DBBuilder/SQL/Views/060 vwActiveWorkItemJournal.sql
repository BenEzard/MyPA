CREATE VIEW vwActiveWorkItemJournal AS
SELECT		Journal_ID, 
			WorkItem_ID, 
			Header,
			[Entry],
			CreationDateTime,
			ModificationDateTime
FROM		WorkItemJournal
WHERE		DeletionDateTime IS NULL;