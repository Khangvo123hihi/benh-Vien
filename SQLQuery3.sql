CREATE DATABASE BloodLinkDB
GO

USE BloodLinkDB
GO

-- =========================
-- USERS
-- =========================
CREATE TABLE Users
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    FullName NVARCHAR(100) NOT NULL,

    Email NVARCHAR(100) NOT NULL UNIQUE,

    Phone NVARCHAR(20),

    Password NVARCHAR(255) NOT NULL,

    BloodType NVARCHAR(10),

    Age INT,

    Address NVARCHAR(255),

    Role NVARCHAR(20) NOT NULL,

    IsActive BIT DEFAULT 1,

    CreatedAt DATETIME DEFAULT GETDATE()
)
GO

-- =========================
-- BLOOD REQUESTS
-- =========================
CREATE TABLE BloodRequests
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    HospitalId INT NOT NULL,

    BloodType NVARCHAR(10) NOT NULL,

    QuantityNeeded INT NOT NULL,

    Description NVARCHAR(MAX),

    IsUrgent BIT DEFAULT 0,

    CreatedAt DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_BloodRequests_Users
        FOREIGN KEY (HospitalId)
        REFERENCES Users(Id)
)
GO

-- =========================
-- DONATION APPOINTMENTS
-- =========================
CREATE TABLE DonationAppointments
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    UserId INT NOT NULL,

    BloodRequestId INT NOT NULL,

    DonateDate DATETIME NOT NULL,

    Location NVARCHAR(255),

    Status NVARCHAR(50) DEFAULT 'Pending',

    CreatedAt DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_DonationAppointments_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(Id),

    CONSTRAINT FK_DonationAppointments_BloodRequests
        FOREIGN KEY (BloodRequestId)
        REFERENCES BloodRequests(Id)
)
GO

-- =========================
-- DONATION HISTORY
-- =========================
CREATE TABLE DonationHistories
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    UserId INT NOT NULL,

    HospitalName NVARCHAR(255),

    BloodType NVARCHAR(10),

    DonateDate DATETIME NOT NULL,

    Note NVARCHAR(MAX),

    CreatedAt DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_DonationHistories_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(Id)
)
GO

-- =========================
-- ADMIN ACCOUNT
-- =========================
INSERT INTO Users
(
    FullName,
    Email,
    Phone,
    Password,
    Role,
    IsActive
)
VALUES
(
    'System Admin',
    'admin@gmail.com',
    '0123456789',
    '123456',
    'Admin',
    1
)
GO

-- =========================
-- SAMPLE HOSPITAL
-- =========================
INSERT INTO Users
(
    FullName,
    Email,
    Phone,
    Password,
    Address,
    Role,
    IsActive
)
VALUES
(
    N'Bệnh viện Chợ Rẫy',
    'choray@gmail.com',
    '0909999999',
    '123456',
    N'TP Hồ Chí Minh',
    'Hospital',
    1
)
GO

-- =========================
-- SAMPLE USER
-- =========================
INSERT INTO Users
(
    FullName,
    Email,
    Phone,
    Password,
    BloodType,
    Age,
    Address,
    Role,
    IsActive
)
VALUES
(
    N'Nguyễn Văn A',
    'user@gmail.com',
    '0988888888',
    '123456',
    'O+',
    20,
    N'Thủ Đức, TP.HCM',
    'User',
    1
)
GO

-- =========================
-- SAMPLE BLOOD REQUEST
-- =========================
INSERT INTO BloodRequests
(
    HospitalId,
    BloodType,
    QuantityNeeded,
    Description,
    IsUrgent
)
VALUES
(
    2,
    'O+',
    5,
    N'Cần máu gấp cho bệnh nhân cấp cứu',
    1
)
GO

-- =========================
-- SAMPLE APPOINTMENT
-- =========================
INSERT INTO DonationAppointments
(
    UserId,
    BloodRequestId,
    DonateDate,
    Location,
    Status
)
VALUES
(
    3,
    1,
    '2026-05-25 08:00:00',
    N'Bệnh viện Chợ Rẫy',
    'Pending'
)
GO

-- =========================
-- SAMPLE DONATION HISTORY
-- =========================
INSERT INTO DonationHistories
(
    UserId,
    HospitalName,
    BloodType,
    DonateDate,
    Note
)
VALUES
(
    3,
    N'Bệnh viện Chợ Rẫy',
    'O+',
    '2026-04-10',
    N'Hiến máu định kỳ'
)
GO