# Backend

The backend is completely empty and up to you to build.
You can use any tech you think would suit this challenge well.

# Welcome to a backend explanation 🧑‍🏫

The goal here is to explain how to initialize and configure the backend of our application.
Enjoy reading! 

## Tecnologias Utilizadas
Nesta seção, apresento as tecnologias escolhidas para o desenvolvimento do projeto, juntamente com uma breve justificativa para cada escolha:

 - Linguagem de Programação: C# + ASP.NET Core
A linguagem C# e o framework ASP.NET Core foram selecionados por sua robustez, desempenho e ampla adoção na construção de APIs REST. O ASP.NET Core oferece suporte nativo a padrões modernos de desenvolvimento, como injeção de dependência, modularidade e alta performance, tornando-o ideal para aplicações backend escaláveis.

- Entity Framework (EF)
O Entity Framework foi utilizado como ORM (Object-Relational Mapping) para facilitar a interação entre o código e o banco de dados. Ele permite que consultas sejam feitas de forma declarativa utilizando LINQ, simplificando operações complexas e reduzindo a necessidade de escrever SQL manualmente.

- SQLite
O SQLite foi escolhido como banco de dados pela sua leveza e facilidade de configuração. Ele é ideal para projetos pequenos ou protótipos, permitindo execuções rápidas e integração eficiente com o Entity Framework. Além disso, sua portabilidade garante que o ambiente seja fácil de configurar e executar.

- Documentação de Endpoints: OpenAPI + Swagger
Para garantir uma documentação clara e acessível dos endpoints da API, utilizamos o padrão OpenAPI em conjunto com o Swagger. Essa abordagem facilita a compreensão e o teste das APIs, oferecendo uma interface interativa para desenvolvedores explorarem os recursos disponíveis no sistema.

## how to run the project
    antes de qualquer coisa vou recomendar o uso da IDE visual Studio community 2022 pois ja possui algumas automações que agilizam o trabalho, mas pensando nos outros cenarios dentro do arquivo 