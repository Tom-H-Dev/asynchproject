<?php
$json = $_POST["json"];
$request = json_decode($json);

if ($request->action == "get_fruit"){
    $fruit= ["appel", "peer", "aardbei", "kers"];
    $response = new stdClass();
    $response->fruit = $fruit;
    $json = json_encode($response);
    echo($json);
}

?>