# InterPlayers:
## Case técnico para vaga de desenvolvedor fullstack

O projeto consiste em um sistema simples para gerenciamento de pedidos e seus itens.  
Atendendo aos requisitos de tecnologias (ASP .NET Core e SQL Server) e 
estrategias (aquitetura limpa e recursos do baco de dados, como stored procedures e triggers).

### Tecnologias utilizadas
- ASP .NET 10 (.NET 10)
- SQL Server
- Dapper
- XUnit

### Estrategias aplicadas
- <a href="https://medium.com/@jenilsojitra/clean-architecture-in-net-core-e18b4ad229c8">Arquitetura limpa</a>
- <a href="https://dev.to/cristofima/building-rich-domain-models-a-practical-guide-to-ddd-in-net-5952">Dominio rico</a>
- <a href="https://djesusnet.medium.com/test-driven-development-tdd-com-xunit-para-c-net-core-6d3732f40963">Test Driven Design ou TDD</a>

### Recursos de dominio
- Produto
- Pedido
- PedidoItem

### Rodando o projeto
1. Clone o projeto
```bash
git clone git@github.com:vitormcampos/interplayers-case-tecnico.git
```
2. Edite o appsettings.Development.json para acesso ao banco de dados.  
Edite a propriedade **"InterPlayersConnectionString"** para os valores do seu banco de dados local.

3. Agora precisamos definir o banco de dados, preparei outro documento com esses passos: [Configuração do banco de dados](database.md)


4. Rode o projeto **InterPlayer.API**, via Visual Studio ou CLI:
```bash
dotnet run --project ./InterPlayers.API/
```

5. Agora acesse a URL da aplicação, com a rota do **Swagger**. No meu caso ficou assim:
```
http://localhost:5249/swagger/index.html
```

Com isso você terá acesso aos endpoints e verá as operações de backend:
- Gerenciar Produtos
- Gerenciar Pedidos
- Gerenciar Items de pedidos  

---
# Obrigado pela oportunidade