<?php
// Указываем браузеру/приложению, что это JSON
header('Content-Type: application/json');

// Список поддерживаемых моделей
$supported_models = [
    'iPhone4,1', 'iPad2,5', 'iPad2,6', 'iPad2,7', 
    'iPad2,2', 'iPad2,3', 'iPad2,4', 'iPad3,1', 
    'iPad3,2', 'iPad3,3', 'iPod5,1'
];

// Список поддерживаемых версий iOS
$supported_ios = ['9.3.5', '9.3.6'];

// Получаем параметры из GET-запроса
$current_model = $_GET['model'] ?? '';
$current_ios = $_GET['ios'] ?? ''; // Получаем версию iOS

// Проверка: модель должна быть в списке И версия должна быть 9.3.5 или 9.3.6
$is_supported = in_array($current_model, $supported_models) && in_array($current_ios, $supported_ios);

// Формируем ответ
echo json_encode([
    "supported" => $is_supported,
    "received_model" => $current_model,
    "received_ios" => $current_ios
]);
?>