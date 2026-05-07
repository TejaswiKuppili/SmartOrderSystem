# Smart Order Management System

## Project Overview

Smart Order Management System is a microservices-based distributed backend application built using .NET 8.

This project demonstrates:

* Microservices Architecture
* Event-Driven Communication
* RabbitMQ Integration
* Advanced Data Access (ADO.NET + Dapper + EF Core)
* CQRS Pattern
* Clean Architecture
* Docker Containerization
* Kubernetes Deployment
* CI/CD using GitHub Actions
* Enterprise Design Patterns

The application simulates a real-world order processing workflow where OrderService creates orders and PaymentService processes payments asynchronously through RabbitMQ.

---

# Solution Structure

```text
SmartOrderSystem/
│
├── BuildingBlocks/
├── OrderService/
├── PaymentService/
├── ProductService/
├── NotificationService/
├── docker-compose.yml
├── SmartOrderSystem.sln
└── .github/workflows/
```

---

# Architecture

```text
Client / Swagger
       |
OrderService
       |
RabbitMQ (Event Bus)
       |
PaymentService
       |
SQL Server
```

---

# Core Projects

# 1. BuildingBlocks

Shared library used across all microservices.

## Folder Structure

```text
BuildingBlocks/
├── EventBus/
│   ├── IEventBus.cs
│   └── RabbitMQEventBus.cs
│
├── Events/
│   ├── OrderCreatedEvent.cs
│   └── PaymentSuccessEvent.cs
│
└── BuildingBlocks.csproj
```

## File Responsibilities

### EventBus/IEventBus.cs

Defines the contract for publishing and subscribing to events.

### EventBus/RabbitMQEventBus.cs

Implements RabbitMQ-based event publishing.

Responsibilities:

* Create RabbitMQ connection
* Publish messages
* Queue management
* Event serialization/deserialization
* Retry handling

### Events/OrderCreatedEvent.cs

Shared event contract published after order creation.

### Events/PaymentSuccessEvent.cs

Shared event contract published after successful payment processing.

---

# 2. OrderService

Responsible for:

* Order creation
* Saving orders into SQL Server
* Publishing OrderCreatedEvent
* Exposing REST APIs

---

## OrderService Structure

```text
OrderService/
│
├── API/
│   └── Controllers/
│       └── OrdersController.cs
│
├── Application/
│   ├── Commands/
│   │   └── CreateOrderCommand.cs
│   │
│   ├── DTOs/
│   │   └── CreateOrderDto.cs
│   │
│   ├── Handlers/
│   │   └── CreateOrderHandler.cs
│   │
│   └── Interfaces/
│       ├── IOrderCommandRepository.cs
│       ├── IOrderQueryRepository.cs
│       ├── IOrderRepository.cs
│       └── IUnitOfWork.cs
│
├── Domain/
│   └── Entities/
│       ├── Order.cs
│       └── OrderItem.cs
│
├── Infrastructure/
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   │
│   └── Repositories/
│       ├── OrderCommandRepository.cs
│       ├── OrderQueryRepository.cs
│       ├── OrderRepository.cs
│       └── UnitOfWork.cs
│
├── Migrations/
├── Program.cs
├── Dockerfile
└── appsettings.json
```

---

# OrderService File Explanation

## API/Controllers/OrdersController.cs

Exposes API endpoints.

Responsibilities:

* Create Orders
* Fetch Orders
* Delegate requests through MediatR

---

## Application/Commands/CreateOrderCommand.cs

Represents command object for creating orders.

Used in CQRS write flow.

---

## Application/DTOs/CreateOrderDto.cs

Request payload model used by API.

Contains:

* UserId
* Product details
* Quantity
* Price

---

## Application/Handlers/CreateOrderHandler.cs

Core business logic handler.

Responsibilities:

* Validate request
* Create order entity
* Save order
* Publish OrderCreatedEvent

Patterns Used:

* Mediator Pattern
* CQRS

---

## Application/Interfaces/

### IOrderCommandRepository.cs

Defines write-side repository contract.

### IOrderQueryRepository.cs

Defines read-side repository contract.

### IOrderRepository.cs

Generic order repository abstraction.

### IUnitOfWork.cs

Handles transaction management.

---

## Domain/Entities/Order.cs

Represents Order aggregate root.

Contains:

* OrderId
* UserId
* TotalAmount
* CreatedDate
* OrderItems

---

## Domain/Entities/OrderItem.cs

Represents individual order items.

Contains:

* ProductId
* Quantity
* Price

---

## Infrastructure/Data/ApplicationDbContext.cs

EF Core DbContext.

Responsibilities:

* Database mapping
* Entity configuration
* SQL Server connection
* Migration support

---

## Infrastructure/Repositories/OrderCommandRepository.cs

Implements write-side logic using ADO.NET.

Responsibilities:

* Insert Orders
* Insert OrderItems
* Handle SQL transactions

Advanced Concept:

* ADO.NET Transaction Handling

---

## Infrastructure/Repositories/OrderQueryRepository.cs

Implements read-side logic using Dapper.

Responsibilities:

* Fetch Orders
* Fetch OrderItems
* Execute optimized SQL queries

Advanced Concept:

* Dapper for high-performance reads

---

## Infrastructure/Repositories/OrderRepository.cs

General repository implementation.

---

## Infrastructure/Repositories/UnitOfWork.cs

Implements transaction management.

Responsibilities:

* Commit
* Rollback
* Maintain consistency

---

## Program.cs

Application startup configuration.

Responsibilities:

* Dependency Injection
* MediatR registration
* EF Core configuration
* RabbitMQ registration
* Swagger setup
* Database initialization

---

## Dockerfile

Docker image configuration for OrderService.

---

## Migrations/

Contains EF Core database migration files.

### 20260410093917_InitialCreate.cs

Creates initial database schema.

---

# 3. PaymentService

Responsible for:

* Listening to OrderCreatedEvent
* Processing payments
* Publishing PaymentSuccessEvent

---

## PaymentService Structure

```text
PaymentService/
│
├── Infrastructure/
│   └── Messaging/
│       └── OrderCreatedConsumer.cs
│
├── Program.cs
├── Dockerfile
└── appsettings.json
```

---

# PaymentService File Explanation

## Infrastructure/Messaging/OrderCreatedConsumer.cs

RabbitMQ consumer implementation.

Responsibilities:

* Listen to OrderCreatedEvent
* Process payment asynchronously
* Publish PaymentSuccessEvent

Patterns Used:

* Publish-Subscribe Pattern
* Background Worker Pattern

---

## Program.cs

Registers:

* RabbitMQ EventBus
* Background consumer
* Dependency Injection

---

## Dockerfile

Docker image configuration for PaymentService.

---

# 4. ProductService

Currently a placeholder service generated for future expansion.

Contains:

* Default WeatherForecastController

Future scope:

* Product management
* Inventory management

---

# 5. NotificationService

Currently a placeholder microservice.

Future scope:

* Email notifications
* SMS notifications
* Push notifications

---

# Advanced Data Access Implementation

This project demonstrates hybrid data access strategy.

---

## ADO.NET

Implemented in:

```text
OrderCommandRepository.cs
```

Used for:

* Transactional write operations
* Insert operations
* Atomic commits

Benefits:

* Fine-grained SQL control
* Better transaction handling

---

## Dapper

Implemented in:

```text
OrderQueryRepository.cs
```

Used for:

* Read-heavy operations
* Optimized query execution
* Multi-table mapping

Benefits:

* Lightweight ORM
* High performance

---

## EF Core

Implemented in:

```text
ApplicationDbContext.cs
```

Used for:

* Entity mapping
* DbContext management
* Migrations
* Retry policies

---

# CQRS Implementation

The project separates:

| Operation       | Implementation |
| --------------- | -------------- |
| Command (Write) | ADO.NET        |
| Query (Read)    | Dapper         |

This improves:

* Performance
* Maintainability
* Scalability

---

# Design Patterns Used

| Pattern              | Implementation             |
| -------------------- | -------------------------- |
| Repository Pattern   | OrderRepository            |
| CQRS                 | Command + Query separation |
| Mediator Pattern     | MediatR                    |
| Publish-Subscribe    | RabbitMQ                   |
| Dependency Injection | Program.cs                 |
| Unit of Work         | UnitOfWork.cs              |
| Factory Pattern      | RabbitMQ ConnectionFactory |

---

# Event Flow

```text
1. User creates order
2. OrderService saves order
3. OrderCreatedEvent published
4. RabbitMQ stores event
5. PaymentService consumes event
6. Payment processed
7. PaymentSuccessEvent published
```

---

# Docker Implementation

## Dockerfiles

Implemented for:

* OrderService
* PaymentService

## docker-compose.yml

Orchestrates:

* OrderService
* PaymentService
* SQL Server
* RabbitMQ

Run project:

```bash
docker compose up --build
```

---

# Kubernetes Deployment

Kubernetes manifests were created for:

* OrderService Deployment
* PaymentService Deployment
* SQL Server Deployment
* RabbitMQ Deployment
* Services for networking

Commands:

```bash
kubectl apply -f k8s/
kubectl get pods
```

---

# CI/CD Implementation

Implemented using GitHub Actions.

Workflow location:

```text
.github/workflows/docker-ci.yml
```

Pipeline Steps:

```text
1. Restore packages
2. Build solution
3. Build Docker images
4. Push images to Docker Hub
5. Deploy to environment
```

---

# APIs

## Create Order

```http
POST /api/orders
```

Sample Request:

```json
{
  "userId": "user-1",
  "items": [
    {
      "productId": "P100",
      "quantity": 2,
      "price": 100
    }
  ]
}
```

---

## Get Orders

```http
GET /api/orders/{userId}
```

---

# RabbitMQ Access

Management UI:

```text
http://localhost:15672
```

Credentials:

```text
Username: guest
Password: guest
```

---

# SQL Server Access

Use SQL Server Management Studio.

| Field    | Value            |
| -------- | ---------------- |
| Server   | localhost,1433   |
| Username | sa               |
| Password | Your_password123 |

---

# Key Learning Outcomes

This project demonstrates:

* Enterprise microservices architecture
* Event-driven systems
* Advanced data access concepts
* CQRS implementation
* Docker containerization
* Kubernetes orchestration
* CI/CD automation
* Distributed system communication
* Real-world debugging and resiliency
