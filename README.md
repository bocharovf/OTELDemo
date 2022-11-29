# OTELDemo
Демонстрационное приложение для митапа [Observability – показать все, что скрыто 30.11.2022](https://ak-bars-digital-org.timepad.ru/event/2229783/)

Показано инструментирование .NET приложения для сбора логов, метрик и распределенной трассировки с помощью Open Telemetry.

## Схема
<img width="637" alt="scheme" src="https://user-images.githubusercontent.com/5367465/204533075-1057f02c-5b97-4804-9dc0-fc336a6cbe10.PNG">

## Запуск
В корневом каталоге запустить
`docker-compose up -d --build`

## Работа с приложением
Для генерации данных открыть несколько раз страницу http://localhost:80 

Посмотреть трейсы в Jaeger можно по ссылке http://localhost:16686/search

Посмотреть метрики и логи можно в Grafana http://localhost:3000 (логин/пароль - admin/admin)

Для этого нужно создайть два источника данных:

<img width="300" alt="data sources" src="https://user-images.githubusercontent.com/5367465/204534443-69e91a9f-1472-4886-8d5e-d8aa7a438570.PNG">

И импортировать дашборд из файла *OpenTelemetry ASP.NET Core Metrics-1669721792271.json*

