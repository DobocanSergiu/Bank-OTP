# Bank-OTP
Bank OTP project that simulates the login process of a bank application.
# Bank Login System with OTP

## Overview

A simple implementation of a bank login system that uses OTP codes alongside passwords.

## Frontend

The frontend was written using **ReactJS**. For the toast message component, I used [react-hot-toast](https://react-hot-toast.com/).

## Backend

The backend was written using **.NET**. For this proof-of-concept, an in-memory database is used to save user information. The passwords are hashed using **SHA-256**. For the generation of the OTP codes, I used [Otp.NET](https://www.nuget.org/packages/Otp.NET/1.2.2).

## Screenshots

*(Insert Screenshots here)*


# API Endpoints Documentation
## 1. **Create User**

### `POST /api/createUser`

Creates a new user with a unique username and a securely hashed password.

### **Request Body**
``{ 
	"Username": "john_doe", "Password": "StrongPass123!" 
	}``
### **Response (201 Created)**
``{
  "message": "User created successfully"
}``
## 2. **Login User**

### `POST /api/loginUser`

Authenticates a user with their username and password.

### **Request Body**

``
{ "Username": "john_doe", "Password": "StrongPass123!" }
``

**Response (200 OK)**

``{
  "message": "Login Succesfull"
}
``

**Response (401 Unauthorized)**
``{
  "message": "Invalid Password"
}
``
## 3. **Request OTP Code**

### `POST /api/requestCode`

Generates a one-time password (OTP) for a user.
**Request Body**

``{
  "Username": "john_doe"
}
``

**Response (200 OK)**
``{
  "owner": "john_doe",
  "code": "123456",
  "time": 30
}
``
## 4. **Validate OTP Code**

### `POST /api/validateCode`

Validates an OTP code provided by the user.
**Request Body**
``{
  "Username": "john_doe",
  "Code": "123456"
}
``

**Response (200 OK)**
``{
  "message": "Validation successful"
}
``

**Response (400 Bad Request) - Incorrect Code**
``{
  "message": "Incorrect code"
}``

**Response (400 Bad Request) - Code Expired**
``{
  "message": "Code has expired"
}
``
