CREATE VIEW vwActiveWorkItemStatus AS
SELECT 
    WorkItemStatus_ID
    StatusLabel,
    IsConsideredActive,
    IsDefault
FROM
    WorkItemStatus
WHERE
    DeletionDateTime IS NULL
ORDER BY
    DisplayOrder ASC
