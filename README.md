# Hot-dog Brasília — Projeto Interdisciplinar IV

Repositório do projeto que estou desenvolvendo com minha equipe para a disciplina de Projeto Interdisciplinar IV, do curso de Sistemas de Informação da UEMG, Unidade Frutal.

## Sobre o projeto

Estamos desenvolvendo uma aplicação desktop de gestão para a empresa da minha família (Hot-dog Brasília, uma lanchonete), como parte da nossa formação em Sistemas de Informação. É um projeto acadêmico, então não tem fins comerciais diretos — o objetivo é aplicar na prática o que aprendemos em desenvolvimento de software, programação orientada a objetos e banco de dados.

A aplicação é desktop, feita em C# com banco de dados MySQL, seguindo os requisitos e regras de negócio que levantamos junto com a empresa parceira.

Por ser um trabalho acadêmico, não garantimos que o sistema final esteja pronto para produção comercial, nem vamos dar manutenção, hospedagem ou suporte depois que a disciplina terminar.

## Equipe

- Eduardo Pedro Batista
- Guilherme da Silva Guerra
- Luiz Guilherme Alves Almeida
- Rafael Rosa Caliman

## Tecnologias

- Linguagem: C# (Windows Forms, .NET Framework 4.7.2)
- Banco de dados: MySQL 8.0 ou mais novo
- Conector: MySql.Data (ADO.NET), chamando só stored procedures
- Arquitetura: em camadas (UI / Domain / Data)

## Vigência do projeto

Início: 14/09/2026
Término: 18/11/2026

## Licença

Projeto acadêmico, de uso restrito ao contexto do Projeto Interdisciplinar IV na UEMG, Unidade Frutal.

## Como Rodar o Projeto

### Pré-requisitos
- Visual Studio 2022 (ou mais recente), com a carga de trabalho ".NET desktop development"
- .NET Framework 4.7.2
- MySQL Server 8.0 ou mais novo, instalado localmente na porta 3306

### 1. Clonar o repositório
git clone https://github.com/RafaelCaliman/projeto-interdisciplinar-IV.git

### 2. Criar o banco de dados
Rodar o script `DataBase/hotdog_brasilia_v6_banco_final.sql` no seu MySQL local (por exemplo, no MySQL Workbench). Esse é o script atual (v6): ele cria o banco `hotdog_brasilia` do zero, já com dados de teste (4 clientes, incluindo o "Cliente balcão", 4 produtos, 3 insumos com as receitas dos hot-dogs, 3 vendas e 5 lançamentos financeiros) e com os usuários `admin`, `operador` e `gerencia` (as senhas iniciais são provisórias e estão no próprio script; troque-as antes de qualquer uso real).

No v6 o estoque dos hot-dogs é controlado por insumo: cada produto pode ter uma receita (pão, salsicha, molho), a venda baixa os insumos e o estoque mostrado do produto é calculado pelo que sobrou deles. Produtos sem receita (refrigerante, suco) continuam com estoque próprio.

Os scripts `DataBase/hotdog_brasilia_v6_testes.sql` (testes completos do banco), `DataBase/hotdog_brasilia_v6_teste_essencial.sql` (versão enxuta) e `DataBase/hotdog_brasilia_v6_teste_T10.sql` (chamadas dos relatórios) são opcionais e rodam depois do script principal.

Versões anteriores do script (v2 a v5) são apenas histórico do projeto e não devem ser usadas para criar o banco.

### 3. Abrir o projeto
Dentro da pasta clonada, entrar em `HotDogBrasilia/` e abrir o arquivo `HotDogBrasilia.sln`.

### 4. Ajustar a conexão com o banco
Abrir `HotDogBrasilia.UI/App.config` e, na cadeia de conexão `hotdog`, ajustar o usuário e a senha do MySQL da sua máquina. O arquivo vem configurado com o usuário `root` sem senha, que é o caso de um MySQL de demonstração; se o seu `root` tem senha, coloque-a só na sua cópia local e não a envie para o repositório. Depois de compilar, o mesmo ajuste vale para o arquivo `HotDogBrasilia.UI.exe.config`, que fica na pasta de saída ao lado do executável.

### 5. Rodar
O projeto de inicialização já vem configurado como `HotDogBrasilia.UI`. É só apertar **F5** no Visual Studio.

### Estrutura do projeto
- `HotDogBrasilia.UI` — telas (Windows Forms)
- `HotDogBrasilia.Domain` — entidades do sistema (Cliente, etc.)
- `HotDogBrasilia.Data` — conexão com o banco e repositórios

### Telas e menu
O menu mostra só o que o perfil do usuário logado pode acessar.
- **Cadastros**: Clientes, Produtos (com a receita de cada produto), Insumos e Fornecedores
- **Vendas**: Nova venda e Consultar vendas
- **Financeiro**: Lançamentos
- **Relatórios**: Vendas por período, Estoque, Insumos e Financeiro
- **Sistema**: Log de ações (consulta de quem fez o quê, por período e por usuário), Trocar usuário e Sair

### Executável (pacote RC)
Para usar o sistema sem o Visual Studio, copie a pasta do pacote RC para o computador. Ela traz `HotDogBrasilia.UI.exe`, as DLLs e o `HotDogBrasilia.UI.exe.config` na mesma pasta, e a pasta `DataBase` com os scripts. O computador precisa de Windows 10 ou 11, .NET Framework 4.7.2 e do MySQL 8.0 ou mais novo, com o script `hotdog_brasilia_v6_banco_final.sql` já executado. Se o `root` do MySQL tiver senha, ajuste `User` e `Password` na cadeia `hotdog` do `.exe.config` antes de abrir o programa.

### Usuários do sistema
O sistema não tem tela de usuários. O script cria `admin`, `operador` e `gerencia`, com as senhas provisórias que aparecem nele. Para trocar a senha, criar ou inativar um usuário, use as procedures `sp_usuario_alterar_senha`, `sp_usuario_inserir` e `sp_usuario_definir_ativo`, direto no banco.

### Usuário hotdog_app e backup
O script também cria o usuário `hotdog_app` do MySQL, que só tem permissão de executar as procedures do banco, com uma senha provisória que está no script. Para trocar a senha, use `ALTER USER` para `localhost` e para `127.0.0.1`. Para usar esse usuário na conexão, troque `User` e `Password` na cadeia `hotdog`.

Backup e restauração (no Prompt de Comando, na pasta `bin` do MySQL, cada comando em uma linha só; o `-p` pede a senha):

```
mysqldump -u root -p --single-transaction --routines --triggers --databases hotdog_brasilia > backup_hotdog_brasilia.sql
mysql -u root -p < backup_hotdog_brasilia.sql
```

O usuário `hotdog_app` não entra no backup: depois de restaurar, rode de novo só o bloco que o cria, no final do `hotdog_brasilia_v6_banco_final.sql`. Não guarde o arquivo de backup dentro do repositório.
