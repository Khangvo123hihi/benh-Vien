USE BloodLinkDB;
GO

-- ==========================================================
-- BƯỚC 1: XÓA DỮ LIỆU Ở CÁC BẢNG CON TRƯỚC (NẾU CÓ)
-- Để tránh bị lỗi khóa ngoại (Foreign Key), ta xóa dữ liệu liên quan đến Id 1 và 2 trước
-- ==========================================================

-- 1. Xóa lịch sử hiến máu liên quan đến 2 bệnh viện này
DELETE FROM [dbo].[Hospitals] WHERE HospitalId IN (1, 2);
GO