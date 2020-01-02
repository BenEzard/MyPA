CREATE VIEW vwMostRecentWorkItemStatusEntry AS

-- Get a list of all the non-deleted WorkItems.
WITH Base_CTE AS (
    SELECT DISTINCT 
        wise.WorkItem_ID
    FROM 
        WorkItemStatusEntry wise
    WHERE
        DeletionDateTime IS NULL
),

-- Get the most recent status entry per WorkItem, using both the creation and modification dates.
MaxStatusEntry_CTE AS (
    SELECT
        wise.WorkItemStatusEntry_ID,
        wise.WorkItem_ID,
        wise.WorkItemStatus_ID,
        wise.CompletionAmount,
        wise.CreationDateTime,
        wise.ModificationDateTime
    FROM
        WorkItemStatusEntry wise
    -- Get the max modification date for the creation date
    INNER JOIN (
            SELECT
                wise.WorkItem_ID,
                wise.CreationDateTime,
                MAX(IFNULL(wise.ModificationDateTime, CURRENT_DATE)) AS mxModificationDT
            FROM
                WorkItemStatusEntry wise
            -- Get the max creation date of the WorkItem Status Entry
            INNER JOIN (
                SELECT
                    wise.WorkItem_ID,
                    MAX(wise.CreationDateTime) AS mxCreationDT
                FROM
                    WorkItemStatusEntry wise
                WHERE
                    DeletionDateTime IS NULL
                GROUP BY
                    wise.WorkItem_ID
            ) AS subCreationDT
                ON wise.WorkItem_ID = subCreationDT.WorkItem_ID
                AND wise.CreationDateTime = subCreationDT.mxCreationDT
            WHERE
                DeletionDateTime IS NULL
            GROUP BY
                wise.WorkItem_ID
        ) AS subModification
            ON wise.WorkItem_ID = subModification.WorkItem_ID
            AND wise.CreationDateTime = subModification.CreationDateTime    
            AND IFNULL(wise.ModificationDateTime, CURRENT_DATE) = subModification.mxModificationDT
    WHERE
        DeletionDateTime IS NULL
)

SELECT 
    wise.WorkItem_ID,
    wise.WorkItemStatusEntry_ID,
    wise.WorkItemStatus_ID,
    wise.CompletionAmount,
    wise.CreationDateTime,
    wise.ModificationDateTime,
    WorkItemStatus.StatusLabel,
    CASE
       WHEN wise.ModificationDateTime IS NOT NULL THEN wise.ModificationDateTime
       ELSE wise.CreationDateTime
    END AS StatusDateTime,
    CASE
       WHEN WorkItemStatus.IsConsideredActive = 0 THEN 
            CAST ( (julianday('now') - julianday(wise.CreationDateTime)) as int ) + ( (julianday('now') - julianday(wise.CreationDateTime)) > cast ( (julianday('now') - julianday(wise.CreationDateTime)) as int )) 
       ELSE -1
    END AS DaysSinceCompletion,
    WorkItemStatus.IsConsideredActive
FROM
    Base_CTE
LEFT JOIN MaxStatusEntry_CTE wise
    ON Base_CTE.WorkItem_ID = wise.WorkItem_ID
LEFT JOIN WorkItemStatus 
    ON wise.WorkItemStatus_ID = WorkItemStatus.WorkItemStatus_ID