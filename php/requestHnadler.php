<?php
    $servername="localhost";
    $username = "root";
    $password= "";
    $dbname="unityusers";

    $conn = new mysqli($servername,$username,$password,$dbname);

    if ($conn -> connect_error){
        die("Connection failed: " . $conn->connect_error);
    }

    if (isset($_POST["register"])){
        $unityUsername = $_POST["username"];
        $unityPassword = $_POST["password"];

        $stmt = $conn->prepare("INSERT INTO users (username, password) VALUES (?, ?)");
        $stmt->bind_param("ss", $unityUsername, $unityPassword);
        
        if ($stmt->execute()){
            echo "true";
        } else {
            echo "false";
        }

        $stmt->close();
        $conn->close();
    }

    else if (isset($_POST["login"])) {
        $unityUsername = $_POST["username"];
        $unityPassword = $_POST["password"];
    
        $stmt = $conn->prepare("SELECT * FROM users WHERE username = ? AND password = ?");
        $stmt->bind_param("ss", $unityUsername, $unityPassword);
        $stmt->execute();
        $result = $stmt->get_result();
        $stmt->close();
    
        if ($result->num_rows >= 1) {
            echo "true";
        } else {
            echo "false";
        }
    }
?>