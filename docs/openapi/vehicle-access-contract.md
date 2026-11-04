# Контракт операции доступа к автомобилю (ПР4)

`POST /api/v1/vehicle-access`, заголовок `Authorization: Bearer <JWT>`.

## Запрос
```json
{
  "bookingId": "5b0e0c8a-6f3e-4c2a-9a41-3d7f1e2b9c40",
  "vehicleVin": "XTA210930Y2765432",
  "employeeNumber": "004512",
  "action": "UNLOCK",
  "otp": "385921",
  "location": { "latitude": 59.934280, "longitude": 30.335100, "accuracyM": 12.5 },
  "clientTime": "2026-11-04T09:00:12Z",
  "nonce": "c0f1d6a2-8b5e-4f7a-b3c9-1e2d4a6b8c0f"
}
```

## Валидация
| Поле | Правило |
|---|---|
| bookingId | UUID v4 |
| vehicleVin | `^[A-HJ-NPR-Z0-9]{17}$` |
| employeeNumber | `^[0-9]{6}$` |
| action | UNLOCK или LOCK |
| otp | `^[0-9]{6}$` |
| location.latitude / longitude | −90…90 / −180…180 |
| location.accuracyM | 0…100, необязательное |
| clientTime | RFC 3339, UTC, расхождение с сервером не более 60 с |
| nonce | UUID, не повторяется 5 минут |

## Ответы
| HTTP | code |
|---|---|
| 200 | SUCCESS |
| 400 | VALIDATION_ERROR |
| 401 | UNAUTHORIZED |
| 403 | FORBIDDEN |
| 404 | BOOKING_NOT_FOUND |
| 409 | REPLAY_DETECTED |
| 422 | BOOKING_NOT_ACTIVE, STALE_REQUEST, OTP_INVALID, GEOFENCE_VIOLATION |
| 429 | TOO_MANY_ATTEMPTS |
