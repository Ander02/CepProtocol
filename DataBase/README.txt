Nessa pasta. H� a classe DBWriter. Essa classe tem como objetivo intermediar as intera��es com a base de dados.
Essa classe gerenciar o arquivo de texto da base de dados e uma vari�vel de buffer que facilita e otmiza as fun��es de busca.
As principais funcionalidades que ela fornece s�o fun��es de busca e inser��o de dados, sendo que, dentre estas podemos
destacar as seguintes fun��es:
- public bool insertLine(String [] lineValues): Essa fun��o insere registros na base de dados e retorna True caso tudo ocorra
  como o esperado. Essa fun��o recebe os campos do registro na forma de um vetor se Strings, que, internamente � convertido 
  para uma string que separa os campos por ";".

- public List<List<String>> GetLines(String columName, String KeyValue): Essa fun��o � a fun��o  de busca da classe. Ela realiza filtros no
  buffer a partir de uma coluna e um valor pelo qual a coluna deve ser filtrada. 
  Um exemplo de aplica��o dessa fun��o seria descobrir as buscas de um usu�rio "User1". A coluna que cont�m o usu�rio na base
  de hist�rico de buscas � a coluna "UserID". A fun��o seria chamada da seguinte forma a partir de um objeto DB do tipo DBWriter:
  List <List<String>> X=DB.GetLines("UserID","User1");

  A vari�vel "X" contem uma lista com os registros de buscas do usu�rio "User1" na base de hist�rico de buscas.


- public void ShowBuffer(): Exibe o estado do buffer de dados. Este deve refletir fielmente o que est� gravado no arquivo.

- public int GetColumIndex(String columName): Essa fun��o retorna o indice de uma coluna com base em seu Nome. A busca n�o � 
  Case Sensitive, o que diminui a chance de erros por falta de aten��o com mai�sculas e min�suculas. Um exemplo de aplica��o dessa
  fun��o seria quando se � necess�rio acessar dados em uma determinada coluna, pois o usu�rio n�o precisaria se preocupar em saber
  abrir a base de dados manualmente para localizar o �ndice.

- public List<List<String>> GetBuffer(): Essa fun��o retorna uma c�pia do buffer de dados do objeto DBWriter. A vari�vel que
  armazena o buffer � private e n�o pode ser alterada.