CREATE VIEW vwWorkItem AS 
SELECT 
	WorkItem.WorkItem_ID, 
	WorkItem.TaskTitle, 
	WorkItem.CreationDateTime,
	WorkItem.ModificationDateTime,
	WorkItem.DeletionDateTime,
	WorkItem.TaskDescription, 
	DueDate.DueDateTime,
	DueDate.DueDateCreationDateTime,
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
LEFT JOIN ( 
    SELECT 
		WorkItemDueDate.WorkItemDueDate_ID, 
		WorkItemDueDate.DueDateTime,
		WorkItemDueDate.CreationDateTime AS DueDateCreationDateTime,
		WorkItemDueDate.ChangeReason, 
		WorkItemDueDate.WorkItem_ID 
    FROM WorkItemDueDate 
    INNER JOIN ( 
        SELECT WorkItemDueDate.WorkItem_ID, 
			MAX(WorkItemDueDate.CreationDateTime) AS mxCreateDate 
        FROM WorkItemDueDate 
        GROUP BY WorkItemDueDate.WorkItem_ID 
    ) AS mxDueDate 
	ON mxDueDate.WorkItem_ID = WorkItemDueDate.WorkItem_ID AND 
		mxDueDate.mxCreateDate = WorkItemDueDate.CreationDateTime 
) AS DueDate 
	ON DueDate.WorkItem_ID = WorkItem.WorkItem_ID
WHERE WorkItem.DeletionDateTime IS NULL