CREATE TABLE Preference ( 
    [Name] VARCHAR (30)  NOT NULL ,
    [Model] VARCHAR (50) NOT NULL,
    [Value] VARCHAR(500) NOT NULL,
    [DefaultValue] VARCHAR(500),
    [Description] VARCHAR(500) NOT NULL,
    [UserCanEdit] CHAR(1) DEFAULT ('N'),
	PRIMARY KEY (Name, Model)
 );

INSERT INTO Preference ([Name], [Model], [Value], DefaultValue, [Description], UserCanEdit) 
VALUES	/* Application Values - Non configurable */
		('APPLICATION_NAME', 'Application', 'My PA', 'My PA', 'The name of the Application', 'N'),
		('APPLICATION_VERSION', 'Application', '0.3.1', '0.3.1', 'The version number of the Application', 'N'),

		/* Application Values - Configurable */
		('APPLICATION_POSITION_LEFT', 'Application', '100', '100', 'The window''s location (left)', 'Y'),
		('APPLICATION_POSITION_TOP','Application',  '0', '0', 'The window''s location (top)', 'Y'),
		('APPLICATION_WIDTH', 'Application', '600', '600', 'The window''s size (width)', 'Y'),
		('APPLICATION_HEIGHT', 'Application', '750', '750', 'The window''s size (height)', 'Y'),
		('SAVE_WINDOW_COORDS_ON_EXIT', 'Application', '1', '1', 'Should the window''s location and size be saved when the application exits?', 'Y'),

		/* Due Date variables */
		('LOAD_STALE_DAYS', 'WorkItem', '100', '100', 'Load Work Items that were completed this many days ago.', 'Y'),
		('DEFAULT_WORKITEM_LENGTH_DAYS', 'WorkItem', '35', '5', 'The default number of days that should be added to complete a WorkItem.', 'Y'),
		('DEFAULT_WORKITEM_COB_HOURS', 'WorkItem', '16', '16', 'The default Due Date Close of Business (COB) Hours (24hr clock).', 'Y'),
		('DEFAULT_WORKITEM_COB_MINS', 'WorkItem', '0', '0', 'The default Due Date Close of Business (COB) Minutes.', 'Y'),
		('DUE_DATE_SET_WINDOW_MINUTES', 'WorkItem', '10', '10', 'If the Due Date is altered within this time period of setting it, don''t record it as a change.', 'Y'),
		('DUE_DATE_CAN_BE_WEEKENDS', 'WorkItem', '0', '0', 'Should a Due Date on a Saturday or Sunday be considered legitimate?', 'Y'),

		/* Work Item Status */
		('STATUS_COMPLETE_TO_ACTIVE', 'WorkItem', '75', '75', 'When a WorkItem is moved from complete-to-active, set the Completion percent.', 'Y'),

		/* Journals */
		('CONFIRM_JOURNAL_DELETION', 'WorkItemJournal', '1', '1', 'Should a Journal Entry deletion be confirmed?', 'Y'),
		('JOURNAL_ORDERING', 'WorkItemJournal', 'bottom', 'bottom', 'Should new Journal Entries appear at bottom or top of list?', 'Y'),

		('DELETE_OPTION', 'WorkItemJournal', 'logically (leave trace)', 'logically (leave trace)', 'Should deletion be logical or physical. Options logically (leave trace), physically (permanent)', 'Y'),

		/* Backup options */
		('DATA_EXPORT_LAST_DONE', 'Application', '2019-09-13', '2019-09-13', 'When the backup was last done (a date)', 'N'),
		('DATA_EXPORT_LAST_DIRECTORY', 'Application', 'C:\', '', 'The directory where the last export file was chosen from', 'N'),
		('DATA_IMPORT_LAST_DIRECTORY', 'Application', 'C:\', '', 'The directory where the last import file was chosen from', 'N'),
		('DATA_EXPORT_AUTOMATICALLY', 'Application', '0', '1', 'Should a backup be done automatically? (1 or 0)', 'Y'),
		('DATA_EXPORT_PERIOD_DAYS', 'Application', '1', '1', 'How often should the backup be done (in days)?', 'Y'),
		('DATA_EXPORT_WORKITEM_SELECTION', 'Application', 'all', 'all', 'Which WorkItems should be backed up? Options are all, active only and active plus closed', 'Y'),
		('DATA_EXPORT_DAYS_STALE', 'Application', '100', '100', 'When exporting Closed Work Items, take this many days?', 'Y'),
		('DATA_EXPORT_DAYS_STALE_DEFAULT', 'Application', '9999', '9999', 'If no value is selected for DATA_EXPORT_DAYS_STALE, what should it be defaulted to?', 'Y'),
		('DATA_EXPORT_DUEDATE_SELECTION', 'Application', 'full', 'full', 'What Due Date info should be backed up? Options are "full" and "latest"', 'Y'),
		('DATA_EXPORT_STATUS_SELECTION', 'Application', 'full', 'full', 'What Status info should be backed up? Options are "full" and "latest"', 'Y'),
		('DATA_EXPORT_INCLUDE_DELETED', 'Application', '0', '0', 'Should deleted Work Items be included in the backup? Options are 0 or 1', 'Y'),
		('DATA_EXPORT_INCLUDE_PREFERENCES', 'Application', '0', '0', 'Should Preferences be included in the backup? Options are 0 or 1', 'Y'),
		('DATA_EXPORT_SAVE_TO_LOCATION', 'Application', 'D:\Work\MyWorkTracker Backup', 'D:\Work\MyWorkTracker Backup', 'Location where backup files should be placed.', 'Y'),
		('DATA_EXPORT_SAME_DAY_OVERWRITE', 'Application', '1', '1', 'Overwrite the backup if from the same day. Options are 0 or 1', 'Y'),
		('DATA_EXPORT_COPY_LOCATION', 'Application', 'D:\Work\MyWorkTracker Copy', 'D:\Work\MyWorkTracker Copy', 'Location where backup files should be copied (duplicated) to.', 'Y'),
		('DATA_EXPORT_AVAILABLE_VERSIONS', 'Application', '0.3.1,0.3.0', '0.3.1,0.3.0', 'Available Export options (comma separated)', 'N'),
		('DATA_EXPORT_DEFAULT_VERSION', 'Application', '0.3.1', '0.3.1', 'Default Export version', 'Y')
		;