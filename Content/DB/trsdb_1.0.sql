CREATE TABLE tbl_profile (
[profile_id] int NOT NULL identity,
[staff_id] nvarchar(10) NOT NULL,
[staff_name] nvarchar(50) NOT NULL,
CONSTRAINT PK_tbl_profile_staff_id PRIMARY KEY ([staff_id]),
)
;
GO

-- Dumping data for table `tbl_profile`

INSERT INTO tbl_profile ([staff_id], [staff_name]) VALUES ('110022', 'Mohamad Azmir Bin Mohamed Sakri')
INSERT INTO tbl_profile ([staff_id], [staff_name]) VALUES ('087876', 'Halim Bin Luqman')
INSERT INTO tbl_profile ([staff_id], [staff_name]) VALUES ('096754', 'Hairi Bin Saad')
INSERT INTO tbl_profile ([staff_id], [staff_name]) VALUES ('089876', 'Rahmah Binti Khalid')
INSERT INTO tbl_profile ([staff_id], [staff_name]) VALUES ('098765', 'Sarah binti Kasim')
;
GO

CREATE TABLE tbl_record (
[record_id] int NOT NULL identity,
[staff_id] nvarchar(10) NOT NULL,
[staff_name] nvarchar(50) NOT NULL,
[temperature] nvarchar(4) NOT NULL,
[datetime_in] datetime2(0) NOT NULL,
CONSTRAINT PK_tbl_record_result_id PRIMARY KEY ([record_id]),
CONSTRAINT FK_tbl_record_staff_id FOREIGN KEY ([staff_id]) REFERENCES tbl_profile ([staff_id])
)
;
GO

-- Dumping data for table `tbl_record`

INSERT INTO tbl_record ([staff_id], [staff_name], [temperature], [datetime_in]) VALUES ('110022', 'Mohamad Azmir Bin Mohamed Sakri', '36.4', '20200511 07:56:03')
INSERT INTO tbl_record ([staff_id], [staff_name], [temperature], [datetime_in]) VALUES ('087876', 'Halim Bin Luqman', '36.7', '20200511 07:57:03')
INSERT INTO tbl_record ([staff_id], [staff_name], [temperature], [datetime_in]) VALUES ('096754', 'Hairi Bin Saad', '36.3', '20200511 07:58:07')
INSERT INTO tbl_record ([staff_id], [staff_name], [temperature], [datetime_in]) VALUES ('089876', 'Rahmah Binti Khalid', '36.9', '20200511 07:59:03')
INSERT INTO tbl_record ([staff_id], [staff_name], [temperature], [datetime_in]) VALUES ('098765', 'Sarah binti Kasim', '36.8', '20200511 07:59:12')
;
GO