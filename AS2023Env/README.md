# Окружение для компетенции "Программные решения для бизнеса" на AtomSkills 2023

## Принцип работы
При старте сервис создаёт фейковых сотрудников на каждую должность, а на каждого сотрудника ровно одну штатную единицу в статусе "Закрыта". Далее раз в заданный период времени сервис будет уволнять сотрудника до тех пор, пока число открытых штатных единиц не достигнет некоторого предела. При добавлении сотрудника штатная единица поменяется в статус "Ожидание" на другой заданный период времени, после чего переходит в статус "Закрыта".

## Инструкция по развёртыванию

### I. Задать необходимые параметры в файле `docker-compose.yml`

| Парметр | Пример значения | Описание |
| --- | --- | --- |
| `ports` | _5000:80_ | Маппинг портов. Первое число определяет, на каком порту будет открываться сервис. Второе число всегда должно быть 80. |
| `AS23_CREDENTIALS` в блоке `environment` | _admin:password_ | Логин и пароль для Basic-авторизации с сервисе. Какие будут установлены здесь, такие и потребуются впоследствии клиентам для авторизации. |
| `AS23_CONFIG` в блоке `environment` | _{"empPerPosMin":2, "empPerPosMax":4, "activeUnitsMax":5, "fireDelaySec":15, "pendingDelaySec":30}_ | JSON с конфигурацией сервиса, см. ниже |
| `AS23_POSITIONS` в блоке `environment` | _{"backend-developer":"Бэкенд-разработчик", "frontend-developer":"Фронтенд-разработчик"}_ | Список предустановленных в систему должностей в формате JSON: ключи это идентификаторы должностей, а значения это текстовые описания. |

#### Конфигурация сервиса
| Поле | Описание |
| --- | --- |
| `empPerPosMin` | Минимальное количество сотрудников, которые будут созданы на каждую должность. |
| `empPerPosMax` | Максимальное количество сотрудников, которые будут созданы на каждую должность. |
| `activeUnitsMax` | Количество свободных штатных единиц, после которого прекращается увольнение сотрудников. |
| `fireDelaySec` | Промежуток времени в секундах между увольнениями сотрудников. |
| `pendingDelaySec` | Промежуток времени в секундах от регистрации нового сотрудника до закрытия его штатной единицы. |

#### Должности по-умолчанию
```json
{
  "backend-developer": "Бэкенд-разработчик",
  "design": "Дизайнер",
  "devops": "DevOps инженер",
  "frontend-developer": "Фронтенд-разработчик",
  "hr": "Сотрудник отдела кадров",
  "qa": "Инженер по тестированию",
  "teamlead": "Тимлид"
}
```

### II. Запустить сервис и вывести логи в нужный файл

Команда на запуск

`docker compose up -d` в репозитории сервиса

Вывод логов в отдельный файл

`docker compose logs -t > /path/to/logs.txt &` в том же репозитории
