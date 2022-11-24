CREATE TABLE WeatherForecast
(
    Date DATE NOT NULL PRIMARY KEY,
    TemperatureC INT NOT NULL,
    Summary VARCHAR(100) NOT NULL
);

INSERT INTO WeatherForecast (Date, TemperatureC, Summary)
select now(), 5, 'Холодно и сыро';


