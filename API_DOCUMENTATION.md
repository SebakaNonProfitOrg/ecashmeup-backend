# eCashMeUp API

A RESTful backend API for a personal loan platform built with ASP.NET Core Web API (.NET 9), MySQL, and JWT Authentication.

---

## Tech Stack

- **Backend:** ASP.NET Core Web API (.NET 9)
- **Database:** MySQL
- **Authentication:** JWT Bearer Tokens
- **Documentation:** Swagger UI
- **ORM:** Entity Framework Core (Pomelo MySQL)
- **Password Hashing:** BCrypt

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [MySQL Server](https://dev.mysql.com/downloads/installer/)
- [MySQL Workbench](https://dev.mysql.com/downloads/workbench/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

---

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/eCashMeUp.git
cd eCashMeUp
```

---

### 2. Set Up the Database

1. Open **MySQL Workbench**
2. Open a new SQL tab
3. Paste and run the full SQL script from `/database/ecashmeup.sql`
4. Confirm all 14 tables are created

---

### 3. Configure Connection String

Open `appsettings.json` and update your MySQL credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=ecashmeup;Uid=root;Pwd=YourPassword;"
  },
  "Jwt": {
    "Key": "eCashMeUpSuperSecretKey2024!MustBe32Chars",
    "Issuer": "eCashMeUp",
    "Audience": "eCashMeUpUsers"
  }
}
```

---

### 4. Run the Project

```bash
dotnet run
```

Or press **F5** in Visual Studio.

Swagger UI will be available at:

```
https://localhost:7179/swagger
```

---

## Authentication

This API uses **JWT Bearer Token** authentication.

### How to Authenticate

**Step 1** — Register or Login to get a token:

```
POST /api/auth/register
POST /api/auth/login
```

**Step 2** — Copy the token from the response.

**Step 3** — In Swagger UI, click the 🔒 **Authorize** button and paste just the token:

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Step 4** — All protected endpoints are now unlocked.

> For tools like Postman, add the token to the request header:
> `Authorization: Bearer {your_token}`

---

## API Endpoints

### Auth

| Method | Endpoint             | Description                 | Auth Required |
| ------ | -------------------- | --------------------------- | ------------- |
| POST   | `/api/auth/register` | Register a new user         | ❌            |
| POST   | `/api/auth/login`    | Login and receive JWT token | ❌            |

---

### Loan

| Method | Endpoint              | Description                      | Auth Required |
| ------ | --------------------- | -------------------------------- | ------------- |
| POST   | `/api/loan/calculate` | Calculate loan repayments        | ❌            |
| POST   | `/api/loan/apply`     | Submit a loan application        | ✅            |
| GET    | `/api/loan/{id}`      | Get a loan by ID                 | ✅            |
| GET    | `/api/loan/my-loans`  | Get all loans for logged-in user | ✅            |

---

### Repayments

| Method | Endpoint                   | Description                       | Auth Required |
| ------ | -------------------------- | --------------------------------- | ------------- |
| GET    | `/api/repayments/{loanId}` | Get repayment schedule for a loan | ✅            |

---

### Admin

| Method | Endpoint                                | Description                     | Auth Required |
| ------ | --------------------------------------- | ------------------------------- | ------------- |
| GET    | `/api/admin/applications`               | Get all loan applications       | ✅            |
| GET    | `/api/admin/applications/{id}`          | Get full application details    | ✅            |
| PUT    | `/api/admin/applications/{id}/review`   | Set application to Under Review | ✅            |
| PUT    | `/api/admin/applications/{id}/approve`  | Approve a loan application      | ✅            |
| PUT    | `/api/admin/applications/{id}/decline`  | Decline a loan application      | ✅            |
| PUT    | `/api/admin/applications/{id}/disburse` | Disburse an approved loan       | ✅            |

---

## Request & Response Examples

### Register

**POST** `/api/auth/register`

Request:

```json
{
  "titleId": 1,
  "raceId": 1,
  "firstName": "Thabo",
  "lastName": "Nkosi",
  "email": "thabo@test.com",
  "phone": "0712345678",
  "password": "Test@1234",
  "dateOfBirth": "1990-01-01",
  "idNumber": "9001015009087"
}
```

Response `200 OK`:

```json
{
  "message": "Registration successful.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

### Login

**POST** `/api/auth/login`

Request:

```json
{
  "email": "thabo@test.com",
  "password": "Test@1234"
}
```

Response `200 OK`:

```json
{
  "message": "Login successful.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

### Calculate Loan

**POST** `/api/loan/calculate`

Request:

```json
{
  "loanAmount": 1500,
  "termMonths": 3
}
```

Response `200 OK`:

```json
{
  "loanAmount": 1500,
  "termMonths": 3,
  "interestRate": "15% per annum",
  "monthlyInstallment": 510.03,
  "totalRepayable": 1530.09,
  "totalInterest": 30.09,
  "schedule": [
    {
      "month": 1,
      "dueDate": "2026-04-16",
      "installment": 510.03,
      "outstandingBalance": 1008.72
    },
    {
      "month": 2,
      "dueDate": "2026-05-16",
      "installment": 510.03,
      "outstandingBalance": 505.3
    },
    {
      "month": 3,
      "dueDate": "2026-06-16",
      "installment": 510.03,
      "outstandingBalance": 0.0
    }
  ]
}
```

---

### Apply for a Loan

**POST** `/api/loan/apply` 🔒

Request:

```json
{
  "loanAmount": 1500,
  "termMonths": 3,
  "loanPurpose": "Home repairs",

  "employerName": "ABC Company",
  "employmentType": "Permanent",
  "jobTitle": "Software Developer",
  "monthlySalary": 25000,
  "startDate": "2020-01-01",
  "employerPhone": "0112345678",
  "employerAddress": "123 Main Street, Johannesburg",

  "monthlyGrossIncome": 25000,
  "monthlyNetIncome": 18000,
  "monthlyExpenses": 8000,
  "otherLoanObligations": 0,

  "bankName": "FNB",
  "accountHolder": "Thabo Nkosi",
  "accountNumber": "62012345678",
  "branchCode": "250655",
  "accountType": "Cheque"
}
```

Response `200 OK`:

```json
{
  "message": "Loan application submitted successfully.",
  "data": {
    "applicationId": 1,
    "loanAmount": 1500,
    "loanTermMonths": 3,
    "monthlyInstallment": 510.03,
    "totalRepayable": 1530.09,
    "totalInterest": 30.09,
    "status": "Pending"
  }
}
```

---

### Approve a Loan

**PUT** `/api/admin/applications/{id}/approve` 🔒

Request:

```json
{
  "creditScore": 750,
  "debtToIncomeRatio": 25.5,
  "affordabilityAmount": 1500,
  "assessedBy": "John Admin"
}
```

Response `200 OK`:

```json
{
  "message": "Loan approved successfully."
}
```

---

### Decline a Loan

**PUT** `/api/admin/applications/{id}/decline` 🔒

Request:

```json
{
  "reason": "Insufficient disposable income",
  "assessedBy": "John Admin"
}
```

Response `200 OK`:

```json
{
  "message": "Loan declined.",
  "reason": "Insufficient disposable income"
}
```

---

### Get Repayment Schedule

**GET** `/api/repayments/{loanId}` 🔒

Response `200 OK`:

```json
[
  {
    "scheduleId": 1,
    "installmentNumber": 1,
    "dueDate": "2026-04-16",
    "amountDue": 510.03,
    "outstandingBalance": 1008.72,
    "isPaid": false,
    "paidAt": null
  },
  {
    "scheduleId": 2,
    "installmentNumber": 2,
    "dueDate": "2026-05-16",
    "amountDue": 510.03,
    "outstandingBalance": 505.3,
    "isPaid": false,
    "paidAt": null
  },
  {
    "scheduleId": 3,
    "installmentNumber": 3,
    "dueDate": "2026-06-16",
    "amountDue": 510.03,
    "outstandingBalance": 0.0,
    "isPaid": false,
    "paidAt": null
  }
]
```

---

## Loan Lifecycle

```
User Applies
     │
     ▼
  Pending  ──► PUT /review
     │
     ▼
Under Review  ──► PUT /approve  or  PUT /decline
     │                    │
     ▼                    ▼
 Approved             Declined
     │
     ▼
PUT /disburse
     │
     ▼
  Active  (repayments begin)
     │
     ▼
  Settled  (fully repaid)
```

---

## Loan Rules

| Rule                | Value         |
| ------------------- | ------------- |
| Minimum Loan Amount | R100          |
| Maximum Loan Amount | R3,000        |
| Maximum Loan Term   | 3 months      |
| Interest Rate       | 15% per annum |
| Minimum Age         | 18 years      |

---

## Validation Rules

| Field          | Rule                      |
| -------------- | ------------------------- |
| First Name     | No numbers allowed        |
| Last Name      | No numbers allowed        |
| Date of Birth  | Must be 18 years or older |
| ID or Passport | At least one is required  |
| Loan Amount    | Between R100 and R3,000   |
| Loan Term      | Between 1 and 3 months    |
| Email          | Must be unique            |

---

## Error Responses

| Status Code | Meaning                                 |
| ----------- | --------------------------------------- |
| 200         | Success                                 |
| 400         | Bad Request — validation failed         |
| 401         | Unauthorized — missing or invalid token |
| 404         | Not Found — resource does not exist     |

Example error response:

```json
{
  "message": "You must be at least 18 years old."
}
```

---

## Project Structure

```
eCashMeUp/
├── Controllers/
│   ├── AuthController.cs
│   ├── LoanController.cs
│   ├── RepaymentController.cs
│   └── AdminController.cs
├── Services/
│   ├── AuthService.cs
│   ├── LoanService.cs
│   ├── LoanCalculatorService.cs
│   └── RepaymentService.cs
├── Models/
│   ├── User.cs
│   ├── LoanApplication.cs
│   ├── EmploymentDetail.cs
│   ├── FinancialDetail.cs
│   ├── BankingDetail.cs
│   ├── RepaymentSchedule.cs
│   ├── CreditAssessment.cs
│   └── LoanDisbursement.cs
├── DTOs/
│   ├── RegisterDto.cs
│   ├── LoginDto.cs
│   ├── LoanCalculateDto.cs
│   └── LoanApplyDto.cs
├── Data/
│   └── AppDbContext.cs
├── Helpers/
│   └── JwtHelper.cs
├── appsettings.json
└── Program.cs
```

---

## Database Tables

| Table                 | Description                  |
| --------------------- | ---------------------------- |
| `users`               | Registered users             |
| `employment_details`  | User employment info         |
| `financial_details`   | User financial info          |
| `banking_details`     | User bank account info       |
| `loan_applications`   | Loan applications            |
| `credit_assessments`  | Assessment results           |
| `loan_disbursements`  | Disbursement records         |
| `repayment_schedules` | Monthly repayment schedule   |
| `repayment_payments`  | Actual payments made         |
| `documents`           | Uploaded documents           |
| `ref_titles`          | Title lookup (Mr, Mrs, etc.) |
| `ref_races`           | Race lookup                  |
| `ref_loan_statuses`   | Loan status lookup           |
| `ref_document_types`  | Document type lookup         |

---

## Reference Data

### Titles

| ID  | Title |
| --- | ----- |
| 1   | Mr    |
| 2   | Mrs   |
| 3   | Miss  |
| 4   | Ms    |
| 5   | Dr    |
| 6   | Prof  |

### Loan Statuses

| ID  | Status       |
| --- | ------------ |
| 1   | Pending      |
| 2   | Under Review |
| 3   | Approved     |
| 4   | Declined     |
| 5   | Disbursed    |
| 6   | Active       |
| 7   | Settled      |
| 8   | Defaulted    |

```

```
