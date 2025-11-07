using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Security.Cryptography;
using System.Runtime.InteropServices.WindowsRuntime;

public class Database : MonoBehaviour
{
    // Variables
    private readonly string dbName = "URI=file:UserData.db";
    // GameObjects

    // GameObject accessors

    // Other files

    void Awake()
    {
        CreateDataBase();
        //DisplayUserData();
    }

    private void CreateDataBase()
    {
        using var connection = new SqliteConnection(dbName);
        connection.Open();
        using var command = connection.CreateCommand();
        // New table created if doesn't already exist
        command.CommandText = "CREATE TABLE IF NOT EXISTS userData (name VARCHAR(30), password VARCHAR(64), salt VARCHAR(20), ID INT);";
        command.ExecuteNonQuery();
    }

    [ContextMenu("Diplay user data")]
    public void DisplayUserData()
    {
        using var connection = new SqliteConnection(dbName);
        connection.Open();
        using var command = connection.CreateCommand();
        // Displays user data
        // Name: ExName  Password: ExPassword  Salt: ExSalt  ID: ExID
        command.CommandText = "SELECT * FROM userData;";
        using IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            Debug.Log("Name: " + reader["name"] + "\tPassword: " + reader["password"] + "\tSalt: " + reader["salt"] + "\tID: " + reader["ID"]);
        }
    }

    public void AddUserData(string username, string password, string salt, int ID)
    {
        using var connection = new SqliteConnection(dbName);
        connection.Open();
        using var command = connection.CreateCommand();
        // Inserts user data into table
        command.CommandText = "INSERT INTO userData (name, password, salt, ID) VALUES ('" + username + "','" + password + "','" + salt + "','" + ID + "');";
        command.ExecuteNonQuery();
    }

    public bool CheckForExistingData(string data, string dataType)
    {
        using var connection = new SqliteConnection(dbName);
        connection.Open();
        using var command = connection.CreateCommand();
        // Depending on data type, checks if data with same name or ID exists
        if (dataType == "username") command.CommandText = "SELECT COUNT(*) FROM userData WHERE name='" + data + "';";
        else command.CommandText = "SELECT COUNT(*) FROM userData WHERE ID='" + data + "';";

        int dataCount = Convert.ToInt32(command.ExecuteScalar());

        if (dataCount == 1) return true;
        else return false;
    }

    public string FetchSalt(string username)
    {
        using var connection = new SqliteConnection(dbName);
        connection.Open();
        using var command = connection.CreateCommand();
        // If username exists, fetch the user's salt
        bool usernameExists = CheckForExistingData(username, "username");
        if (usernameExists)
        {
            command.CommandText = "SELECT salt FROM userData WHERE name='" + username + "';";
            return Convert.ToString(command.ExecuteScalar());
        }
        else return null;
    }

    public int CompareHashedPasswords(string hashedPassword)
    {
        using var connection = new SqliteConnection(dbName);
        // Checks if hashed password already exists in the table
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM userData WHERE password='" + hashedPassword + "';";
        return Convert.ToInt32(command.ExecuteScalar());
    }
}