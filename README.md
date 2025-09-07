# E-Commerce API

This project is the **backend** implementation of an E-Commerce application built with **ASP.NET Core Web API**.
It was developed as part of a course on building a full-stack app with **.NET Core** and **Angular**, but this repo contains only the backend logic.

---

## 🚀 Features

* ASP.NET Core 9 Web API
* Entity Framework Core with Code-First Migrations
* Repository Pattern & Unit of Work
* Specification Pattern for flexible queries
* Authentication & Authorization with ASP.NET Identity
* Shopping cart stored in **Redis**
* Order management
* Payment integration with **Stripe** (server-side only)
* Error handling middleware & validation responses
* Pagination, Filtering, Sorting, Searching

---

## 🛠️ Tech Stack

* **.NET Core 9** (Web API)
* **Entity Framework Core**
* **SQL Server** (database)
* **Redis** (shopping cart)
* **Stripe API** (payments)

---

## 📂 Project Structure

```
src/
 ├── API/              
 ├── Core/             
 ├── Infrastructure/   
```

---

## ⚡ Getting Started

### 1. Clone the repo

```bash
git clone https://github.com/your-username/ecommerce-backend.git
```

### 2. Navigate into the project

```bash
cd ecommerce-backend/src/API
```

### 3. Update connection string

Edit **`appsettings.Development.json`** with your SQL Server & Redis configuration.

### 4. Run migrations

```bash
dotnet ef database update
```

### 5. Run the API

```bash
dotnet run
```

---

## 📜 License

This project is for learning purposes.
Feel free to use, modify, and extend it.

