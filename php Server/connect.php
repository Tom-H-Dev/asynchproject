<?php

$dbServerName = "127.0.0.1";
$dbName = "example";
$dbUsername = "root"; 
$dbPassword = "";

    $conn = new PDO("mysql:host=$dbServerName;dbname=$dbName", $dbUsername, $dbPassword);

    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    //echo "Connected successfully";
?>