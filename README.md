## POS сервер  

Промежуточное  ПО  для  подключения  принтеров  и торгового оборудования  к  учетным  программам, работающим  в онлайн режиме (через браузер). 
Разработано для проекта <https://github.com/leon-mbs/zstore> но может использоватся  и с другмими системами.  
Сервер  реализует  API к которому учетная програма  обращается  с браузера с  помощью javascript запроса как  к  обычному  веб серверу.   

Готовой реадизацией является  режим  принт-сервера для работы  с  чековыми  принтерами  и принтерами этикеток подключаемыми  по  USB. 
Остальные  режимы реадлизуются  кастомно  в  зависимлсти от типа  оборудования  

Адрес сервера задается  в  конфигурационных файлах.  По умолчанию http://127.0.0.1:8080  
проверка  сервера с  браузера  http://127.0.0.1:8080/check  










