using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
public class DBManager
{
    private SQLiteConnection connection;
    public DBManager()
    {
        //setup database
        string cs = @"URI=file:userData.db";
        this.connection = new SQLiteConnection(cs);
        connection.Open();

        //DeleteTable();
        //CreateTable();
        CreateUser("eyal", "123456");
        CreateUser("yon", "0000");
        CreateUser("net", "666");

        AddMove(new Move(new Point(5, 2), new Point(1, 2)), 2);
        PrintTable();
    }
    public void PrintTable()
    {
        string statement = "SELECT * FROM chessUsers LIMIT 5";
        SQLiteDataReader reader = ExecuteReadQuery(statement);
        while (reader.Read())
        {
            Console.WriteLine($"{reader.GetInt32(0)} {reader.GetString(1)} {reader.GetString(2)} {reader.GetString(3)}");
        }
    }
    public void CreateTable()
    {
        string statement = @"CREATE TABLE chessUsers(id INTEGER PRIMARY KEY,
            username TEXT, password TEXT, currentGameMoves TEXT)";
        ExecuteVoidQuery(statement);
    }
    public void DeleteTable()
    {
        string statement = "DROP TABLE IF EXISTS chessUsers";
        ExecuteVoidQuery(statement);
    }
    public void CreateUser(string user, string password)
    {
        string statement = String.Format(@"INSERT INTO chessUsers(username, password, currentGameMoves) 
                                        VALUES('{0}','{1}','{2}')", user, password, "");
        ExecuteVoidQuery(statement);
    }
    public void AddMove(Move move, int id)
    {
        //add the move to the move list in the game field (string manipulation)
        string statement = String.Format("SELECT * FROM chessUsers WHERE id={0}", id);
        Console.WriteLine(statement);

        SQLiteDataReader reader = ExecuteReadQuery(statement);

        while (reader.Read())
        {
            string moves = reader.GetString(3);
            moves += ";" + move.ToString();
            statement = String.Format("UPDATE chessUsers SET currentGameMoves='{0}' WHERE id='{1}'", moves, id);
            Console.WriteLine(statement);
            ExecuteVoidQuery(statement);
            //Console.WriteLine($"{reader.GetInt32(0)} {reader.GetString(1)} {reader.GetString(2)} {reader.GetString(3)}");
        }
    }

    private void ExecuteVoidQuery(string statement)
    {
        SQLiteCommand command = new SQLiteCommand(statement, connection);
        command.ExecuteNonQuery();
    }
    private SQLiteDataReader ExecuteReadQuery(string statement)
    {
        SQLiteCommand command = new SQLiteCommand(statement, connection);
        SQLiteDataReader reader= command.ExecuteReader();
        return reader;
    }
}
