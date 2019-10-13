using System;
using System.Collections.Generic;
using System.Linq;

namespace DataBase
{
    /// <summary>
    /// Essa classe e o gerenciador do CSV que simula o DB da aplicacao. Ela possui as funcoes
    /// que a aplicacao precisa para interagir com a base de dados
    ///
    /// </summary>
    public class DBWriter
    {
        private System.IO.StreamReader File; 
        private String FilePath; // Caminho do arquivo em que o DB é armazenado
        private List<List<String>> DBBuffer; // Buffer de dados que reflete o arquivo. 
        private int LastLineIndex; // Numero de registros do banco

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
            String Header = "UserID;UserIP;CEP;ReturnedAddres;SearchTime;UserIPLocation;\n"; //Campos do Header
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
           
            try
            {
                String aux = "";
                foreach (String s in lineValues)
                {
                    aux += s + ";";
                }
                aux += '\n';
                
                
                this.writeLineinFile(aux); // Grava registro no arquivo
                this.DBBuffer.Add(lineValues.ToList<String>()); //Adiciona valor no Buffer
                this.LastLineIndex++;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e); //Exibe exception (remover nas verssoes finais)
                return false;
            }
        }

        /// <summary>
        /// Exibe o buffer no console
        /// </summary>
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

        /// <summary>
        /// Essa funcao retorna o indica de uma coluna com base no nome passado no parametro.
        /// A busca nao e case senstive, pois ha uma normalizacao das strings feita dentro da funcao
        /// </summary>
        /// <param name="columName"></param>
        /// <returns></returns>
        public int GetColumIndex(String columName)
        {
            columName = columName.ToUpper();
            int i = 0;
            List<String> Header = this.DBBuffer[0];
            foreach (String element in Header)
            {
                String aux = element.ToUpper();
                if (aux.Equals(columName))
                {
                    return i;
                }
                i++;
            }
            return -1; // Esse valor indica que o columName informado não foi localizado no Header
        }

        /// <summary>
        /// Essa funcao retorna um List<List<String>> contendo um subset do DB.
        /// Esse subset consiste de um filtro que traz somente as linhas em que a coluna
        /// com o nome infomrado em "columName" possua o valor igual ao valor de KeyValue, 
        /// ou seja, essa funcao retorna um filtro do DB feito na coluna columName com o valor
        /// de keyValue
        /// </summary>
        /// <param name="columName"></param>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public List<List<String>> GetLines(String columName, String KeyValue)
        {
            return this.GetLines(this.GetColumIndex(columName), KeyValue);
        }

        /// <summary>
        /// Essa funcao retorna um List<List<String>> contendo um subset do DB.
        /// Esse subset consiste de um filtro que traz somente as linhas em que a coluna
        /// com o indice informado em index possua o valor igual ao valor de KeyValue, 
        /// ou seja, essa funcao retorna um filtro do DB feito na coluna do index com o valor
        /// de keyValue
        /// </summary>
        /// <param name="index"></param>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public List<List<String>> GetLines(int index,String KeyValue)
        {
            List<List<String>> answer = new List<List<String>>();
            foreach(List<String> line in this.DBBuffer)
            {
                if (line[index].Equals(KeyValue)){
                    //Console.WriteLine(line);
                    answer.Add(line);
                }
            }
            return answer;
        }

    }
}