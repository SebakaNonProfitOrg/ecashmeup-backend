-- ══════════════════════════════════════════════════════════════════
--  eCashMeUp — Personal Loan Platform
--  Database Schema v2
--  Updated: CASCADE deletes, fixed inserts, fixed column aliases
-- ══════════════════════════════════════════════════════════════════

DROP DATABASE IF EXISTS ecashmeup;
CREATE DATABASE ecashmeup 
    CHARACTER SET utf8mb4 
    COLLATE utf8mb4_unicode_ci;

USE ecashmeup;

-- ──────────────────────────────────────────────────────────────────
-- 1. LOOKUP / REFERENCE TABLES
-- ──────────────────────────────────────────────────────────────────

CREATE TABLE ref_titles (
    title_id    TINYINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    title_name  VARCHAR(20) NOT NULL UNIQUE
);

INSERT INTO ref_titles (title_name) 
VALUES ('Mr'), ('Mrs'), ('Miss'), ('Ms'), ('Dr'), ('Prof');


CREATE TABLE ref_races (
    race_id    TINYINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    race_name  VARCHAR(50) NOT NULL UNIQUE
);

INSERT INTO ref_races (race_name) 
VALUES ('African'), ('Coloured'), ('Indian/Asian'), ('White'), ('Other');


CREATE TABLE ref_loan_statuses (
    status_id   TINYINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    status_name VARCHAR(30) NOT NULL UNIQUE,
    description VARCHAR(120)
);

INSERT INTO ref_loan_statuses (status_name, description) VALUES
('Pending',      'Application submitted, awaiting review'),
('Under Review', 'Credit assessment in progress'),
('Approved',     'Loan approved, pending disbursement'),
('Declined',     'Application was declined'),
('Disbursed',    'Funds sent to client account'),
('Active',       'Loan active, repayments in progress'),
('Settled',      'Loan fully repaid'),
('Defaulted',    'Client failed to repay');


CREATE TABLE ref_document_types (
    doc_type_id   TINYINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    doc_type_name VARCHAR(60) NOT NULL UNIQUE
);

INSERT INTO ref_document_types (doc_type_name) VALUES
('South African ID'),
('Passport'),
('Payslip'),
('Bank Statement'),
('Proof of Residence');


-- ──────────────────────────────────────────────────────────────────
-- 2. USERS
-- ──────────────────────────────────────────────────────────────────

CREATE TABLE users (
    user_id         INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    title_id        TINYINT UNSIGNED NOT NULL,
    race_id         TINYINT UNSIGNED NOT NULL,
    first_name      VARCHAR(60)  NOT NULL,
    last_name       VARCHAR(60)  NOT NULL,
    id_number       VARCHAR(13)  NULL,
    passport_number VARCHAR(20)  NULL,
    date_of_birth   DATE         NOT NULL,
    phone           VARCHAR(15)  NOT NULL,
    email           VARCHAR(150) NOT NULL,
    password_hash   VARCHAR(255) NOT NULL,
    is_active       TINYINT(1)   NOT NULL DEFAULT 1,
    created_at      DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at      DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP 
                    ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT uq_users_email     UNIQUE (email),
    CONSTRAINT chk_id_or_passport 
        CHECK (id_number IS NOT NULL OR passport_number IS NOT NULL),
    CONSTRAINT fk_users_title 
        FOREIGN KEY (title_id) REFERENCES ref_titles(title_id),
    CONSTRAINT fk_users_race  
        FOREIGN KEY (race_id)  REFERENCES ref_races(race_id)
);


-- ──────────────────────────────────────────────────────────────────
-- 3. EMPLOYMENT DETAILS
-- ──────────────────────────────────────────────────────────────────

CREATE TABLE employment_details (
    employment_id    INT UNSIGNED  AUTO_INCREMENT PRIMARY KEY,
    user_id          INT UNSIGNED  NOT NULL,
    employer_name    VARCHAR(150)  NOT NULL,
    employment_type  ENUM('Permanent','Contract','Self-Employed','Part-Time') NOT NULL,
    job_title        VARCHAR(100)  NOT NULL,
    start_date       DATE          NOT NULL,
    employer_phone   VARCHAR(15)   NULL,
    employer_address VARCHAR(255)  NULL,
    monthly_salary   DECIMAL(12,2) NOT NULL,
    created_at       DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at       DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP 
                     ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT uq_employment_user UNIQUE (user_id),
    CONSTRAINT fk_employment_user 
        FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE
);


-- ──────────────────────────────────────────────────────────────────
-- 4. FINANCIAL DETAILS
-- ──────────────────────────────────────────────────────────────────

CREATE TABLE financial_details (
    financial_id           INT UNSIGNED  AUTO_INCREMENT PRIMARY KEY,
    user_id                INT UNSIGNED  NOT NULL,
    monthly_gross_income   DECIMAL(12,2) NOT NULL,
    monthly_net_income     DECIMAL(12,2) NOT NULL,
    monthly_expenses       DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    other_loan_obligations DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    net_disposable_income  DECIMAL(12,2) 
        GENERATED ALWAYS AS 
        (monthly_net_income - monthly_expenses - other_loan_obligations) 
        STORED,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP 
               ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT uq_financial_user UNIQUE (user_id),
    CONSTRAINT fk_financial_user 
        FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE
);


-- ──────────────────────────────────────────────────────────────────
-- 5. BANKING DETAILS
-- ──────────────────────────────────────────────────────────────────

CREATE TABLE banking_details (
    banking_id     INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    user_id        INT UNSIGNED NOT NULL,
    bank_name      VARCHAR(100) NOT NULL,
    account_holder VARCHAR(120) NOT NULL,
    account_number VARCHAR(20)  NOT NULL,
    branch_code    VARCHAR(10)  NOT NULL,
    account_type   ENUM('Cheque','Savings','Transmission') NOT NULL,
    created_at     DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at     DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP 
                   ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT uq_banking_user UNIQUE (user_id),
    CONSTRAINT fk_banking_user 
        FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE
);


-- ──────────────────────────────────────────────────────────────────
-- 6. LOAN APPLICATIONS
-- ──────────────────────────────────────────────────────────────────

CREATE TABLE loan_applications (
    application_id      INT UNSIGNED     AUTO_INCREMENT PRIMARY KEY,
    user_id             INT UNSIGNED     NOT NULL,
    status_id           TINYINT UNSIGNED NOT NULL DEFAULT 1,
    loan_amount         DECIMAL(10,2)    NOT NULL,
    loan_term_months    TINYINT UNSIGNED NOT NULL,
    interest_rate       DECIMAL(5,2)     NOT NULL DEFAULT 15.00,
    monthly_installment DECIMAL(10,2)    NOT NULL,
    total_repayable     DECIMAL(10,2)    NOT NULL,
    total_interest      DECIMAL(10,2)    NOT NULL,
    loan_purpose        VARCHAR(200)     NULL,
    applied_at          DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at          DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP 
                        ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT chk_loan_amount 
        CHECK (loan_amount BETWEEN 100.00 AND 3000.00),
    CONSTRAINT chk_loan_term   
        CHECK (loan_term_months BETWEEN 1 AND 3),
    CONSTRAINT fk_loan_user   
        FOREIGN KEY (user_id)   REFERENCES users(user_id) ON DELETE CASCADE,
    CONSTRAINT fk_loan_status 
        FOREIGN KEY (status_id) REFERENCES ref_loan_statuses(status_id)
);


-- ──────────────────────────────────────────────────────────────────
-- 7. CREDIT ASSESSMENTS
-- ──────────────────────────────────────────────────────────────────

CREATE TABLE credit_assessments (
    assessment_id        INT UNSIGNED  AUTO_INCREMENT PRIMARY KEY,
    application_id       INT UNSIGNED  NOT NULL,
    assessment_result    ENUM('Approved','Declined','Manual Review') NOT NULL,
    credit_score         SMALLINT UNSIGNED NULL,
    debt_to_income_ratio DECIMAL(5,2)  NULL,
    affordability_amount DECIMAL(10,2) NULL,
    decline_reason       VARCHAR(255)  NULL,
    assessed_by          VARCHAR(100)  NULL COMMENT 'Officer name or System',
    assessed_at          DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT uq_assessment_application UNIQUE (application_id),
    CONSTRAINT fk_assessment_application 
        FOREIGN KEY (application_id) 
        REFERENCES loan_applications(application_id) ON DELETE CASCADE
);


-- ──────────────────────────────────────────────────────────────────
-- 8. LOAN DISBURSEMENTS
-- ──────────────────────────────────────────────────────────────────

CREATE TABLE loan_disbursements (
    disbursement_id     INT UNSIGNED  AUTO_INCREMENT PRIMARY KEY,
    application_id      INT UNSIGNED  NOT NULL,
    disbursed_amount    DECIMAL(10,2) NOT NULL,
    disbursement_method ENUM('EFT','Cash','Mobile') NOT NULL DEFAULT 'EFT',
    reference_number    VARCHAR(60)   NOT NULL,
    disbursed_at        DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT uq_disbursement_application UNIQUE (application_id),
    CONSTRAINT uq_disbursement_reference   UNIQUE (reference_number),
    CONSTRAINT fk_disbursement_application 
        FOREIGN KEY (application_id) 
        REFERENCES loan_applications(application_id) ON DELETE CASCADE
);


-- ──────────────────────────────────────────────────────────────────
-- 9. REPAYMENT SCHEDULES
-- ──────────────────────────────────────────────────────────────────

CREATE TABLE repayment_schedules (
    schedule_id         INT UNSIGNED     AUTO_INCREMENT PRIMARY KEY,
    application_id      INT UNSIGNED     NOT NULL,
    installment_number  TINYINT UNSIGNED NOT NULL COMMENT '1, 2, or 3',
    due_date            DATE          NOT NULL,
    amount_due          DECIMAL(10,2) NOT NULL,
    outstanding_balance DECIMAL(10,2) NOT NULL,
    is_paid             TINYINT(1)    NOT NULL DEFAULT 0,
    paid_at             DATETIME      NULL,

    CONSTRAINT uq_schedule UNIQUE (application_id, installment_number),
    CONSTRAINT fk_schedule_application 
        FOREIGN KEY (application_id) 
        REFERENCES loan_applications(application_id) ON DELETE CASCADE
);


-- ──────────────────────────────────────────────────────────────────
-- 10. REPAYMENT PAYMENTS
-- ──────────────────────────────────────────────────────────────────

CREATE TABLE repayment_payments (
    payment_id            INT UNSIGNED  AUTO_INCREMENT PRIMARY KEY,
    application_id        INT UNSIGNED  NOT NULL,
    schedule_id           INT UNSIGNED  NOT NULL,
    amount_paid           DECIMAL(10,2) NOT NULL,
    payment_method        ENUM('EFT','DebiCheck','Cash','Mobile') NOT NULL,
    transaction_reference VARCHAR(80)   NOT NULL,
    payment_date          DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT uq_transaction_ref UNIQUE (transaction_reference),
    CONSTRAINT fk_payment_application 
        FOREIGN KEY (application_id) 
        REFERENCES loan_applications(application_id) ON DELETE CASCADE,
    CONSTRAINT fk_payment_schedule   
        FOREIGN KEY (schedule_id) 
        REFERENCES repayment_schedules(schedule_id) ON DELETE CASCADE
);


-- ──────────────────────────────────────────────────────────────────
-- 11. DOCUMENTS
-- ──────────────────────────────────────────────────────────────────

CREATE TABLE documents (
    document_id    INT UNSIGNED     AUTO_INCREMENT PRIMARY KEY,
    user_id        INT UNSIGNED     NOT NULL,
    application_id INT UNSIGNED     NULL,
    doc_type_id    TINYINT UNSIGNED NOT NULL,
    file_name      VARCHAR(200) NOT NULL,
    file_path      VARCHAR(500) NOT NULL,
    file_size_kb   INT UNSIGNED NULL,
    mime_type      VARCHAR(100) NULL,
    is_verified    TINYINT(1)   NOT NULL DEFAULT 0,
    uploaded_at    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_document_user 
        FOREIGN KEY (user_id) 
        REFERENCES users(user_id) ON DELETE CASCADE,
    CONSTRAINT fk_document_application 
        FOREIGN KEY (application_id) 
        REFERENCES loan_applications(application_id) ON DELETE CASCADE,
    CONSTRAINT fk_document_type 
        FOREIGN KEY (doc_type_id) 
        REFERENCES ref_document_types(doc_type_id)
);


-- ──────────────────────────────────────────────────────────────────
-- VERIFY
-- ──────────────────────────────────────────────────────────────────

SELECT 'ref_titles'        AS table_name, COUNT(*) AS row_count FROM ref_titles
UNION ALL
SELECT 'ref_races',          COUNT(*) FROM ref_races
UNION ALL
SELECT 'ref_loan_statuses',  COUNT(*) FROM ref_loan_statuses
UNION ALL
SELECT 'ref_document_types', COUNT(*) FROM ref_document_types
UNION ALL
SELECT 'users',              COUNT(*) FROM users
UNION ALL
SELECT 'employment_details', COUNT(*) FROM employment_details
UNION ALL
SELECT 'financial_details',  COUNT(*) FROM financial_details
UNION ALL
SELECT 'banking_details',    COUNT(*) FROM banking_details
UNION ALL
SELECT 'loan_applications',  COUNT(*) FROM loan_applications
UNION ALL
SELECT 'credit_assessments', COUNT(*) FROM credit_assessments
UNION ALL
SELECT 'loan_disbursements', COUNT(*) FROM loan_disbursements
UNION ALL
SELECT 'repayment_schedules',COUNT(*) FROM repayment_schedules
UNION ALL
SELECT 'repayment_payments', COUNT(*) FROM repayment_payments
UNION ALL
SELECT 'documents',          COUNT(*) FROM documents;