CREATE VIEW vwMostRecentDueDate AS 
SELECT
    WorkItemDueDate.*
FROM
    WorkItemDueDate
INNER JOIN (
    SELECT
        WorkItem_ID,
        MAX(CreationDateTime) AS MaxCreate
    FROM
        WorkItemDueDate
    WHERE
        DeletionDateTime IS NULL
    GROUP BY
        WorkItem_ID
) maxCreate
ON WorkItemDueDate.WorkItem_ID = maxCreate.WorkItem_ID
AND WorkItemDueDate.CreationDateTime = maxCreate.MaxCreate