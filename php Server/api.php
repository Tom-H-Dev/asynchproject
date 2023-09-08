<?php
if (!isset($_POST["json"])){
    die("No json exits...");
}

$request = json_decode($_POST["json"]);

$response = new STDClass();

switch($request->action){
    case "create_account":
        CreateAccount($request);
        return;
    case "login_request":
        Login($request);
        return;
    default:
        $response->serverMessage = "No valid server action";
        echo(json_encode($response));
}

function CreateAccount($request){
    global $response;
    require_once("connect.php");
    //Email
    $stmt = $conn->prepare("SELECT id from users WHERE email = :email");
    $stmt->bindParam(":email", $input);
    $stmt->execute();
    if ($stmt->fetchColumn() > 0){
        $response->errorMEssage = "Email already exists";
        echo(json_encode($response));
        return;     
    }

    //The username
    $stmt = $conn->prepare("SELECT id from users WHERE username = :username");
    $stmt->bindParam(":username", $input);
    $stmt->execute();
    if ($stmt->fetchColumn() > 0){
        $response->errorMEssage = "Username already exists";
        echo(json_encode($response));
        return;     
    }

    //Password
    $hash = password_hash($request->password, PASSWORD_DEFAULT);

    $stmt = $conn->prepare("INSERT INTO users (email, username, hash) VALUES (:email, :username, :hash)");
    $stmt->bindValue(":email",$request->email);
    $stmt->bindValue(":username", $request->username);
    $stmt->bindValue(":hash", $hash);

    $stmt->execute();

    $response->serverMessage = "Succes";
    echo(json_encode($response));
}

function Login($request){
    global $response;
    require_once("connect.php");
    $stmt = $conn->prepare("SELECT * from users WHERE email = :email");
    $stmt->bindParam(":email", $response->email);
    $stmt->execute();
    if ($stmt->fetchColumn() == 0){
        $response->serverMessage = "Email_not_found";
        echo(json_encode($response));
        return;     
    }

    
}

?>