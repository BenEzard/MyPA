﻿namespace MyPA.Code
{
    public enum PreferenceName
    {
        START_OF_BUSINESS_DAY,
        END_OF_BUSINESS_DAY,
        DUE_DATE_MINUTE_INCREMENTS,
        DUE_DATE_CAN_BE_MONDAY,
		DUE_DATE_CAN_BE_TUESDAY,
		DUE_DATE_CAN_BE_WEDNESDAY,
		DUE_DATE_CAN_BE_THURSDAY,
		DUE_DATE_CAN_BE_FRIDAY,
		DUE_DATE_CAN_BE_SATURDAY,
		DUE_DATE_CAN_BE_SUNDAY,
        DUE_DATE_BLACKOUT_RANGE,
        DUE_DATE_SET_WINDOW_SECONDS,
        DUE_DATE_REQUIRE_CHANGE_REASON,

        LOGICAL_DELETE,

        APPLICATION_NAME,
        APPLICATION_VERSION,
        DEFAULT_WORKITEM_LENGTH_DAYS,
        DEFAULT_WORKITEM_COB_HOURS,
        DEFAULT_WORKITEM_COB_MINS,
        WORK_ITEM_STATUS_SET_WINDOW_SECONDS,
/*        CONFIRM_JOURNAL_DELETION,*/
        APPLICATION_POSITION_LEFT,
        APPLICATION_POSITION_TOP,
        APPLICATION_WIDTH,
        APPLICATION_HEIGHT,
        SAVE_WINDOW_COORDS_ON_EXIT,
        LOAD_STALE_DAYS, /* Load this number of days-old inactive Work Items */
        STATUS_COMPLETE_TO_ACTIVE,
        USE_OVERDUE_COLOURING,
        SAVE_SESSION_ON_EXIT,
        LAST_SELECTED_WORK_ITEM,
        LAST_SELECTED_WORK_ITEM_TAB,

        JOURNAL_ON_CREATION_GIVE_PROMINENCE, /* Gives the 'Detail' tab prominence (hiding the Overview tab) on WorkItemJournal creation */

        /*JOURNAL_ORDERING,
        DELETE_OPTION,

                DATA_EXPORT_AUTOMATICALLY,
                DATA_EXPORT_COPY_LOCATION,
                DATA_EXPORT_DAYS_STALE,
                DATA_EXPORT_DAYS_STALE_DEFAULT,
                DATA_EXPORT_DUEDATE_SELECTION,
                DATA_EXPORT_INCLUDE_DELETED,
                DATA_EXPORT_INCLUDE_PREFERENCES,
                DATA_EXPORT_LAST_DONE,
                DATA_EXPORT_LAST_DIRECTORY,
                DATA_IMPORT_LAST_DIRECTORY,
                DATA_EXPORT_PERIOD_DAYS,
                DATA_EXPORT_SAVE_TO_LOCATION,
                DATA_EXPORT_SAME_DAY_OVERWRITE,
                DATA_EXPORT_STATUS_SELECTION,
                DATA_EXPORT_WORKITEM_SELECTION,
                DATA_EXPORT_AVAILABLE_VERSIONS,
                DATA_EXPORT_DEFAULT_VERSION,*/
    }
}
