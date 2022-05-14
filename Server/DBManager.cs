using System;
using System.Text;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Linq;
using System.Threading.Tasks;
public class DBManager
{
    private SQLiteConnection connection;

    public DBManager()
    {
        //setup database
        string cs = @"URI=file:userData.db";
        connection = new SQLiteConnection(cs);
        connection.Open();
        CreateTable();
    }
    private bool ComparePassword(string hashedTriedPassword, int id)
    {
        string statement = string.Format("SELECT * FROM chessUsers WHERE id={0}", id);
        SQLiteDataReader reader = ExecuteReadQuery(statement);
        string password = " ";
        while (reader.Read())
        {
            password = reader.GetString(2);
        }
        return hashedTriedPassword == password;
    }
    public bool CreateUser(string user, string password)
    {
        //create new user and returns if the user is taken
        string statement = string.Format("SELECT COUNT(1) FROM chessUsers WHERE username='{0}'", user);
        SQLiteDataReader reader = ExecuteReadQuery(statement);

        int count = 0;
        while (reader.Read())
            count = reader.GetInt32(0);
        if (count > 0)
            return false;

        statement = string.Format(@"INSERT INTO chessUsers(username, password, currentGameMoves) 
                                        VALUES('{0}','{1}','{2}')", user, password, "");
        ExecuteVoidQuery(statement);
        return true;
    }
    public int GetUserID(string username, string password)
    {
        //returns the user id
        //returns 0 if couldnt found
        //returns -1 if wrong password
        string statement = string.Format("SELECT * FROM chessUsers WHERE username='{0}'", username);
        SQLiteDataReader reader = ExecuteReadQuery(statement);
        int id = 0;
        while (reader.Read())
            id = reader.GetInt32(0);
        if (id != 0)
            if (ComparePassword(password, id))
                return id;
            else
                return -1;
        return 0;
    }
    public void AddMove(Move move, int id)
    {
        //Add the move to the move list in the game field (string manipulation)
        string statement = String.Format("SELECT * FROM chessUsers WHERE id={0}", id);
        SQLiteDataReader reader = ExecuteReadQuery(statement);

        while (reader.Read())
        {
            string moves = reader.GetString(3);
            moves += move.ToString() + ";";
            statement = String.Format("UPDATE chessUsers SET currentGameMoves='{0}' WHERE id='{1}'", moves, id);
            ExecuteVoidQuery(statement);
        }
    }
    public List<Move> GetGameMoves(int id, bool getCurrentOrLastGame)
    {
        string statement = string.Format("SELECT * FROM chessUsers WHERE id={0}", id);
        SQLiteDataReader reader = ExecuteReadQuery(statement);
        List<Move> moves = new List<Move>();
        string stringMoves = "";
        while (reader.Read())
            stringMoves = reader.GetString(getCurrentOrLastGame ? 3 : 4);
        string moveStr = "";
        foreach (char ch in stringMoves)
            if (ch == ';')
            {
                string[] arr = moveStr.Split('_');
                moves.Add(new Move(arr[0], arr[1]));
                moveStr = "";
            }
            else
                moveStr += ch;
        return moves;
    }
    public void SetCurrentGameToLast(int id)
    {
        string statement = string.Format("SELECT * FROM chessUsers WHERE id={0}", id);
        SQLiteDataReader reader = ExecuteReadQuery(statement);
        string currentGame = "";
        while (reader.Read())
            currentGame = reader.GetString(3);

        statement = String.Format("UPDATE chessUsers SET currentGameMoves='{0}' WHERE id='{1}'", "", id);
        ExecuteVoidQuery(statement);
        statement = String.Format("UPDATE chessUsers SET lastGameMoves='{0}' WHERE id='{1}'", currentGame, id);
        ExecuteVoidQuery(statement);
    }
    private bool IsTableExist()
    {
        string statment = @"SELECT COUNT(1) FROM sqlite_schema WHERE type IN ('table') AND name='chessUsers'";
        SQLiteDataReader reader = ExecuteReadQuery(statment);
        while (reader.Read())
            return reader.GetInt32(0) > 0;
        return false;
    }
    private void CreateTable()
    { 
        if (!IsTableExist())
        {
            string statement = @"CREATE TABLE chessUsers(
                            id INTEGER PRIMARY KEY,
                            username TEXT, 
                            password TEXT, 
                            currentGameMoves TEXT, 
                            lastGameMoves TEXT)";
            ExecuteVoidQuery(statement);
        }
    }
    private void DeleteTable()
    {
        string statement = "DROP TABLE IF EXISTS chessUsers";
        ExecuteVoidQuery(statement);
    }
    private void ExecuteVoidQuery(string statement)
    {
        SQLiteCommand command = new SQLiteCommand(statement, connection);
        command.ExecuteNonQuery();
    }
    private SQLiteDataReader ExecuteReadQuery(string statement)
    {
        SQLiteCommand command = new SQLiteCommand(statement, connection);
        SQLiteDataReader reader = command.ExecuteReader();
        return reader;
    }
    private static string Hash(string password)
    {
        var sha256 = SHA256.Create();
        byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        string hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        return hash;
    }
    private void PrintTable()
    {
        string statement = "SELECT * FROM chessUsers LIMIT 5";
        SQLiteDataReader reader = ExecuteReadQuery(statement);
        while (reader.Read())
        {
            Console.WriteLine($"{reader.GetInt32(0)} {reader.GetString(1)} {reader.GetString(2)} {reader.GetString(3)}");
        }
    }
}
