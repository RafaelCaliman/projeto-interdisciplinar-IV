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

- Linguagem: C#
- Banco de dados: MySQL
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
- MySQL Server instalado localmente

### 1. Clonar o repositório
git clone https://github.com/RafaelCaliman/projeto-interdisciplinar-IV.git

### 2. Criar o banco de dados
Rodar o script que está na pasta `Banco de dados/` no seu MySQL local. Isso cria o banco `hotdog_brasilia` do zero, já com dados de teste.

### 3. Abrir o projeto
Dentro da pasta clonada, entrar em `HotDogBrasilia/` e abrir o arquivo `HotDogBrasilia.sln`.

### 4. Ajustar a senha do banco
Abrir `HotDogBrasilia.Data/ConexaoBanco.cs` e trocar a senha pela senha do MySQL da sua máquina.

### 5. Rodar
O projeto de inicialização já vem configurado como `HotDogBrasilia.UI`. É só apertar **F5** no Visual Studio.

### Estrutura do projeto
- `HotDogBrasilia.UI` — telas (Windows Forms)
- `HotDogBrasilia.Domain` — entidades do sistema (Cliente, etc.)
- `HotDogBrasilia.Data` — conexão com o banco e repositórios
