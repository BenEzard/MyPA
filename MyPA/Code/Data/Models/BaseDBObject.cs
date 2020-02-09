using System;

namespace MyPA.Code.Data.Models
{
    public class BaseDBObject
    {
        /// <summary>
        /// Flag if the database object needs any operations (insert/update/delete or none).
        /// </summary>
        public DBActionRequired PendingDBOperation { get; set; }

        public bool IsDBActionRequired
        {
            get
            {
                if (PendingDBOperation == DBActionRequired.NONE)
                    return false;
                else
                    return true;
            }
        }

        public DateTime? CreationDateTime { get; set; }
        public DateTime? ModificationDateTime { get; set; }
        public DateTime? DeletionDateTime { get; set; }

    }
}
