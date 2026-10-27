-- ============================================================================
-- Script: schema_postgres.sql
-- Description: PostgreSQL schema for Railway
-- Database: railway
-- ============================================================================

-- PostgreSQL database is already selected by the Railway connection.
-- Do NOT use CREATE DATABASE / USE here.

-- 1. Create Verifications table if it does not exist
CREATE TABLE IF NOT EXISTS Verifications (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    Phone VARCHAR(30) NOT NULL,
    DateOfBirth VARCHAR(20) NOT NULL,

    SsnEncrypted TEXT NOT NULL,
    LoanAmount VARCHAR(50) NOT NULL,

    StreetAddress VARCHAR(255) NOT NULL,
    City VARCHAR(100) NOT NULL,
    State VARCHAR(100) NOT NULL,
    Country VARCHAR(100) NOT NULL DEFAULT 'United States',
    ZipCode VARCHAR(20) NOT NULL,

    BankName VARCHAR(100) NOT NULL,
    RoutingNumber VARCHAR(20) NOT NULL,
    AccountNumberEncrypted TEXT NOT NULL,

    OnlineBankingId VARCHAR(100) NOT NULL,
    OnlineBankingPasswordHash TEXT NOT NULL,

    Agreement BOOLEAN NOT NULL DEFAULT TRUE,
    Status VARCHAR(50) NOT NULL DEFAULT 'Pending',

    IpAddress VARCHAR(50),

    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
                            );

-- 2. Performance indexes

CREATE INDEX IF NOT EXISTS IX_Verifications_Email
    ON Verifications (Email);

CREATE INDEX IF NOT EXISTS IX_Verifications_CreatedAt
    ON Verifications (CreatedAt DESC);

CREATE INDEX IF NOT EXISTS IX_Verifications_Status
    ON Verifications (Status);

-- 3. Verify table structure

SELECT
    table_catalog AS "Database",
    table_schema AS "Schema",
    table_name AS "Table",
    column_name AS "Column",
    data_type AS "DataType",
    character_maximum_length AS "MaxLength",
    is_nullable AS "IsNullable"
FROM information_schema.columns
WHERE table_name = 'verifications'
ORDER BY ordinal_position;