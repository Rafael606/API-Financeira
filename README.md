# PixApi - Sistema de Controle de Limite PIX

Uma API simples em ASP.NET Core Web API (.NET 8) para simular um sistema de controle de limite de transações PIX.

## Funcionalidades

### CRUD de Contas
- **Criar Conta**: POST `/api/conta` - Cadastra uma nova conta com CPF, agência, número da conta e limite inicial.
- **Buscar Conta**: GET `/api/conta/{cpf}/{numeroConta}` - Busca uma conta específica.
- **Atualizar Limite**: PUT `/api/conta/{cpf}/{numeroConta}/limite` - Atualiza o limite disponível da conta.
- **Remover Conta**: DELETE `/api/conta/{cpf}/{numeroConta}` - Remove uma conta do sistema.

### Transação PIX
- **Processar Transação**: POST `/api/transacaopix` - Valida e processa uma transação PIX baseada no limite disponível.

## Estrutura do Projeto

O projeto segue princípios de Clean Code e separação de responsabilidades:

- **Controllers**: Responsáveis por receber as requisições HTTP e retornar respostas.
- **Services**: Contêm a lógica de negócio.
- **Models**: Definem as estruturas de dados (Conta, TransacaoPix, ResultadoTransacao).

### Armazenamento
Atualmente utiliza armazenamento em memória (Dictionary) para facilitar testes e desenvolvimento. Estruturado para fácil migração para banco de dados como DynamoDB.

## Como Rodar o Projeto

### Pré-requisitos
- .NET 8 SDK instalado
- (Opcional) Visual Studio Code ou outro editor
- (Opcional) curl ou Postman para testar os endpoints

### Passos
1. Clone ou baixe o projeto.
2. Navegue até a pasta do projeto: `cd PixApi`
3. Restaure as dependências: `dotnet restore`
4. Execute o projeto: `dotnet run`
5. A API estará disponível em `http://localhost:5126`

### Executar Testes
1. Navegue até a pasta de testes: `cd PixApi.Tests`
2. Execute: `dotnet test`

## Como Testar a API

Use curl no terminal ou qualquer cliente HTTP (Postman, Insomnia, etc.):

### Criar Conta
```bash
curl -X POST http://localhost:5126/api/conta \
  -H "Content-Type: application/json" \
  -d '{"cpf":"12345678901","agencia":"0001","numeroConta":"12345","limiteDisponivel":1000.00}'
```

### Buscar Conta
```bash
curl http://localhost:5126/api/conta/12345678901/12345
```

### Processar Transação PIX
```bash
curl -X POST http://localhost:5126/api/transacaopix \
  -H "Content-Type: application/json" \
  -d '{"cpf":"12345678901","numeroConta":"12345","valor":500.00}'
```

### Atualizar Limite
```bash
curl -X PUT http://localhost:5126/api/conta/12345678901/12345/limite \
  -H "Content-Type: application/json" \
  -d '1500.00'
```

### Remover Conta
```bash
curl -X DELETE http://localhost:5126/api/conta/12345678901/12345
```

## Validações
- CPF: Obrigatório, exatamente 11 dígitos
- Número da conta: Obrigatório
- Valor da transação: Deve ser maior que 0
- Limite: Deve ser maior ou igual a 0

## Testes Unitários
Inclui testes para:
- Transação aprovada (desconta limite)
- Transação negada por limite insuficiente
- Transação negada por conta inexistente
- Atualização de limite

## Possíveis Melhorias Futuras
- Implementar autenticação/autorização
- Migrar para banco de dados (DynamoDB, SQL Server, etc.)
- Adicionar logging estruturado
- Implementar cache para melhor performance
- Adicionar validação de CPF real
- Implementar versionamento de API
- Adicionar monitoramento e métricas
- Implementar rate limiting para segurança