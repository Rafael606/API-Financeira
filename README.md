# PixApi - Sistema de Controle de Limite PIX

API RESTful desenvolvida em ASP.NET Core (.NET 8) para gerenciar contas bancárias e processar transações PIX com controle de limite, utilizando AWS DynamoDB Local para armazenamento de dados.

Desenvolvido para: Teste Técnico BTG Pactual

---

## 1. Tecnologias Utilizadas

- .NET 8 / ASP.NET Core Web API
- AWS DynamoDB Local
- AWSSDK.DynamoDBv2
- Docker & Docker Compose
- MSTest + Moq

---

## 2. Arquitetura do Projeto

```
PixApi/
├── Controllers/        # Camada de apresentação - endpoints HTTP
├── Services/           # Camada de negócio - regras e validações
├── Repositories/       # Camada de dados - persistência no DynamoDB
├── Models/             # Entidades de domínio
└── Validations/        # Validações customizadas (ex: CPF)

PixApi.Tests/
├── ContaControllerTests.cs
├── TransacaoPixControllerTests.cs
└── TransacaoPixServiceTests.cs
```

**Fluxo de dados:**
```
[Cliente HTTP] → [Controller] → [Service] → [Repository] → [DynamoDB]
```

---

## 3. Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

---

## 4. Como Executar

**1. Suba o DynamoDB Local:**
```bash
docker-compose up
```

**2. Crie as tabelas:**
```bash
# Tabela Conta
aws dynamodb create-table \
  --table-name Conta \
  --attribute-definitions \
    AttributeName=CPF,AttributeType=S \
    AttributeName=NumeroConta,AttributeType=S \
    AttributeName=AgenciaConta,AttributeType=S \
    AttributeName=LimitePIX,AttributeType=S \
  --key-schema \
    AttributeName=CPF,KeyType=HASH \
    AttributeName=NumeroConta,KeyType=RANGE \
  --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5 \
  --endpoint-url http://localhost:8000

# Tabela Transacao
aws dynamodb create-table \
  --table-name Transacao \
  --attribute-definitions \
    AttributeName=NumeroConta,AttributeType=S \
    AttributeName=DataTransacao,AttributeType=S \
    AttributeName=CPF,AttributeType=S \
    AttributeName=ValorTransacao,AttributeType=S \
  --key-schema \
    AttributeName=NumeroConta,KeyType=HASH \
    AttributeName=DataTransacao,KeyType=RANGE \
  --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5 \
  --endpoint-url http://localhost:8000
```

**3. Execute a API:**
```bash
cd PixApi
dotnet run
```

A API estará disponível em `http://localhost:5126`.

---

## 5. Endpoints

### Contas

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/conta` | Criar conta |
| GET | `/api/conta?cpf={cpf}&numeroConta={numeroConta}` | Buscar conta |
| PUT | `/api/conta` | Atualizar limite PIX |
| DELETE | `/api/conta?cpf={cpf}&numeroConta={numeroConta}` | Remover conta |

### Transações PIX

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/transacaopix` | Processar transação PIX |

---

## 6. Exemplos de Uso

**Criar conta:**
```json
POST /api/conta
{
  "cpf": "11122233344",
  "agenciaConta": "0001",
  "numeroConta": "54321",
  "limitePIX": 1000.00
}
```

**Processar transação:**
```json
POST /api/transacaopix
{
  "cpf": "11122233344",
  "numeroConta": "54321",
  "valor": 300.00
}

// Resposta aprovada
{ "aprovada": true, "mensagem": "Transação aprovada." }

// Resposta negada
{ "aprovada": false, "mensagem": "Limite insuficiente." }
```

---

## 7. Testes

```bash
cd PixApi.Tests
dotnet test
```

Os testes cobrem:
- Respostas HTTP dos controllers (200, 201, 400, 404, 409)
- Regras de negócio do service (aprovação, limite insuficiente, conta não encontrada)
- Verificação de chamadas ao service com Moq

---

## 8. Validações

| Campo | Regras |
|-------|--------|
| CPF | Obrigatório, 11 dígitos, apenas números, válido pelo algoritmo da Receita Federal |
| NumeroConta | Obrigatório, 4 a 10 dígitos, apenas números |
| AgenciaConta | Apenas números |
| LimitePIX / Valor | Entre 0 e 1.000.000 |