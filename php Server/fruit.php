<?php
require_once("connect.php");

$input = "ballen";

$stmt = $conn->prepare("SELECT * from fruit where name = :name");
$stmt->bindParam(":name", $input);
$stmt->execute();

$result = $stmt->setFetchMode(PDO::FETCH_ASSOC);
foreach($stmt->fetchAll() as $key => $value){
    echo($value["name"] . "<br>");
}
?>