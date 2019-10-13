using System;
using System.Collections.Generic;
using System.Linq;

namespace DataBase
{

    public class DBWriter
    {
        private System.IO.StreamReader File;
        private String FilePath;
        private List<List<String>> DBBuffer;
        private int LastLineIndex;

        /// <summary>
        /// Construtor da classe que gerencia o txt. Ele inicia carregando o arquivo
        /// no caminho informado no parametro filePath para o buffer DBBuffer
        /// </summary>
        /// <param name="filePath"></param>
        public DBWriter(String filePath)
        {
            DBBuffer = new List<List<String>>();
            //System.IO.File.AppendAllText(FilePath, "");
            this.FilePath = filePath;
            //Console.WriteLine(FilePath);
            this.File = new System.IO.StreamReader(filePath);
            this.LoadFileToBuffer();
            Console.WriteLine(this.LastLineIndex);
            if (this.LastLineIndex == 0)
            {
                
                this.File.Close();
                this.WriteHeader();
            }
            
        }

        /// <summary>
        /// Funcao responsavel por carregar os dados do arquivo para o DBBuffer. Ela é chamada no construtor
        /// </summary>
        private void LoadFileToBuffer()
        {
            int i = 0;
            while (true)
            {
                String line = File.ReadLine();
                if (line == null)
                {
                    break;
                }
                else
                {
                    String[] aux = line.Split(";");
                    List<String> aux2 = aux.ToList<String>();
                    this.DBBuffer.Add(aux2);
                }
                i++;
            }
            this.LastLineIndex = i;
        }

        private void WriteHeader()
        {
            String Header = "UserID;UserIP;CEP;ReturnedAddres;SearchTime;UserIPLocation\n";
            this.writeLineinFile(Header);
            this.DBBuffer.Add((Header.Split(";")).ToList<String>());
            
        }

        private void writeLineinFile(String line)
        {

            System.IO.File.AppendAllText(this.FilePath, line);
            
        }


        /// <summary>
        /// Funcao que insere linhas de dados no arquivo e no buffer
        /// </summary>
        /// <param name="lineValues"></param>
        /// <returns></returns>
        public bool insertLine(String [] lineValues)
        {
            
            this.File.Close();
            if (lineValues.Length > this.DBBuffer[0].Count)
            {
                return false;
            }
            this.DBBuffer.Add(lineValues.ToList<String>());
            try
            {
                String aux = "";
                foreach (String s in lineValues)
                {
                    aux += s + ";";
                }
                aux += '\n';
                Console.WriteLine(aux);
                
                this.writeLineinFile(aux);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public void ShowBuffer()
        {
            foreach (System.Collections.Generic.List<string> line in this.DBBuffer)
            {
                foreach(String element in line)
                {
                    Console.Write(element+' ');
                }
                Console.WriteLine("");
            }
            {

            }
        }

    }
}