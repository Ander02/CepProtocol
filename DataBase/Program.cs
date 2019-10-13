using System;
using DataBase;

namespace DataBase
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            DBWriter DB = new DBWriter("C:\\Users\\Gu\\Desktop\\Gustavo\\USP\\6 semestre\\Redes\\EP\\CepProtocol\\DataBase\\Server_DB.txt");
            String [] x= {"a","1"};
            Console.WriteLine(DB.insertLine(x));
            DB.ShowBuffer();
        }
    }
}
