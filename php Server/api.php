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
    case "game_action":
        GameAction($request);
        return;
    case "logout_action":
        LogOut($request);
        return;
    case "Buy_Troops_Request":
        BuyTroop($request);
        return;
    case "Resource_Display":
        DisplayInfo($request);
        return;
    case "upgrade_gold_mine":
        UpgradeGenerator($request);
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

    $stmt = $conn->prepare("INSERT INTO users (email, username, hash, Gold, Lumber, Mana, Knight, Peasent, Archer, Mage, Catapult, goldIncome, lumberIncome, manaIncome) VALUES (:email, :username, :hash, :Gold, :Lumber, :Mana, :Knight, :Peasent, :Archer, :Mage, :Catapult, :goldIncome, :lumberIncome, :manaIncome)");
    $stmt->bindValue(":email",$request->email);
    $stmt->bindValue(":username", $request->username);
    $stmt->bindValue(":hash", $hash);
    $stmt->bindValue(":Gold", 5000);
    $stmt->bindValue(":Lumber", 5000);
    $stmt->bindValue(":Mana", 5000);

    $stmt->bindValue(":Knight", 0);
    $stmt->bindValue(":Peasent", 0);
    $stmt->bindValue(":Archer", 0);
    $stmt->bindValue(":Mage", 0);
    $stmt->bindValue(":Catapult", 0);

    $stmt->bindValue(":goldIncome", 1);
    $stmt->bindValue(":lumberIncome", 1);
    $stmt->bindValue(":manaIncome", 1);

    $stmt->execute();

    $response->serverMessage = "Succes";
    echo(json_encode($response));
}

function Login($request){
    global $response;
    require_once("connect.php");
    $stmt = $conn->prepare("SELECT * from users WHERE email = :email");
    $stmt->bindParam(":email", $request->email);
    $stmt->execute();

    $row = $stmt->fetch(PDO:: FETCH_ASSOC);
    if ($row == false){
        $response->serverMessage = "Email_not_found";
        echo(json_encode($response));
        return;     
    }
    $id = $row["id"];
    if(!password_verify($request->password, $row["hash"])){
        $response->serverMessage = "login_failed";
        echo(json_encode($response));
        return; 
    }
    $token = GetRandomStringUniqid(32);
    $stmt = $conn->prepare("UPDATE users SET token = :token WHERE id = :id");
    $stmt->bindValue(":token", $token);
    $stmt->bindValue(":id", $id);
    $stmt->execute();
    $response->token = $token;
    $response->serverMessage = "Succes";
    echo(json_encode($response));
}

function GetRandomStringUniqid($length = 16)
{
    $string = uniqid(rand());
    $randomString = substr($string, 0, $length);
    return $randomString;
}

function GameAction($request){
    global $response;
    require_once("connect.php");

    $stmt = $conn->prepare("SELECT * FROM users WHERE token = :token");
    $stmt->bindValue(":token", $request->token);
    $stmt->execute();
    $row = $stmt->fetch(PDO:: FETCH_ASSOC);
    if ($row == null){
        $response->serverMessage = "token not found";
        echo(json_encode($response));
        return;
    }
    //Voer hier alle gameplay acties uit voor de user
    $response->serverMessage = $row["username"];
    echo(json_encode($response));
}

function LogOut($request){
    global $response;
    require_once("connect.php");

    $stmt = $conn->prepare("SELECT * FROM users WHERE token = :token");
    $stmt->bindValue(":token", $request->token);
    $stmt->execute();
    $row = $stmt->fetch(PDO:: FETCH_ASSOC);
    if ($row == null){
        $response->serverMessage = "token not found";
        echo(json_encode($response));
        return;
    }
    $id = $row["id"];
    $stmt = $conn->prepare("UPDATE users SET token = null WHERE id = :id");
    $stmt->bindValue(":id", $id);
    $stmt->execute();

    $response->serverMessage = "Succes";
    echo(json_encode($response));
}

function BuyTroop($request){
    global $response;
    require_once("connect.php");

    $stmt = $conn->prepare("SELECT * FROM users WHERE token = :token");
    $stmt->bindValue(":token", $request->token);
    $stmt->execute();


    $row = $stmt->fetch(PDO:: FETCH_ASSOC);
    if ($row == null){
        $response->serverMessage = "token not found";
        echo(json_encode($response));
        return;
    }


    $id = $row["id"];
    $dbgold = $row["Gold"];
    $dblumber = $row["Lumber"];
    $dbmana = $row["Mana"];

    $peasantPrice = [5,5,0];
    $knightPrices = [15,5,0];
    $archerPrice = [10,15,5];
    $magePrice = [20,0,20];
    $catapultPrice = [50,50,10];


    $goldPrice = $peasantPrice[0] * $request->peasantTroop + $knightPrices[0] * $request->knightTroop + $archerPrice[0] * $request->archerTroop + $magePrice[0] * $request->mageTroop + $catapultPrice[0] * $request->catapultTroop;
    $lumberPrice = $peasantPrice[1] * $request->peasantTroop + $knightPrices[1] * $request->knightTroop + $archerPrice[1] * $request->archerTroop + $magePrice[1] * $request->mageTroop + $catapultPrice[1] * $request->catapultTroop;
    $manaPrice = $peasantPrice[2] * $request->peasantTroop + $knightPrices[2] * $request->knightTroop + $archerPrice[2] * $request->archerTroop + $magePrice[2] * $request->mageTroop + $catapultPrice[2] * $request->catapultTroop;


    if ($dbgold < $goldPrice){
        $response->serverMessage = "More Gold Is Required";
        $response->noresource = "More Gold Is Required";
        echo(json_encode($response));
        return;
    }
    if ($dblumber < $lumberPrice){
        $response->serverMessage = "More Lumber Is Required";
        $response->noresource = "More Gold Is Required";
        echo(json_encode($response));
        return;
    }
    if ($dbmana < $manaPrice){
        $response->serverMessage = "More Lumber Is Required";
        $response->noresource = "More Gold Is Required";
        echo(json_encode($response));
        return;
    }

    $peasant = $row["Peasent"];
    $knight = $row["Knight"];
    $archer = $row["Archer"];
    $mage = $row["Mage"];
    $catapult = $row["Catapult"];


    $stmt = $conn->prepare("UPDATE users SET Peasent = :Peasent, Knight = :Knight, Archer = :Archer, Mage = :Mage, Catapult = :Catapult WHERE token = :token");
    $stmt->bindValue(":Peasent", $peasant + $request->peasantTroop);
    $stmt->bindValue(":Knight", $knight + $request->knightTroop);
    $stmt->bindValue(":Archer", $archer + $request->archerTroop);
    $stmt->bindValue(":Mage", $mage + $request->mageTroop);
    $stmt->bindValue(":Catapult", $catapult + $request->catapultTroop);
    $stmt->bindValue(":token", $request->token);
    $stmt->execute();
    
    $stmt = $conn->prepare("UPDATE users SET Gold = :Gold, Lumber = :Lumber, Mana = :Mana WHERE token = :token");
    $stmt->bindValue(":Gold", $dbgold - $goldCost);
    $stmt->bindValue(":Lumber", $dblumber - $lumberCost);
    $stmt->bindValue(":Mana", $dbmana - $manaCost);
    $stmt->bindValue(":token", $request->token);
    $stmt->execute();

    $response->peasant = $peasant;
    $response->knight = $knight;
    $response->archer = $archer;
    $response->mage = $mage;
    $response->catapult = $catapult;
    $response->gold = $dbgold;
    $response->lumber = $dblumber;
    $response->mana = $dbmana;

    $response->serverMessage = "Troops bought!";
    $response->debug = $dbgold . " " . $goldCost;
    echo(json_encode($response));
}

function DisplayInfo($request){
    global $response;
    require_once("connect.php");

    $stmt = $conn->prepare("SELECT * FROM users WHERE token = :token");
    $stmt->bindValue(":token", $request->token);
    $stmt->execute();

    $row = $stmt->fetch(PDO:: FETCH_ASSOC);
    if ($row == null){
        $response->serverMessage = "token not found";
        echo(json_encode($response));
        return;
    }

    $id = $row["id"];
    $dbgold = $row["Gold"];
    $dblumber = $row["Lumber"];
    $dbmana = $row["Mana"];

    $peasant = $row["Peasent"];
    $knight = $row["Knight"];
    $archer = $row["Archer"];
    $mage = $row["Mage"];
    $catapult = $row["Catapult"];

    $response->peasant = $peasant;
    $response->knight = $knight;
    $response->archer = $archer;
    $response->mage = $mage;
    $response->catapult = $catapult;
    $response->gold = $dbgold;
    $response->lumber = $dblumber;
    $response->mana = $dbmana;

    $response->serverMessage = "Info displayed";
    echo(json_encode($response));
}


function CalculateAmountTimeOffline(){
    //$lastupdate = //time from db;
    $deltaTime = time() - $lastupdate;
    $tick = 5; // the amount of seconds 
    $ticksGainedWhileOffline = floor($deltaTime / $tick);
    $newLastUpdate = $lastupdate + ($tick * $ticksGainedWhileOffline);
}

function UpgradeGenerator($request){
    global $response;
    require_once("connect.php");

    $stmt = $conn->prepare("SELECT * FROM users WHERE token = :token");
    $stmt->bindValue(":token", $request->token);
    $stmt->execute();

    $row = $stmt->fetch(PDO:: FETCH_ASSOC);
    if ($row == null){
        $response->serverMessage = "token not found";
        echo(json_encode($response));
        return;
    }

    $upgradePrice = [0];
    $goldIncome = $row["goldIncome"];
    $goldCost = $row["goldUpgradeCost"]
    $id = $row["id"];

    $stmt = $conn->prepare("UPDATE users SET token = null WHERE id = :id, goldIncome = :goldIncome");
    $stmt->bindValue(":id", $id);
    $stmt->bindValue(":goldIncome", $goldIncome);
    $stmt->execute();

    $response->serverMessage = "Resource Gained";
    $response->goldIncome = $goldIncome;
    echo(json_encode($response));
}
?>