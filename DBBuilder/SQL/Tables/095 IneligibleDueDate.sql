CREATE TABLE IneligibleDueDate (
	IneligibleDate DATETIME PRIMARY KEY NOT NULL 
);

CREATE INDEX IDX_IneligibleDueDate ON [IneligibleDueDate](IneligibleDate);

INSERT INTO IneligibleDueDate (IneligibleDate) VALUES ('2020-03-09');
INSERT INTO IneligibleDueDate (IneligibleDate) VALUES ('2020-04-10');
INSERT INTO IneligibleDueDate (IneligibleDate) VALUES ('2020-04-11');
INSERT INTO IneligibleDueDate (IneligibleDate) VALUES ('2020-04-13');
INSERT INTO IneligibleDueDate (IneligibleDate) VALUES ('2020-04-25');
INSERT INTO IneligibleDueDate (IneligibleDate) VALUES ('2020-06-08');
INSERT INTO IneligibleDueDate (IneligibleDate) VALUES ('2020-10-05');
INSERT INTO IneligibleDueDate (IneligibleDate) VALUES ('2020-12-25');
INSERT INTO IneligibleDueDate (IneligibleDate) VALUES ('2020-12-28');
INSERT INTO IneligibleDueDate (IneligibleDate) VALUES ('2020-12-31');