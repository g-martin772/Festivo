# API

*This file includes the specification for every API endpoint.*

## Routes to services

First of all, the general routes to the service through the YARP reverse proxy are listed.

### Ticket Service

The ticket service is routed under `/api/tickets/` to the service `http://festivo.ticketservice:8080`.

### Access Control Service

The access control service is routed under `/api/access/` to the service `http://festivo.accesscontrolservice:8080`.

### Schedule Service

The schedule service is routed under `/api/schedule/` to the service `http://festivo.scheduleservice:8080`.

### Crowd Monitor Service

The crowd monitor service is routed under `/api/crowd/` to the service `http://festivo.crowdmonitorservice:8080`.

### Notification Service

The notification service is routed under `/api/notifications/` to the service `http://festivo.notificationservice:8080`.
