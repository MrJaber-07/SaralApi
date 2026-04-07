# Transaction-Safe Order Engine - Saral ERP Submission

## 1. Project Overview
This system is a fault-tolerant order processing engine designed to handle concurrent requests while maintaining strict data integrity. It ensures that inventory updates, order creation, and payment simulations occur as a single atomic unit.

## 2. Design Explanations

### **Transaction Handling Approach**
We utilize **Database-Level Transactions** via Entity Framework Core's `BeginTransactionAsync`.
* [cite_start]**Atomicity**: All inventory deductions across multiple products must succeed for the order to be confirmed[cite: 14].
* [cite_start]**Reliable Rollback**: If the payment simulation fails or an item is out of stock, the transaction is explicitly rolled back to restore inventory levels[cite: 17, 31].

### **Inventory Locking Strategy**
[cite_start]The system implements **Optimistic Concurrency Control**[cite: 29].
* The `Product` model uses a `[Timestamp]` (RowVersion) column.
* This prevents "Lost Updates" where two users attempt to buy the same last item simultaneously. [cite_start]EF Core detects the version mismatch and triggers a concurrency exception, which we handle to inform the user[cite: 25, 39].

### **Idempotency Implementation**
[cite_start]To satisfy the requirement for **retry safety**, we implement an **Idempotency Key** strategy[cite: 23, 32].
* Every request includes a unique key.
* The system checks for this key in the `Orders` table before processing. [cite_start]If found, it returns the existing order instead of creating a duplicate, preventing double inventory deduction[cite: 24].

### **Failure & Rollback Mechanism**
* [cite_start]**Partial Failures**: If one item in a multi-product order is out of stock, the `InvalidOperationException` triggers a full rollback of all other reserved items in that request[cite: 22].
* [cite_start]**Payment Failure**: If the `SimulatePayment()` method returns false, the order is marked as `FAILED` and inventory is restored[cite: 16, 17].

## 3. Architecture
[cite_start]The project follows **Clean Architecture** principles by using the **Service/Repository Pattern**:
* **Repositories**: Handle raw data access and EF Core interactions.
* **Services**: Orchestrate business logic, transaction boundaries, and state transitions.

## 4. Steps to Run
1. Update the Connection String in `appsettings.json`.
2. Run `dotnet ef database update` to create the schema.
3. Use the Swagger UI at `/swagger` to send a POST request with an `Idempotency-Key` header.
