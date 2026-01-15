# Queue Specification

*This file contains information about the specification of the queues.*

## Dead Letter Queue

This queue binds the exchange "message-exchange" and accepts the routing key
"#.error" to denote a message that was processed in a wrong way.

## Access Control Queue

This queue binds the exchange "message-exchange" and accepts the routing key
"1-access-control.normal", whereas wrongly processed messages go to 
"1-access-control.error" and end up in the Dead Letter Queue.

## Api Gateway Queue

This queue binds the exchange "message-exchange" and accepts the routing key
"2-api-gateway.normal", whereas wrongly processed messages go to
"2-api-gateway.error" and end up in the Dead Letter Queue.

## Callback Queue

This queue binds the exchange "message-exchange" and accepts the routing key
"3-callback.normal", whereas wrongly processed messages go to
"3-callback.error" and end up in the Dead Letter Queue.

## Crowd Monitor Queue

This queue binds the exchange "message-exchange" and accepts the routing key
"4-crowd-monitor.normal", whereas wrongly processed messages go to
"4-crowd-monitor.error" and end up in the Dead Letter Queue.

## Notification Queue

This queue binds the exchange "message-exchange" and accepts the routing key
"5-notification.normal", whereas wrongly processed messages go to
"5-notification.error" and end up in the Dead Letter Queue.

## Orchestrator Queue

This queue binds the exchange "message-exchange" and accepts the routing key
"6-orchestrator.normal", whereas wrongly processed messages go to
"6-orchestrator.error" and end up in the Dead Letter Queue.

## Schedule Queue

This queue binds the exchange "message-exchange" and accepts the routing key
"7-schedule.normal", whereas wrongly processed messages go to
"7-schedule.error" and end up in the Dead Letter Queue.

## Ticket Queue

This queue binds the exchange "message-exchange" and accepts the routing key
"8-ticket.normal", whereas wrongly processed messages go to
"8-ticket.error" and end up in the Dead Letter Queue.
