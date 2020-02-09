using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPA.Code
{
    public enum AppAction
    {
        CREATING_WORK_ITEM,
        DELETING_WORK_ITEM,
        INSERT_WORK_ITEM,
        APPLICATION_CLOSING, /* Notify models that the application is closing */
    }
}
