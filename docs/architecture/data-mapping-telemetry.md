# Сопоставление данных: телеметрическая запись (ПР3)

Источник: пакет трекера GPS/OBD (JSON после разбора TCP-пакета). Приёмник: таблица `telemetry_records` (PostgreSQL).

| Поле источника | Поле приёмника | Тип приёмника | Правило |
|---|---|---|---|
| imei | vehicle_id | uuid | Поиск автомобиля по IMEI в справочнике |
| dt | recorded_at | timestamptz | Unix-время в секундах, перевод в UTC |
| — | received_at | timestamptz | Время получения приёмником |
| lat | latitude | numeric(9,6) | Деление на 1 000 000 |
| lon | longitude | numeric(9,6) | Деление на 1 000 000 |
| spd | speed_kmh | smallint | Без изменения |
| hdg | heading_deg | smallint | 0–359, иначе NULL |
| sat | satellites | smallint | Без изменения |
| fuel | fuel_level_pct | numeric(4,1) | Литры / объём бака × 100 |
| odo | odometer_km | numeric(9,1) | Метры / 1000, округление до 0,1 |
| ign | ignition_on | boolean | 1 — true, 0 — false |
| rpm | engine_rpm | integer | Без изменения |
| tmp | coolant_temp_c | smallint | Без изменения |
| bat | battery_v | numeric(4,2) | Милливольты / 1000 |
| dtc | dtc_codes | text[] | Верхний регистр, удаление дублей |
| — | quality | text | OK, SUSPECT или REJECTED |

## Обработка ошибок
- Нет обязательных полей или некорректный JSON — пакет отклоняется (PARSE_ERROR).
- IMEI не найден в справочнике — пакет помещается в карантин.
- Повтор пары (imei, dt) — вторая запись пропускается.
- Координаты вне диапазона — запись отклоняется (COORD_RANGE).
- Скорость больше 250 км/ч, будущее время, уменьшение пробега, топливо вне 0–100 % — запись сохраняется с quality = SUSPECT.
