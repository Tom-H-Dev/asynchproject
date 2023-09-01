<?php
$json = $_POST["json"];
$request = json_decode($json);

//request die heeft nu een array die fruit heeft echo een random stuk fruit uit array
echo($request->fruit[rand(0,2)])
?>