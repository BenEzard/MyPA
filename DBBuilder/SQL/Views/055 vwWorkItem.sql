CREATE VIEW vwWorkItem AS 
SELECT 
	WorkItem.WorkItem_ID, 
	WorkItem.TaskTitle, 
	WorkItem.CreationDateTime,
	WorkItem.ModificationDateTime,
	WorkItem.DeletionDateTime,
	WorkItem.TaskDescription, 
	DueDate.WorkItemDueDate_ID,
	DueDate.DueDateTime,
	DueDate.CreationDateTime AS DueDateCreationDateTime,
	DueDate.ModificationDateTime AS DueDateModificationDateTime,
	vwMostRecentWorkItemStatusEntry.WorkItemStatusEntry_ID,
	vwMostRecentWorkItemStatusEntry.WorkItemStatus_ID AS WorkItemStatus_ID,
	vwMostRecentWorkItemStatusEntry.StatusLabel AS wisStatusLabel,
	vwMostRecentWorkItemStatusEntry.StatusDateTime AS wisStatusDateTime,
	vwMostRecentWorkItemStatusEntry.IsConsideredActive AS wisIsConsideredActive,
	vwMostRecentWorkItemStatusEntry.DaysSinceCompletion,
	vwMostRecentWorkItemStatusEntry.CompletionAmount,
	vwMostRecentWorkItemStatusEntry.CreationDateTime AS wisCreationDateTime,
	vwMostRecentWorkItemStatusEntry.ModificationDateTime AS wisModificationDateTime
FROM WorkItem
LEFT JOIN vwMostRecentWorkItemStatusEntry 
	ON WorkItem.WorkItem_ID = vwMostRecentWorkItemStatusEntry.WorkItem_ID
LEFT JOIN vwMostRecentDueDate AS DueDate
	ON WorkItem.WorkItem_ID = DueDate.WorkItem_ID
WHERE 
	WorkItem.DeletionDateTime IS NULL