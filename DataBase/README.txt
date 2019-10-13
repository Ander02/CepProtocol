Nessa pasta. Há a classe DBWriter. Essa classe tem como objetivo intermediar as interações com a base de dados.
Essa classe gerenciar o arquivo de texto da base de dados e uma variável de buffer que facilita e otmiza as funções de busca.
As principais funcionalidades que ela fornece são funções de busca e inserção de dados, sendo que, dentre estas podemos
destacar as seguintes funções:
- public bool insertLine(String [] lineValues): Essa função insere registros na base de dados e retorna True caso tudo ocorra
  como o esperado. Essa função recebe os campos do registro na forma de um vetor se Strings, que, internamente é convertido 
  para uma string que separa os campos por ";".

- public List<List<String>> GetLines(String columName, String KeyValue): Essa função é a função  de busca da classe. Ela realiza filtros no
  buffer a partir de uma coluna e um valor pelo qual a coluna deve ser filtrada. 
  Um exemplo de aplicação dessa função seria descobrir as buscas de um usuário "User1". A coluna que contém o usuário na base
  de histórico de buscas é a coluna "UserID". A função seria chamada da seguinte forma a partir de um objeto DB do tipo DBWriter:
  List <List<String>> X=DB.GetLines("UserID","User1");

  A variável "X" contem uma lista com os registros de buscas do usuário "User1" na base de histórico de buscas.


- public void ShowBuffer(): Exibe o estado do buffer de dados. Este deve refletir fielmente o que está gravado no arquivo.

- public int GetColumIndex(String columName): Essa função retorna o indice de uma coluna com base em seu Nome. A busca não é 
  Case Sensitive, o que diminui a chance de erros por falta de atenção com maiúsculas e minúsuculas. Um exemplo de aplicação dessa
  função seria quando se é necessário acessar dados em uma determinada coluna, pois o usuário não precisaria se preocupar em saber
  abrir a base de dados manualmente para localizar o índice.

- public List<List<String>> GetBuffer(): Essa função retorna uma cópia do buffer de dados do objeto DBWriter. A variável que
  armazena o buffer é private e não pode ser alterada.