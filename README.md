Краткая информация:<br>
/api/authenticate/register - регистрация пользователя, сделал для тестирования, принимает json email и password;<br>
/api/authenticate/login - аутентификация пользователя, принимает json email и password, так же принимает вместо email phone, если он задан у пользователя;<br>
/api/authenticate/logout - удаляет куки аутентификации;<br>
/api/file/upload - загрузка файла;<br>
/api/file/downloading/[id файла] - скачивает указанный файл по его id;<br>
/api/file/[email пользователя] - выводит список файлов, загруженных текущим пользователем;
