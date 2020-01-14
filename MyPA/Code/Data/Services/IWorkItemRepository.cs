using MyPA.Code.Data.Models;
using System.Collections.Generic;

namespace MyPA.Code.Data.Services
{
    public interface IWorkItemRepository
    {
        /// <summary>
        /// Get a list of all of the Work Items.
        /// </summary>
        /// <returns></returns>
        List<WorkItem> GetWorkItems(int loadAgedDays);

        /// <summary>
        /// Get the list of all WorkItemStatus(es), including deleted.
        /// </summary>
        /// <returns></returns>
        List<WorkItemStatus> GetWorkItemStatuses();

        int InsertWorkItemStatusEntry(WorkItemStatusEntry wise);
    }
}
