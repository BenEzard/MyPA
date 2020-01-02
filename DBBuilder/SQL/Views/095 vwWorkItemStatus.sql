CREATE VIEW vwWorkItemStatus AS
SELECT 
    WorkItemStatus_ID,
    StatusLabel,
    IsConsideredActive,
    IsDefault,
    DeletionDateTime
FROM
    WorkItemStatus
ORDER BY
    DisplayOrder ASC
