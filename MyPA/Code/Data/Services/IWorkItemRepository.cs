using MyPA.Code.Data.Models;
using System;
using System.Collections.Generic;

namespace MyPA.Code.Data.Services
{
    public interface IWorkItemRepository
    {
        // Get WorkItem Preferences
        Dictionary<PreferenceName, Preference> GetWorkItemPreferences();

        /// <summary>
        /// Get a list of all of the Work Items.
        /// </summary>
        /// <returns></returns>
        List<WorkItem> GetWorkItems(int loadAgedDays);

        /// <summary>
        /// Insert a WorkItem into the database.
        /// </summary>
        /// <param name="workItem"></param>
        /// <returns></returns>
        int InsertWorkItem(WorkItem workItem);

        /// <summary>
        /// Update a WorkItem in the database.
        /// </summary>
        /// <param name="workItem"></param>
        /// <returns></returns>
        void UpdateWorkItem(WorkItem workItem);

        /// <summary>
        /// Get the list of all WorkItemStatus(es), including deleted.
        /// </summary>
        /// <returns></returns>
        List<WorkItemStatus> GetWorkItemStatuses();

        int InsertWorkItemStatusEntry(BaseWorkItemStatusEntry wise);

        void UpdateWorkItemStatusEntry(BaseWorkItemStatusEntry wise);

        int InsertWorkItemDueDate(WorkItemDueDate widd);

        void UpdateWorkItemDueDate(WorkItemDueDate widd);

        List<DateTime> GetIneligibleDueDates();

        /// <summary>
        /// Delete a WorkItem.
        /// </summary>
        /// <param name="workItemID"></param>
        /// <param name="logicalDelete"></param>
        void DeleteWorkItem(int workItemID, bool logicalDelete);

        /// <summary>
        /// Delete a WorkItemStatusEntry.
        /// </summary>
        /// <param name="workItemID"></param>
        /// <param name="logicalDelete"></param>
        void DeleteWorkItemStatusEntry(int workItemID, bool logicalDelete);

        /// <summary>
        /// Delete a WorkItemDueDate.
        /// </summary>
        /// <param name="workItemID"></param>
        /// <param name="logicalDelete"></param>
        void DeleteWorkItemDueDate(int workItemID, bool logicalDelete);
    }
}
