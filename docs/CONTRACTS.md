# Contracts

This file includes all event definitions that are published via the RabbitMQ broker.

## Events

All events are listed below so as to provide a summarized overview of them.
The events come with a description, unique name, the data it includes and
the producers and consumers.

### 1. Ticket Service Events

#### 1.1 TicketPurchased

**Event Type:** `com.festivo.ticket.purchased.v1`
**Producer:** Ticket Service
**Consumers:** Access Control Service, Notification Service

**Routing Keys:**
- `8-ticket.ticket-purchased` → `1-access-control.ticket-purchased`
- `8-ticket.ticket-purchased` → `5-notification.ticket-purchased`

**Data Schema:**

```json
{
  "ticketId": "string",
  "ticketCode": "string",
  "ticketType": "string",
  "price": "number",
  "purchaseDate": "datetime",
  "customerId": "string"
}
```

**Field Descriptions:**
- `ticketId`: Unique identifier for the ticket record
- `ticketCode`: Scannable ticket code (used at gates)
- `ticketType`: Type of ticket (e.g., "Standard", "VIP", "Day Pass")
- `price`: Purchase price in system currency
- `purchaseDate`: ISO 8601 timestamp of purchase
- `customerId`: Unique identifier for the customer

**Business Meaning:** A ticket has been successfully purchased by a customer.

---

#### 1.2 TicketRefunded

**Event Type:** `com.festivo.ticket.refunded.v1` 
**Producer:** Ticket Service
**Consumers:** Access Control Service, Notification Service

**Routing Keys:**
- `8-ticket.ticket-refunded` → `1-access-control.ticket-refunded`
- `8-ticket.ticket-refunded` → `5-notification.ticket-refunded`

**Data Schema:**

```json
{
  "ticketId": "string",
  "ticketCode": "string",
  "reason": "string",
  "refundAmount": "number",
  "refundDate": "datetime"
}
```

**Field Descriptions:**
- `ticketId`: Unique identifier for the ticket record
- `ticketCode`: Ticket code that was refunded
- `reason`: Reason for refund (e.g., "Entry denied - system error", "Customer request")
- `refundAmount`: Amount refunded to customer
- `refundDate`: ISO 8601 timestamp of refund

**Business Meaning:** A ticket has been refunded to the customer.

---

#### 1.3 TicketCancelled

**Event Type:** `com.festivo.ticket.cancelled.v1` 
**Producer:** Ticket Service 
**Consumers:** Access Control Service, Notification Service

**Routing Keys:**
- `8-ticket.ticket-cancelled` → `1-access-control.ticket-cancelled`
- `8-ticket.ticket-cancelled` → `5-notification.ticket-cancelled`

**Data Schema:**

```json
{
  "ticketId": "string",
  "ticketCode": "string",
  "reason": "string",
  "cancellationDate": "datetime"
}
```

**Field Descriptions:**
- `ticketId`: Unique identifier for the ticket record
- `ticketCode`: Ticket code that was cancelled
- `reason`: Reason for cancellation
- `cancellationDate`: ISO 8601 timestamp of cancellation

**Business Meaning:** A ticket has been cancelled and is no longer valid.

---

### 2. Access Control Service Events

#### 2.1 EntryRequested

**Event Type:** `com.festivo.access.entry-requested.v1` 
**Producer:** Access Control Service 
**Consumers:** Orchestrator Service (optional)

**Routing Keys:**
- `1-access-control.entry-requested` → `6-orchestrator.entry-requested`

**Data Schema:**

```json
{
  "ticketId": "string",
  "ticketCode": "string",
  "gateId": "string",
  "requestTime": "datetime",
  "customerId": "string"
}
```

**Field Descriptions:**
- `ticketId`: Unique identifier for the ticket
- `ticketCode`: Scanned ticket code
- `gateId`: Identifier of the entry gate
- `requestTime`: ISO 8601 timestamp of scan attempt
- `customerId`: Unique identifier for the customer

**Business Meaning:** A visitor has requested entry at a gate.

---

#### 2.2 EntryGranted

**Event Type:** `com.festivo.access.entry-granted.v1`
**Producer:** Access Control Service
**Consumers:** Crowd Monitor Service, Notification Service

**Routing Keys:**
- `1-access-control.entry-granted` → `4-crowd-monitor.entry-granted`
- `1-access-control.entry-granted` → `5-notification.entry-granted`

**Data Schema:**

```json
{
  "ticketId": "string",
  "ticketCode": "string",
  "gateId": "string",
  "entryTime": "datetime",
  "customerId": "string"
}
```

**Field Descriptions:**
- `ticketId`: Unique identifier for the ticket
- `ticketCode`: Scanned ticket code
- `gateId`: Identifier of the entry gate
- `entryTime`: ISO 8601 timestamp of successful entry
- `customerId`: Unique identifier for the customer

**Business Meaning:** A visitor has been granted entry and is now inside the festival.

---

#### 2.3 EntryDenied

**Event Type:** `com.festivo.access.entry-denied.v1`
**Producer:** Access Control Service
**Consumers:** Orchestrator Service, Notification Service

**Routing Keys:**
- `1-access-control.entry-denied` → `6-orchestrator.entry-denied`
- `1-access-control.entry-denied` → `5-notification.entry-denied`

**Data Schema:**

```json
{
  "ticketId": "string",
  "ticketCode": "string",
  "gateId": "string",
  "reason": "string",
  "deniedTime": "datetime",
  "customerId": "string"
}
```

**Field Descriptions:**
- `ticketId`: Unique identifier for the ticket
- `ticketCode`: Scanned ticket code
- `gateId`: Identifier of the entry gate
- `reason`: Reason for denial (e.g., "Double entry attempt", "Ticket refunded", "Invalid ticket")
- `deniedTime`: ISO 8601 timestamp of denial
- `customerId`: Unique identifier for the customer

**Business Meaning:** Entry has been denied. May trigger compensating actions like refund.

---

#### 2.4 ExitGranted

**Event Type:** `com.festivo.access.exit-granted.v1`
**Producer:** Access Control Service
**Consumers:** Crowd Monitor Service

**Routing Keys:**
- `1-access-control.exit-granted` → `4-crowd-monitor.exit-granted`

**Data Schema:**

```json
{
  "ticketId": "string",
  "ticketCode": "string",
  "gateId": "string",
  "exitTime": "datetime",
  "customerId": "string"
}
```

**Field Descriptions:**
- `ticketId`: Unique identifier for the ticket
- `gateId`: Identifier of the exit gate
- `exitTime`: ISO 8601 timestamp of exit
- `customerId`: Unique identifier for the customer

**Business Meaning:** A visitor has exited the festival.

---

#### 2.5 ExitDenied

**Event Type:** `com.festivo.access.exit-denied.v1`
**Producer:** Access Control Service
**Consumers:** Notification Service (optional)

**Routing Keys:**
- `1-access-control.exit-denied` → `5-notification.exit-denied`

**Data Schema:**

```json
{
  "ticketId": "string",
  "ticketCode": "string",
  "gateId": "string",
  "reason": "string",
  "deniedTime": "datetime",
  "customerId": "string"
}
```

**Field Descriptions:**
- `ticketId`: Unique identifier for the ticket
- `gateId`: Identifier of the exit gate
- `reason`: Reason for denial
- `deniedTime`: ISO 8601 timestamp of denial
- `customerId`: Unique identifier for the customer

**Business Meaning:** Exit has been denied (rare scenario).

---

### 3. Schedule Service Events

#### 3.1 ScheduleItemCreated

**Event Type:** `com.festivo.schedule.item-created.v1`
**Producer:** Schedule Service
**Consumers:** Notification Service (optional)

**Routing Keys:**
- `7-schedule.schedule-item-created` → `5-notification.schedule-item-created`

**Data Schema:**

```json
{
  "itemId": "string",
  "stageId": "string",
  "stageName": "string",
  "artistName": "string",
  "startTime": "datetime",
  "endTime": "datetime",
  "createdAt": "datetime"
}
```

**Field Descriptions:**
- `itemId`: Unique identifier for the schedule item
- `stageId`: Unique identifier for the stage
- `stageName`: Display name of the stage
- `artistName`: Name of the performing artist
- `startTime`: ISO 8601 timestamp of performance start
- `endTime`: ISO 8601 timestamp of performance end
- `createdAt`: ISO 8601 timestamp of creation

**Business Meaning:** A new performance has been added to the schedule.

---

#### 3.2 ScheduleItemUpdated

**Event Type:** `com.festivo.schedule.item-updated.v1`
**Producer:** Schedule Service
**Consumers:** Notification Service (optional)

**Routing Keys:**
- `7-schedule.schedule-item-updated` → `5-notification.schedule-item-updated`

**Data Schema:**

```json
{
  "itemId": "string",
  "stageId": "string",
  "stageName": "string",
  "artistName": "string",
  "startTime": "datetime",
  "endTime": "datetime",
  "updatedAt": "datetime"
}
```

**Field Descriptions:**
- `itemId`: Unique identifier for the schedule item
- `stageId`: Unique identifier for the stage
- `stageName`: Display name of the stage
- `artistName`: Name of the performing artist
- `startTime`: ISO 8601 timestamp of performance start
- `endTime`: ISO 8601 timestamp of performance end
- `updatedAt`: ISO 8601 timestamp of update

**Business Meaning:** An existing performance schedule has been modified.

---

#### 3.3 ScheduleItemDeleted

**Event Type:** `com.festivo.schedule.item-deleted.v1`
**Producer:** Schedule Service
**Consumers:** Notification Service (optional)

**Routing Keys:**
- `7-schedule.schedule-item-deleted` → `5-notification.schedule-item-deleted`

**Data Schema:**

```json
{
  "itemId": "string",
  "stageId": "string",
  "deletedAt": "datetime"
}
```

**Field Descriptions:**
- `itemId`: Unique identifier for the deleted schedule item
- `stageId`: Unique identifier for the stage
- `deletedAt`: ISO 8601 timestamp of deletion

**Business Meaning:** A performance has been removed from the schedule.

---

### 4. Crowd Monitor Service Events

#### 4.1 OccupancyUpdated

**Event Type:** `com.festivo.crowd.occupancy-updated.v1`
**Producer:** Crowd Monitor Service
**Consumers:** Notification Service

**Routing Keys:**
- `4-crowd-monitor.occupancy-updated` → `5-notification.occupancy-updated`

**Data Schema:**

```json
{
  "stageId": "string",
  "stageName": "string",
  "currentOccupancy": "number",
  "maxCapacity": "number",
  "occupancyPercentage": "number",
  "updatedAt": "datetime"
}
```

**Field Descriptions:**
- `stageId`: Unique identifier for the stage/area
- `stageName`: Display name of the stage/area
- `currentOccupancy`: Current number of people
- `maxCapacity`: Maximum allowed capacity
- `occupancyPercentage`: Percentage of capacity (0-100)
- `updatedAt`: ISO 8601 timestamp of update

**Business Meaning:** The occupancy count for a stage/area has changed.

---

#### 4.2 CapacityWarningIssued

**Event Type:** `com.festivo.crowd.capacity-warning.v1`
**Producer:** Crowd Monitor Service
**Consumers:** Notification Service, Orchestrator Service

**Routing Keys:**
- `4-crowd-monitor.capacity-warning` → `5-notification.capacity-warning`
- `4-crowd-monitor.capacity-warning` → `6-orchestrator.capacity-warning`

**Data Schema:**

```json
{
  "stageId": "string",
  "stageName": "string",
  "currentOccupancy": "number",
  "maxCapacity": "number",
  "occupancyPercentage": "number",
  "warningThreshold": "number",
  "issuedAt": "datetime"
}
```

**Field Descriptions:**
- `stageId`: Unique identifier for the stage/area
- `stageName`: Display name of the stage/area
- `currentOccupancy`: Current number of people
- `maxCapacity`: Maximum allowed capacity
- `occupancyPercentage`: Percentage of capacity (0-100)
- `warningThreshold`: Threshold percentage that triggered warning (e.g., 80)
- `issuedAt`: ISO 8601 timestamp of warning

**Business Meaning:** Stage/area is nearing capacity. Warning to staff and attendees.

---

#### 4.3 CapacityCriticalIssued

**Event Type:** `com.festivo.crowd.capacity-critical.v1`
**Producer:** Crowd Monitor Service
**Consumers:** Notification Service, Orchestrator Service

**Routing Keys:**
- `4-crowd-monitor.capacity-critical` → `5-notification.capacity-critical`
- `4-crowd-monitor.capacity-critical` → `6-orchestrator.capacity-critical`

**Data Schema:**

```json
{
  "stageId": "string",
  "stageName": "string",
  "currentOccupancy": "number",
  "maxCapacity": "number",
  "occupancyPercentage": "number",
  "criticalThreshold": "number",
  "issuedAt": "datetime"
}
```

**Field Descriptions:**
- `stageId`: Unique identifier for the stage/area
- `stageName`: Display name of the stage/area
- `currentOccupancy`: Current number of people
- `maxCapacity`: Maximum allowed capacity
- `occupancyPercentage`: Percentage of capacity (0-100)
- `criticalThreshold`: Threshold percentage that triggered critical alert (e.g., 95)
- `issuedAt`: ISO 8601 timestamp of alert

**Business Meaning:** Stage/area is at or over capacity. Requires immediate action.

---

#### 4.4 CapacityBackToNormal

**Event Type:** `com.festivo.crowd.capacity-normal.v1`
**Producer:** Crowd Monitor Service
**Consumers:** Notification Service, Orchestrator Service

**Routing Keys:**
- `4-crowd-monitor.capacity-normal` → `5-notification.capacity-normal`
- `4-crowd-monitor.capacity-normal` → `6-orchestrator.capacity-normal`

**Data Schema:**

```json
{
  "stageId": "string",
  "stageName": "string",
  "currentOccupancy": "number",
  "maxCapacity": "number",
  "occupancyPercentage": "number",
  "normalizedAt": "datetime"
}
```

**Field Descriptions:**
- `stageId`: Unique identifier for the stage/area
- `stageName`: Display name of the stage/area
- `currentOccupancy`: Current number of people
- `maxCapacity`: Maximum allowed capacity
- `occupancyPercentage`: Percentage of capacity (0-100)
- `normalizedAt`: ISO 8601 timestamp of normalization

**Business Meaning:** Stage/area occupancy has returned to safe levels.

---

### 6. Orchestrator Service Events (SAGA)

#### 6.1 SagaStarted

**Event Type:** `com.festivo.saga.started.v1`
**Producer:** Orchestrator Service
**Consumers:** None (audit only)

**Routing Keys:**
- `6-orchestrator.saga-started` (no consumers)

**Data Schema:**

```json
{
  "sagaId": "string",
  "sagaType": "string",
  "triggerEventType": "string",
  "startedAt": "datetime"
}
```

**Field Descriptions:**
- `sagaId`: Unique identifier for the saga instance
- `sagaType`: Type of saga (e.g., "RefundSaga", "EntryDenialSaga")
- `triggerEventType`: Event type that triggered the saga
- `startedAt`: ISO 8601 timestamp of saga start

**Business Meaning:** A saga orchestration process has started.

---

#### 6.2 SagaCompleted

**Event Type:** `com.festivo.saga.completed.v1`
**Producer:** Orchestrator Service
**Consumers:** None (audit only)

**Routing Keys:**
- `6-orchestrator.saga-completed` (no consumers)

**Data Schema:**

```json
{
  "sagaId": "string",
  "sagaType": "string",
  "result": "string",
  "completedAt": "datetime"
}
```

**Field Descriptions:**
- `sagaId`: Unique identifier for the saga instance
- `sagaType`: Type of saga
- `result`: Result description
- `completedAt`: ISO 8601 timestamp of completion

**Business Meaning:** A saga has completed successfully.

---

#### 6.3 SagaFailed

**Event Type:** `com.festivo.saga.failed.v1`
**Producer:** Orchestrator Service
**Consumers:** Notification Service

**Routing Keys:**
- `6-orchestrator.saga-failed` → `5-notification.saga-failed`

**Data Schema:**

```json
{
  "sagaId": "string",
  "sagaType": "string",
  "errorReason": "string",
  "failedAt": "datetime"
}
```

**Field Descriptions:**
- `sagaId`: Unique identifier for the saga instance
- `sagaType`: Type of saga
- `errorReason`: Reason for failure
- `failedAt`: ISO 8601 timestamp of failure

**Business Meaning:** A saga has failed and may require manual intervention.

---

#### 6.4 CompensationTriggered

**Event Type:** `com.festivo.saga.compensation-triggered.v1`
**Producer:** Orchestrator Service
**Consumers:** None (audit only)

**Routing Keys:**
- `6-orchestrator.compensation-triggered` (no consumers)

**Data Schema:**

```json
{
  "sagaId": "string",
  "sagaType": "string",
  "compensationAction": "string",
  "reason": "string",
  "triggeredAt": "datetime"
}
```

**Field Descriptions:**
- `sagaId`: Unique identifier for the saga instance
- `sagaType`: Type of saga
- `compensationAction`: Description of compensation action
- `reason`: Reason for compensation
- `triggeredAt`: ISO 8601 timestamp of trigger

**Business Meaning:** A compensating transaction has been triggered to rollback changes.

---

#### 6.5 RefundRequested

**Event Type:** `com.festivo.saga.refund-requested.v1`
**Producer:** Orchestrator Service
**Consumers:** Ticket Service

**Routing Keys:**
- `6-orchestrator.refund-requested` → `8-ticket.refund-requested`

**Data Schema:**

```json
{
  "ticketId": "string",
  "ticketCode": "string",
  "reason": "string",
  "refundAmount": "number",
  "requestedAt": "datetime"
}
```

**Field Descriptions:**
- `ticketId`: Unique identifier for the ticket
- `ticketCode`: Ticket code to refund
- `reason`: Reason for refund request
- `refundAmount`: Amount to refund
- `requestedAt`: ISO 8601 timestamp of request

**Business Meaning:** The orchestrator is requesting a ticket refund as part of a saga.

---

## Service Event Flow Summary

### Ticket Service
**Publishes:**
- `TicketPurchased` → Access Control, Notification
- `TicketRefunded` → Access Control, Notification
- `TicketCancelled` → Access Control, Notification

**Consumes:**
- `RefundRequested` ← Orchestrator

---

### Access Control Service
**Publishes:**
- `EntryRequested` → Orchestrator (optional)
- `EntryGranted` → Crowd Monitor, Notification
- `EntryDenied` → Orchestrator, Notification
- `ExitGranted` → Crowd Monitor
- `ExitDenied` → Notification (optional)

**Consumes:**
- `TicketPurchased` ← Ticket
- `TicketRefunded` ← Ticket

---

### Schedule Service
**Publishes:**
- `ScheduleItemCreated` → Notification (optional)
- `ScheduleItemUpdated` → Notification (optional)
- `ScheduleItemDeleted` → Notification (optional)

**Consumes:**
- None

---

### Crowd Monitor Service
**Publishes:**
- `OccupancyUpdated` → Notification
- `CapacityWarningIssued` → Notification, Orchestrator
- `CapacityCriticalIssued` → Notification, Orchestrator
- `CapacityBackToNormal` → Notification, Orchestrator

**Consumes:**
- `EntryGranted` ← Access Control
- `ExitGranted` ← Access Control

---

### Notification Service
**Publishes:**
- `NotificationSent` (audit only)
- `BroadcastNotificationSent` (audit only)

**Consumes:**
- `TicketPurchased` ← Ticket
- `EntryGranted` ← Access Control
- `EntryDenied` ← Access Control
- `CapacityWarningIssued` ← Crowd Monitor
- `CapacityCriticalIssued` ← Crowd Monitor
- `OccupancyUpdated` ← Crowd Monitor

---

### Orchestrator Service
**Publishes:**
- `SagaStarted` (audit only)
- `SagaCompleted` (audit only)
- `SagaFailed` → Notification
- `CompensationTriggered` (audit only)
- `RefundRequested` → Ticket

**Consumes:**
- `EntryDenied` ← Access Control
- `CapacityCriticalIssued` ← Crowd Monitor

---

## Event Flow Diagram

```
┌─────────────────┐
│  Ticket Service │
└────────┬────────┘
         │ TicketPurchased
         ├──────────────────────┐
         │                      │
         v                      v
┌────────────────────┐   ┌──────────────────┐
│ Access Control     │   │  Notification    │
│    Service         │   │    Service       │
└──────┬─────────────┘   └──────────────────┘
       │ EntryGranted
       │ EntryDenied
       │ ExitGranted
       ├──────────────────┐
       │                  │
       v                  v
┌──────────────────┐  ┌──────────────────┐
│  Crowd Monitor   │  │  Orchestrator    │
│    Service       │  │    Service       │
└──────┬───────────┘  └─────┬────────────┘
       │                     │
       │ OccupancyUpdated    │ RefundRequested
       │ CapacityWarning     │
       │ CapacityCritical    │
       ├─────────────────────┤
       │                     │
       v                     v
┌──────────────────┐   ┌─────────────────┐
│  Notification    │   │  Ticket Service │
│    Service       │   │                 │
└──────────────────┘   └─────────────────┘

┌─────────────────┐
│ Schedule Service│
└────────┬────────┘
         │ ScheduleItemCreated
         │
         v
┌──────────────────┐
│  Notification    │
│    Service       │
└──────────────────┘
```

## Dead Letter Queue Configuration

Each service has a dead letter queue configured for failed message handling:

- **Exchange:** `messages`
- **Dead Letter Routing Keys:**
  - `1-access-control.error`
  - `2-api-gateway.error`
  - `3-callback.error`
  - `4-crowd-monitor.error`
  - `5-notification.error`
  - `6-orchestrator.error`
  - `7-schedule.error`
  - `8-ticket.error`

Messages that fail processing (after NACK) are routed to the respective error queue for logging and manual inspection.

---
