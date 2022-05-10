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
        DeleteTable();
        CreateTable();
        /*CreateUser("eyal", "123456");
        CreateUser("yon", "0000");
        CreateUser("net", "666");

        AddMove(new Move(new Point(1, 2), new Point(3, 4)), 2);
        SetCurrentGameToLast(2);
        AddMove(new Move(new Point(5, 6), new Point(7, 8)), 2);
        PrintTable();*/
    }
    public bool ComparePassword(string triedPassword, int id)
    {
        string hashedTriedPassword = Hash(triedPassword);

        string statement = String.Format("SELECT * FROM chessUsers WHERE id={0}", id);
        SQLiteDataReader reader = ExecuteReadQuery(statement);
        string password = " ";
        while (reader.Read())
        {
            password = reader.GetString(2);
        }
        return hashedTriedPassword == password;
    }
    public void CreateUser(string user, string password)
    {
        //create new user
        string statement = String.Format(@"INSERT INTO chessUsers(username, password, currentGameMoves) 
                                        VALUES('{0}','{1}','{2}')", user, Hash(password), "", "");
        ExecuteVoidQuery(statement);
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
        //add the move to the move list in the game field (string manipulation)
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

    private void CreateTable()
    {
        string statement = @"CREATE TABLE chessUsers(
                            id INTEGER PRIMARY KEY,
                            username TEXT, 
                            password TEXT, 
                            currentGameMoves TEXT, 
                            lastGameMoves TEXT)";
        ExecuteVoidQuery(statement);
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
