<?php
if (!isset($_POST["json"])){
    die("No json exits...");
}

$request = json_decode($_POST["json"]);

$response = new STDClass();

$publicEnemyID;

$publicEnemyGold;
$publicEnemyLumber;
$publicEnemyMana;

$publicEnemyPeasant;
$publicEnemyKnight;
$publicEnemyArcher;
$publicEnemyMage;
$publicEnemyCatapult;


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
    case "update_resource":
        UpdateResource($request);
        return;
    case "upgrade_generator":
        UpgradeGenerator($request);
        return;
    case "find_new_opponent":
        FindNewOpponent($request);
        return;
    case "battle_opponent":
        BattleOpponent($request);
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

    $stmt = $conn->prepare("INSERT INTO users (email, username, hash, Gold, Lumber, Mana, Knight, Peasent, Archer, Mage, Catapult, goldIncome, lumberIncome, manaIncome, lastOnline, goldUpgradeCost, lumberUpgradeCost, manaUpgradeCost) VALUES (:email, :username, :hash, :Gold, :Lumber, :Mana, :Knight, :Peasent, :Archer, :Mage, :Catapult, :goldIncome, :lumberIncome, :manaIncome, :lastOnline, :goldUpgradeCost, :lumberUpgradeCost, :manaUpgradeCost)");
    $stmt->bindValue(":email",$request->email);
    $stmt->bindValue(":username", $request->username);
    $stmt->bindValue(":hash", $hash);
    $stmt->bindValue(":Gold", 0);
    $stmt->bindValue(":Lumber", 0);
    $stmt->bindValue(":Mana", 0);

    $stmt->bindValue(":Knight", 0);
    $stmt->bindValue(":Peasent", 0);
    $stmt->bindValue(":Archer", 0);
    $stmt->bindValue(":Mage", 0);
    $stmt->bindValue(":Catapult", 0);

    $stmt->bindValue(":goldIncome", 1);
    $stmt->bindValue(":lumberIncome", 1);
    $stmt->bindValue(":manaIncome", 1);
    $stmt->bindValue(":goldUpgradeCost", 5);
    $stmt->bindValue(":lumberUpgradeCost", 5);
    $stmt->bindValue(":manaUpgradeCost", 5);
    $stmt->bindValue(":lastOnline", time());

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

function GetRandomStringUniqid($length = 16){
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
    $stmt->bindValue(":Gold", $dbgold - $goldPrice);
    $stmt->bindValue(":Lumber", $dblumber - $lumberPrice);
    $stmt->bindValue(":Mana", $dbmana - $manaPrice);
    $stmt->bindValue(":token", $request->token);
    $stmt->execute();

    $response->peasant = $peasant + $request->peasantTroop;
    $response->knight = $knight + $request->knightTroop;
    $response->archer = $archer + $request->archerTroop;
    $response->mage = $mage + $request->mageTroop;
    $response->catapult = $catapult + $request->catapultTroop;
    $response->gold = $dbgold;
    $response->lumber = $dblumber;
    $response->mana = $dbmana;

    $response->serverMessage = "Troops bought!";
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

function UpdateResource($request){
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
    $lastOnline = $row["lastOnline"];

    $gold = $row["Gold"];
    $goldIncome = $row["goldIncome"];
    $lumber = $row["Lumber"];
    $lumberIncome = $row["lumberIncome"];
    $mana = $row["Mana"];
    $manaIncome = $row["manaIncome"];

    if ($lastOnline != null){
        $deltaTime = time() - $lastOnline;
        $tick = 5; // the amount of seconds 
        $ticksGainedWhileOffline = floor($deltaTime / $tick);
        
        if ($ticksGainedWhileOffline >= 72){
            $gold = $gold + $goldIncome * 72;
            $lumber = $lumber + $lumberIncome * 72;
            $mana = $mana + $manaIncome * 72;
            $ticksGainedWhileOffline = 0;
        }
        else{
            $gold = $gold + $goldIncome * $ticksGainedWhileOffline;
            $lumber = $lumber + $lumberIncome * $ticksGainedWhileOffline;
            $mana = $mana + $manaIncome * $ticksGainedWhileOffline;
            $ticksGainedWhileOffline = 0;
        }
    }
    else {
        $response->serverMessage = "last tick is null";
        echo(json_encode($response));
        return;
    }



    $stmt = $conn->prepare("UPDATE users SET Gold = :Gold, Lumber = :Lumber, Mana = :Mana, lastOnline = :lastOnline WHERE token = :token");
    $stmt->bindValue(":token", $request->token);
    $stmt->bindValue(":Gold", $gold);
    $stmt->bindValue(":Lumber", $lumber);
    $stmt->bindValue(":Mana", $mana);
    $stmt->bindValue(":lastOnline", time());
    $stmt->execute();

    //return the number of gold, lumber and mana in db
    
    $response->serverMessage = "Resource Update!";
    $response->lastOnlineTick = time();
    $response->goldIncome = $gold;
    $response->lumberIncome = $lumber;
    $response->manaIncome = $mana;
    echo(json_encode($response));
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

    $id = $row["id"];

    $dbgold = $row["Gold"];
    $dblumber = $row["Lumber"];
    $dbmana = $row["Mana"];
    //get the upgrade costs from the database
    $goldUpgradeCost = $row["goldUpgradeCost"];
    $lumberUpgradeCost = $row["lumberUpgradeCost"];
    $manaUpgradeCost = $row["manaUpgradeCost"];

    $goldIncome = $row["goldIncome"];
    $lumberIncome = $row["lumberIncome"];
    $manaIncome = $row["manaIncome"];


    //check type of gen
    if ($request->generatorType == "Gold"){
        if ($dbgold >= $goldUpgradeCost){
            $dbgold  = $dbgold - $goldUpgradeCost;
            $goldIncome = $goldIncome + 1;
            $goldUpgradeCost = $goldUpgradeCost * 2;
        }
    }
    if ($request->generatorType == "Lumber"){
        if ($dblumber >= $lumberUpgradeCost){
            $dblumber  = $dblumber - $lumberUpgradeCost;
            $lumberIncome = $lumberIncome + 1;
            $lumberUpgradeCost = $lumberUpgradeCost * 2;
        }
    }
    if ($request->generatorType == "Mana"){
        if ($dbmana >= $manaUpgradeCost){
            $dbmana  = $dbmana - $manaUpgradeCost;
            $manaIncome = $manaIncome + 1;
            $manaUpgradeCost = $manaUpgradeCost * 2;
        }
    }

    $stmt = $conn->prepare("UPDATE users SET Gold = :Gold, Lumber = :Lumber, Mana = :Mana, goldIncome = :goldIncome, lumberIncome = :lumberIncome, manaIncome = :manaIncome, goldUpgradeCost = :goldUpgradeCost, lumberUpgradeCost = :lumberUpgradeCost, manaUpgradeCost = :manaUpgradeCost WHERE token = :token");
    $stmt->bindValue(":Gold", $dbgold);
    $stmt->bindValue(":Lumber", $dblumber);
    $stmt->bindValue(":Mana", $dbmana);

    $stmt->bindValue(":goldUpgradeCost", $goldUpgradeCost);
    $stmt->bindValue(":lumberUpgradeCost", $lumberUpgradeCost);
    $stmt->bindValue(":manaUpgradeCost", $manaUpgradeCost);

    $stmt->bindValue(":goldIncome", $goldIncome);
    $stmt->bindValue(":lumberIncome", $lumberIncome);
    $stmt->bindValue(":manaIncome", $manaIncome);

    $stmt->bindValue(":token", $request->token);
    $stmt->execute();


    $response->goldIncome = $goldIncome;
    $response->lumberIncome = $lumberIncome;
    $response->manaIncome = $manaIncome;

    $response->goldUpgradePrice = $goldUpgradeCost;
    $response->lumberUpgradePrice = $lumberUpgradeCost;
    $response->manaUpgradePrice = $manaUpgradeCost;

    $response->gold = $dbgold;
    $response->lumber = $dblumber;
    $response->mana = $dbmana;

    $response->serverMessage = "Upgrade!";
    echo(json_encode($response));
}

function FindNewOpponent($request){
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
    $username = $row["username"];

    //$sql = "SELECT id, username, Gold, Lumber, Mana, Peasent, Knight, Archer, Mage, Catapult FROM users";
    //$result = $conn->query($sql);
    
    
    $sql = "SELECT id, username, Gold, Lumber, Mana, Peasent, Knight, Archer, Mage, Catapult, hasBeenAttacked FROM users ORDER BY RAND() LIMIT 1";
    $stmt = $conn->prepare($sql);
    $stmt->execute();
    $row = $stmt->fetch(PDO:: FETCH_ASSOC); 

    // Check if the query was successful
    if ($stmt) {
        // Fetch the random ID
        $randomID = $row['id'];
        $publicEnemyID = $randomID;
        if ($randomID == $request->latestOpponentID && $randomID == $id && $row['hasBeenAttacked'] == 1){
            FindNewOpponent($request);
            return;
        }
        $IDName = $row['username'];
        $enemyGold = $row['Gold'];
        $publicEnemyGold = $enemyGold / 10;
        $enemyLumber = $row['Lumber'];
        $publicEnemyLumber = $enemyLumber / 10;
        $enemyMana = $row['Mana'];
        $publicEnemyMana = $enemyMana / 10;
        $enemyPeasant = $row['Peasent'];
        $publicEnemyPeasant = $enemyPeasant / 2;
        $enemyKnight = $row['Knight'];
        $publicEnemyKnight = $enemyKnight / 2;
        $enemyArcher = $row['Archer'];
        $publicEnemyArcher = $enemyArcher / 2;
        $enemyMage = $row['Mage'];
        $publicEnemyMage = $enemyMage / 2;
        $enemyCatapult = $row['Catapult'];
        $publicEnemyCatapult = $enemyCatapult / 2;
    }

    $response->serverMessage = "Find New Opponent.";
    $response->latestOpponentID = $randomID;
    $response->opponentName = $IDName;
    $response->opponentGold = $publicEnemyGold;
    $response->opponentLumber = $publicEnemyLumber;
    $response->opponentMana = $publicEnemyMana;
    
    $response->opponentPeasant = $publicEnemyPeasant;
    $response->opponentKnight = $publicEnemyKnight;
    $response->opponentArcher = $publicEnemyArcher;
    $response->opponentMage = $publicEnemyMage;
    $response->opponentCatapult = $publicEnemyCatapult;
    
    echo(json_encode($response));
}

function BattleOpponent($request){
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
    $Peasant = $row['Peasant'];
    $Knight = $row['Knight'];
    $Archer = $row['Archer'];
    $Mage = $row['Mage'];
    $Catapult = $row['Catapult'];

    
    $opstmt = $conn->prepare("SELECT * FROM users WHERE id = :id");
    $opstmt->vindValue(":id", $publicEnemyID);
    $opstmt->execute();

    $oprow = $opstmt->fetch(PDO:: FETCH_ASSOC);
    $enemyId = $oprow["id"];
    $enemyPeasant = $oprow['Peasant'];
    $enemyKnight = $oprow['Knight'];
    $enemyArcher = $oprow['Archer'];
    $enemyMage = $oprow['Mage'];
    $enemyCatapult = $oprow['Catapult'];

    $totalOwnPower = $Peasant * 1 + $Knight * 2 + $Archer * 2 + $Mage * 3 + $Catapult * 4;
    $totalEnemyPower = $enemyPeasant * 1 + $enemyKnight * 2 + $enemyArcher * 2 + $enemyMage * 3 + $enemyCatapult * 4;

    if ($totalOwnPower > $totalEnemyPower){
        //Add recources self
        //Remove troops self
        $stmt = $conn->prepare("UPDATE users SET Gold = :Gold, Lumber = :Lumber, Mana = :Mana, Peasant = :Peasant, Knight = :Knight, Archer = :Archer, Mage = :Mage, Catapult = :Catapult WHERE token = :token");
        $stmt->bindValue(":Gold", $row['Gold'] + $publicEnemyGold);
        $stmt->bindValue(":Lumber", $row['Lumber'] + $publicEnemyLumber);
        $stmt->bindValue(":Mana", $row['Mana'] + $publicEnemyMana);

        $stmt->bindValue(":Peasant", 0);
        $stmt->bindValue(":Knight", 0);
        $stmt->bindValue(":Archer", 0);
        $stmt->bindValue(":Mage", 0);
        $stmt->bindValue(":Catapult", 0);

        $stmt->bindValue(":token", $request->token);
        $stmt->execute();
        
        //Add recources enemy
        //Remove troops enemy
        $opstmt = $conn->prepare("UPDATE users SET Gold = :Gold, Lumber = :Lumber, Mana = :Mana, Peasant = :Peasant, Knight = :Knight, Archer = :Archer, Mage = :Mage, Catapult = :Catapult, hasBeenAttacked = :hasBeenAttacked WHERE id = :id");
        $opstmt->bindValue(":Gold", $oprow['Gold'] - $publicEnemyGold);
        $opstmt->bindValue(":Lumber", $oprow['Lumber'] - $publicEnemyLumber);
        $opstmt->bindValue(":Mana", $oprow['Mana'] - $publicEnemyMana);

        $opstmt->bindValue(":Peasant",$enemyPeasant - $publicEnemyPeasant);
        $opstmt->bindValue(":Knight", $enemyKnight - $publicEnemyKnight);
        $opstmt->bindValue(":Archer", $enemyArcher - $publicEnemyArcher);
        $opstmt->bindValue(":Mage", $enemyMage - $publicEnemyMage);
        $opstmt->bindValue(":Catapult", $enemyCatapult - $publicEnemyCatapult);

        //set has been attacked bool
        $opstmt->bindValue(":hasBeenAttacked", 1);
        $opstmt->bindValue(":id", $publicEnemyID);
        $opstmt->execute();
        
        $response->serverMessage = "Battle Won!";

        $response->gold = $row['Gold'] + $publicEnemyGold;
        $response->lumber = $row['Lumber'] + $publicEnemyLumber;
        $response->mana = $row['Mana'] + $publicEnemyMana;

        $response->peasant = 0;
        $response->knight = 0;
        $response->archer = 0;
        $response->mage = 0;
        $response->catapult = 0;
    }
    else{
        //Remove own troops 
        $stmt = $conn->prepare("UPDATE users SET Peasant = :Peasant, Knight = :Knight, Archer = :Archer, Mage = :Mage, Catapult = :Catapult WHERE token = :token");

        $stmt->bindValue(":Peasant", 0);
        $stmt->bindValue(":Knight", 0);
        $stmt->bindValue(":Archer", 0);
        $stmt->bindValue(":Mage", 0);
        $stmt->bindValue(":Catapult", 0);

        $stmt->bindValue(":token", $request->token);
        $stmt->execute();
        

        //set has been attacked bool
        $opstmt = $conn->prepare("UPDATE users SET hasBeenAttacked = :hasBeenAttacked WHERE id = :id");
        $opstmt->bindValue(":hasBeenAttacked", 1);
        $opstmt->bindValue(":id", $publicEnemyID);
        $opstmt->execute();

        $response->serverMessage = "Battle Lost.";

        $response->peasant = 0;
        $response->knight = 0;
        $response->archer = 0;
        $response->mage = 0;
        $response->catapult = 0;
    }

    echo(json_encode($response));
}
?>