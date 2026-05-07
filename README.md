​================================================================================
                    PixApi - Sistema de Controle de Limite PIX
================================================================================

Uma API RESTful desenvolvida em ASP.NET Core Web API (.NET 8) para gerenciar 
contas bancárias e processar transações PIX com controle de limite. O projeto 
utiliza AWS DynamoDB Local para armazenamento de dados e segue princípios de 
Clean Code e arquitetura em camadas.

================================================================================
ÍNDICE
================================================================================

1. Sobre o Projeto
2. Funcionalidades
3. Tecnologias Utilizadas
4. Arquitetura do Projeto
5. Pré-requisitos
6. Configuração e Execução Passo a Passo
7. Documentação da API - Endpoints Completos
8. Modelos de Dados Detalhados
9. Testes Unitários
10. Validações Implementadas
11. Exemplos Práticos - Fluxo Completo
12. Docker e DynamoDB Local
13. Troubleshooting
14. Roadmap - Melhorias Futuras
15. Recursos Adicionais
16. Notas Importantes

================================================================================
1. SOBRE O PROJETO
================================================================================

Este sistema simula um controle de limite PIX bancário, permitindo:
- Gerenciamento completo de contas bancárias
- Processamento de transações PIX com validação de limite
- Registro de histórico de transações
- Validações robustas de dados de entrada

Desenvolvido para: Teste técnico BTG Pactual
Framework: .NET 8
Arquitetura: Clean Architecture com separação em camadas
Banco de Dados: AWS DynamoDB Local

================================================================================
2. FUNCIONALIDADES
================================================================================

GESTÃO DE CONTAS
----------------
✓ Criar Conta: Cadastra uma nova conta bancária com CPF, agência, número da 
  conta e limite PIX inicial
✓ Buscar Conta: Consulta os dados de uma conta específica por CPF e número 
  da conta
✓ Atualizar Limite: Modifica o limite PIX disponível de uma conta
✓ Remover Conta: Exclui uma conta do sistema

TRANSAÇÕES PIX
--------------
✓ Processar Transação: Valida e processa transações PIX verificando limite 
  disponível
✓ Registro de Transações: Armazena histórico de transações no DynamoDB
✓ Controle de Limite: Atualiza automaticamente o limite após cada transação 
  aprovada
✓ Validações: Verifica existência da conta e disponibilidade de limite

================================================================================
3. TECNOLOGIAS UTILIZADAS
================================================================================

┌─────────────────────────────────┬──────────────┬─────────────────────────┐
│ Tecnologia                      │ Versão       │ Finalidade              │
├─────────────────────────────────┼──────────────┼─────────────────────────┤
│ .NET                            │ 8.0          │ Framework principal     │
│ ASP.NET Core Web API            │ 8.0          │ Construção da API       │
│ AWS DynamoDB Local              │ Latest       │ Banco de dados NoSQL    │
│ AWSSDK.DynamoDBv2               │ 4.0.18.1     │ Cliente AWS para .NET   │
│ AWSSDK.Extensions.NETCore.Setup │ 4.0.3.36     │ Integração AWS          │
│ xUnit                           │ Latest       │ Framework de testes     │
│ Moq                             │ Latest       │ Biblioteca para mocks   │
│ Docker & Docker Compose         │ Latest       │ Containerização         │
│ Swashbuckle.AspNetCore          │ 6.6.2        │ Documentação Swagger    │
└─────────────────────────────────┴──────────────┴─────────────────────────┘

================================================================================
4. ARQUITETURA DO PROJETO
================================================================================

ESTRUTURA DE PASTAS
-------------------

Teste-BTG-main/
├── PixApi/                           # Projeto principal da API
│   ├── Controllers/                  # Camada de apresentação
│   │   ├── ContaController.cs       # Endpoints de contas
│   │   └── TransacaoPixController.cs # Endpoints de transações
│   ├── Services/                     # Camada de negócio
│   │   ├── IContaService.cs
│   │   ├── ContaService.cs
│   │   ├── ITransacaoPixService.cs
│   │   └── TransacaoPixService.cs
│   ├── Repositories/                 # Camada de dados
│   │   ├── IContaRepository.cs
│   │   ├── ContaRepositoryDynamo.cs
│   │   ├── ITransacaoPixRepository.cs
│   │   └── TransacaoPixRepositoryDynamo.cs
│   ├── Models/                       # Entidades de domínio
│   │   ├── Conta.cs
│   │   ├── TransacaoPix.cs
│   │   └── ResultadoTransacao.cs
│   ├── Program.cs                    # Configuração e inicialização
│   └── PixApi.csproj                # Dependências do projeto
├── PixApi.Tests/                     # Projeto de testes
│   ├── TransacaoPixServiceTests.cs  # Testes unitários
│   └── PixApi.Tests.csproj
├── docker-compose.yml                # Configuração do DynamoDB Local
└── README.md

CAMADAS DA APLICAÇÃO
--------------------

1. CONTROLLERS (Camada de Apresentação)
   - Recebem requisições HTTP
   - Validam dados de entrada usando Data Annotations
   - Delegam lógica para a camada de serviço
   - Retornam respostas HTTP apropriadas (200, 201, 404, 409, etc.)

2. SERVICES (Camada de Negócio)
   - Implementam regras de negócio
   - Coordenam operações entre repositories
   - Gerenciam transações e validações complexas
   - Independentes de detalhes de infraestrutura

3. REPOSITORIES (Camada de Dados)
   - Gerenciam persistência no DynamoDB
   - Abstraem detalhes de acesso a dados
   - Implementam interfaces para facilitar testes
   - Convertem entre entidades de domínio e estruturas do DynamoDB

4. MODELS (Entidades de Domínio)
   - Definem estruturas de dados
   - Contêm validações usando Data Annotations
   - Representam conceitos do domínio (Conta, Transação)

FLUXO DE DADOS
--------------

[Cliente HTTP] 
    ↓ Request
[Controller] → Valida entrada
    ↓
[Service] → Aplica regras de negócio
    ↓
[Repository] → Persiste/Recupera dados
    ↓
[DynamoDB Local]
    ↑
[Repository] → Retorna dados
    ↑
[Service] → Processa resultado
    ↑
[Controller] → Formata resposta
    ↑ Response
[Cliente HTTP]

================================================================================
5. PRÉ-REQUISITOS
================================================================================

Antes de executar o projeto, certifique-se de ter instalado:

✓ .NET 8 SDK - Framework principal
  https://dotnet.microsoft.com/download/dotnet/8.0

✓ Docker Desktop - Para executar DynamoDB Local
  https://www.docker.com/products/docker-desktop

✓ AWS CLI - Para criar tabelas no DynamoDB (opcional)
  https://aws.amazon.com/cli/

✓ Editor de Código - Visual Studio 2022+, VS Code ou Rider

✓ Cliente HTTP - Postman, Insomnia, Thunder Client ou curl

VERIFICAR INSTALAÇÕES
----------------------

# Verificar .NET SDK
dotnet --version

# Verificar Docker
docker --version
docker-compose --version

# Verificar AWS CLI (opcional)
aws --version

================================================================================
6. CONFIGURAÇÃO E EXECUÇÃO PASSO A PASSO
================================================================================

PASSO 1: CLONE O REPOSITÓRIO
-----------------------------

git clone <url-do-repositorio>
cd Teste-BTG-main

PASSO 2: INICIE O DYNAMODB LOCAL COM DOCKER
--------------------------------------------

docker-compose up -d

Verificar se o container está rodando:
docker ps

Você deve ver um container chamado 'dynamodb-local' rodando na porta 8000.

PASSO 3: CRIAR AS TABELAS NO DYNAMODB
--------------------------------------

OPÇÃO 1: Usando AWS CLI (Recomendado)

# Criar tabela Conta
aws dynamodb create-table `
  --table-name Conta `
  --attribute-definitions `
    AttributeName=CPF,AttributeType=S `
    AttributeName=NumeroConta,AttributeType=S `
  --key-schema `
    AttributeName=CPF,KeyType=HASH `
    AttributeName=NumeroConta,KeyType=RANGE `
  --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5 `
  --endpoint-url http://localhost:8000

# Criar tabela Transacao
aws dynamodb create-table `
  --table-name Transacao `
  --attribute-definitions `
    AttributeName=CPF,AttributeType=S `
    AttributeName=NumeroConta,AttributeType=S `
  --key-schema `
    AttributeName=CPF,KeyType=HASH `
    AttributeName=NumeroConta,KeyType=RANGE `
  --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5 `
  --endpoint-url http://localhost:8000

Verificar tabelas criadas:
aws dynamodb list-tables --endpoint-url http://localhost:8000

OPÇÃO 2: Usando NoSQL Workbench

1. Baixe NoSQL Workbench
   https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/workbench.html
2. Conecte ao localhost:8000
3. Crie as tabelas manualmente com as configurações acima

PASSO 4: RESTAURE AS DEPENDÊNCIAS
----------------------------------

cd PixApi
dotnet restore

PASSO 5: EXECUTE A APLICAÇÃO
-----------------------------

dotnet run

Saída esperada:
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5126
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7126
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.

PASSO 6: TESTAR A API
----------------------

A API estará disponível em:
- HTTP: http://localhost:5126
- HTTPS: https://localhost:7126

Testar com curl:
curl http://localhost:5126/api/conta?cpf=12345678901&numeroConta=12345

================================================================================
7. DOCUMENTAÇÃO DA API - ENDPOINTS COMPLETOS
================================================================================

╔════════════════════════════════════════════════════════════════════════════╗
║                              CONTAS                                        ║
╚════════════════════════════════════════════════════════════════════════════╝

┌────────────────────────────────────────────────────────────────────────────┐
│ 1. CRIAR CONTA                                                             │
└────────────────────────────────────────────────────────────────────────────┘

Endpoint: POST /api/conta

Headers:
Content-Type: application/json

Body:
{
  "cpf": "12345678901",
  "agenciaConta": "0001",
  "numeroConta": "12345",
  "limitePIX": 1000.00
}

RESPOSTAS:

✓ 201 Created
{
  "cpf": "12345678901",
  "agenciaConta": "0001",
  "numeroConta": "12345",
  "limitePIX": 1000.00
}

✗ 400 Bad Request - Dados inválidos
{
  "errors": {
    "Cpf": ["CPF deve ter 11 dígitos."]
  }
}

✗ 409 Conflict - Conta já existe
Conta já existe.

Exemplo curl:
curl -X POST http://localhost:5126/api/conta `
  -H "Content-Type: application/json" `
  -d '{\"cpf\":\"12345678901\",\"agenciaConta\":\"0001\",\"numeroConta\":\"12345\",\"limitePIX\":1000.00}'

┌────────────────────────────────────────────────────────────────────────────┐
│ 2. BUSCAR CONTA                                                            │
└────────────────────────────────────────────────────────────────────────────┘

Endpoint: GET /api/conta?cpf={cpf}&numeroConta={numeroConta}

Parâmetros de Query:
- cpf (string, obrigatório): CPF com 11 dígitos
- numeroConta (string, obrigatório): Número da conta

RESPOSTAS:

✓ 200 OK
{
  "cpf": "12345678901",
  "agenciaConta": "0001",
  "numeroConta": "12345",
  "limitePIX": 1000.00
}

✗ 404 Not Found
Conta não encontrada.

Exemplo curl:
curl http://localhost:5126/api/conta?cpf=12345678901&numeroConta=12345

┌────────────────────────────────────────────────────────────────────────────┐
│ 3. ATUALIZAR LIMITE                                                        │
└────────────────────────────────────────────────────────────────────────────┘

Endpoint: PUT /api/conta

Headers:
Content-Type: application/json

Body:
{
  "cpf": "12345678901",
  "numeroConta": "12345",
  "limitePIX": 1500.00
}

RESPOSTAS:

✓ 200 OK
Limite atualizado com sucesso.

✗ 404 Not Found
Conta não encontrada.

Exemplo curl:
curl -X PUT http://localhost:5126/api/conta `
  -H "Content-Type: application/json" `
  -d '{\"cpf\":\"12345678901\",\"numeroConta\":\"12345\",\"limitePIX\":1500.00}'

┌────────────────────────────────────────────────────────────────────────────┐
│ 4. REMOVER CONTA                                                           │
└────────────────────────────────────────────────────────────────────────────┘

Endpoint: DELETE /api/conta?cpf={cpf}&numeroConta={numeroConta}

Parâmetros de Query:
- cpf (string, obrigatório): CPF com 11 dígitos
- numeroConta (string, obrigatório): Número da conta

RESPOSTAS:

✓ 200 OK
Conta removida com sucesso.

✗ 404 Not Found
Conta não encontrada.

Exemplo curl:
curl -X DELETE http://localhost:5126/api/conta?cpf=12345678901&numeroConta=12345

╔════════════════════════════════════════════════════════════════════════════╗
║                          TRANSAÇÕES PIX                                    ║
╚════════════════════════════════════════════════════════════════════════════╝

┌────────────────────────────────────────────────────────────────────────────┐
│ 5. PROCESSAR TRANSAÇÃO PIX                                                 │
└────────────────────────────────────────────────────────────────────────────┘

Endpoint: POST /api/transacaopix

Headers:
Content-Type: application/json

Body:
{
  "cpf": "12345678901",
  "numeroConta": "12345",
  "valor": 500.00
}

RESPOSTAS:

✓ 200 OK - Transação Aprovada
{
  "aprovada": true,
  "mensagem": "Transação aprovada."
}

✗ 200 OK - Transação Negada (Limite Insuficiente)
{
  "aprovada": false,
  "mensagem": "Limite insuficiente."
}

✗ 200 OK - Transação Negada (Conta Não Encontrada)
{
  "aprovada": false,
  "mensagem": "Conta não encontrada."
}

✗ 400 Bad Request - Dados Inválidos
{
  "errors": {
    "Valor": ["Limite PIX deve estar entre 0 e 1.000.000."]
  }
}

Exemplo curl:
curl -X POST http://localhost:5126/api/transacaopix `
  -H "Content-Type: application/json" `
  -d '{\"cpf\":\"12345678901\",\"numeroConta\":\"12345\",\"valor\":500.00}'

================================================================================
8. MODELOS DE DADOS DETALHADOS
================================================================================

┌────────────────────────────────────────────────────────────────────────────┐
│ CONTA                                                                      │
└────────────────────────────────────────────────────────────────────────────┘

Classe: PixApi.Models.Conta

┌──────────────┬─────────┬──────────────────────┬─────────────────────────┐
│ Propriedade  │ Tipo    │ Validação            │ Descrição               │
├──────────────┼─────────┼──────────────────────┼─────────────────────────┤
│ Cpf          │ string  │ Required, 11 chars,  │ CPF do titular da conta │
│              │         │ somente números      │                         │
├──────────────┼─────────┼──────────────────────┼─────────────────────────┤
│ AgenciaConta │ string  │ Somente números      │ Código da agência       │
├──────────────┼─────────┼──────────────────────┼─────────────────────────┤
│ NumeroConta  │ string  │ Required, 4-10 chars,│ Número da conta         │
│              │         │ somente números      │                         │
├──────────────┼─────────┼──────────────────────┼─────────────────────────┤
│ LimitePIX    │ decimal │ Range(0, 1000000)    │ Limite disponível PIX   │
└──────────────┴─────────┴──────────────────────┴─────────────────────────┘

CÓDIGO:

[DynamoDBTable("Conta")]
public class Conta
{
    [DynamoDBHashKey]
    [Required(ErrorMessage = "CPF é obrigatório.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter 11 dígitos.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter apenas números.")]
    public string Cpf { get; set; } = string.Empty;

    [RegularExpression(@"^\d+$", ErrorMessage = "Agência deve conter apenas números.")]
    public string AgenciaConta { get; set; } = string.Empty;

    [Required(ErrorMessage = "Número da conta é obrigatório.")]
    [StringLength(10, MinimumLength = 4, ErrorMessage = "Número da conta deve ter entre 4 e 10 dígitos.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Número da conta deve conter apenas números.")]
    public string NumeroConta { get; set; } = string.Empty;

    [Range(0, 1000000, ErrorMessage = "Limite PIX deve estar entre 0 e 1.000.000.")]
    public decimal LimitePIX { get; set; }
}

ESTRUTURA DYNAMODB:
- Partition Key (HASH): CPF
- Sort Key (RANGE): NumeroConta

┌────────────────────────────────────────────────────────────────────────────┐
│ TRANSAÇÃO PIX                                                              │
└────────────────────────────────────────────────────────────────────────────┘

Classe: PixApi.Models.TransacaoPix

┌──────────────┬─────────┬────────────────────┬────────────────────────┐
│ Propriedade  │ Tipo    │ Validação          │ Descrição              │
├──────────────┼─────────┼────────────────────┼────────────────────────┤
│ Cpf          │ string  │ Required           │ CPF do pagador         │
├──────────────┼─────────┼────────────────────┼────────────────────────┤
│ NumeroConta  │ string  │ Required           │ Número da conta        │
├──────────────┼─────────┼────────────────────┼────────────────────────┤
│ Valor        │ decimal │ Range(0, 1000000)  │ Valor da transação     │
└──────────────┴─────────┴────────────────────┴────────────────────────┘

CÓDIGO:

public class TransacaoPix
{
    [Required(ErrorMessage = "CPF é obrigatório.")]
    public string Cpf { get; set; } = string.Empty;

    [Required(ErrorMessage = "Número da conta é obrigatório.")]
    public string NumeroConta { get; set; } = string.Empty;

    [Range(0, 1000000, ErrorMessage = "Limite PIX deve estar entre 0 e 1.000.000.")]
    public decimal Valor { get; set; }
}

ARMAZENAMENTO:
- Salva no DynamoDB com timestamp (DataTransacao)
- Formato de data: ISO 8601 (UTC-3)

┌────────────────────────────────────────────────────────────────────────────┐
│ RESULTADO TRANSAÇÃO                                                        │
└────────────────────────────────────────────────────────────────────────────┘

Classe: PixApi.Models.ResultadoTransacao

┌──────────────┬──────────┬───────────────────────────────────────┐
│ Propriedade  │ Tipo     │ Descrição                             │
├──────────────┼──────────┼───────────────────────────────────────┤
│ Aprovada     │ bool     │ Indica se transação foi aprovada      │
├──────────────┼──────────┼───────────────────────────────────────┤
│ Mensagem     │ string   │ Mensagem explicativa do resultado     │
└──────────────┴──────────┴───────────────────────────────────────┘

MENSAGENS POSSÍVEIS:
✓ "Transação aprovada."
✗ "Limite insuficiente."
✗ "Conta não encontrada."

================================================================================
9. TESTES UNITÁRIOS
================================================================================

EXECUTAR TODOS OS TESTES
-------------------------

cd PixApi.Tests
dotnet test

Saída esperada:
Passed!  - Failed:     0, Passed:     3, Skipped:     0, Total:     3

EXECUTAR COM VERBOSIDADE
-------------------------

dotnet test --logger "console;verbosity=detailed"

COBERTURA DE TESTES
-------------------

O projeto inclui testes para o TransacaoPixService:

┌────────────────────────────────────────────────────────────────────────────┐
│ TESTE 1: Transação Aprovada com Limite Suficiente                         │
└────────────────────────────────────────────────────────────────────────────┘

CENÁRIO:
- Conta existe com limite de R$ 500,00
- Transação de R$ 100,00

RESULTADO ESPERADO:
- Transação aprovada
- Limite atualizado para R$ 400,00
- Transação salva no repositório

CÓDIGO:
[Fact]
public async Task Processar_Transacao_Deve_Aprovar_Quando_Limite_Suficiente()
{
    // Arrange
    var transacao = new TransacaoPix { Cpf = "123", NumeroConta = "1", Valor = 100 };
    var conta = new Conta { Cpf = "123", NumeroConta = "1", LimitePIX = 500 };
    
    // Act
    var resultado = await _service.ProcessarTransacaoAsync(transacao);
    
    // Assert
    Assert.True(resultado.Aprovada);
    Assert.Equal("Transação aprovada.", resultado.Mensagem);
}

┌────────────────────────────────────────────────────────────────────────────┐
│ TESTE 2: Transação Negada por Limite Insuficiente                         │
└────────────────────────────────────────────────────────────────────────────┘

CENÁRIO:
- Conta existe com limite de R$ 50,00
- Transação de R$ 100,00

RESULTADO ESPERADO:
- Transação negada
- Mensagem: "Limite insuficiente."
- Limite não alterado

┌────────────────────────────────────────────────────────────────────────────┐
│ TESTE 3: Transação Negada por Conta Inexistente                           │
└────────────────────────────────────────────────────────────────────────────┘

CENÁRIO:
- CPF e conta não existem no sistema
- Transação de R$ 100,00

RESULTADO ESPERADO:
- Transação negada
- Mensagem: "Conta não encontrada."

TECNOLOGIAS DE TESTE
---------------------
- xUnit: Framework de testes
- Moq: Criação de mocks para IContaService e ITransacaoPixRepository
- Assertions: Verificação de comportamentos esperados

================================================================================
10. VALIDAÇÕES IMPLEMENTADAS
================================================================================

VALIDAÇÕES DE CPF
-----------------

┌────────────────────┬──────────────────────────────┬─────────────────────┐
│ Regra              │ Validação                    │ Mensagem de Erro    │
├────────────────────┼──────────────────────────────┼─────────────────────┤
│ Obrigatório        │ [Required]                   │ CPF é obrigatório.  │
├────────────────────┼──────────────────────────────┼─────────────────────┤
│ Tamanho exato      │ [StringLength(11, Min=11)]   │ CPF deve ter 11     │
│                    │                              │ dígitos.            │
├────────────────────┼──────────────────────────────┼─────────────────────┤
│ Somente números    │ [RegularExpression(@"^\d{11}$")]│ CPF deve conter  │
│                    │                              │ apenas números.     │
└────────────────────┴──────────────────────────────┴─────────────────────┘

EXEMPLOS:
✓ Válido: "12345678901"
✗ Inválido: "123456789" (menos de 11)
✗ Inválido: "123.456.789-01" (contém pontuação)

VALIDAÇÕES DE NÚMERO DA CONTA
------------------------------

┌────────────────────┬──────────────────────────────┬─────────────────────┐
│ Regra              │ Validação                    │ Mensagem de Erro    │
├────────────────────┼──────────────────────────────┼─────────────────────┤
│ Obrigatório        │ [Required]                   │ Número da conta é   │
│                    │                              │ obrigatório.        │
├────────────────────┼──────────────────────────────┼─────────────────────┤
│ Tamanho            │ [StringLength(10, Min=4)]    │ Número da conta     │
│                    │                              │ deve ter 4-10 díg.  │
├────────────────────┼──────────────────────────────┼─────────────────────┤
│ Somente números    │ [RegularExpression(@"^\d+$")]│ Deve conter apenas  │
│                    │                              │ números.            │
└────────────────────┴──────────────────────────────┴─────────────────────┘

VALIDAÇÕES DE AGÊNCIA
----------------------

┌────────────────────┬──────────────────────────────┬─────────────────────┐
│ Regra              │ Validação                    │ Mensagem de Erro    │
├────────────────────┼──────────────────────────────┼─────────────────────┤
│ Somente números    │ [RegularExpression(@"^\d+$")]│ Agência deve conter │
│                    │                              │ apenas números.     │
└────────────────────┴──────────────────────────────┴─────────────────────┘

VALIDAÇÕES DE LIMITE PIX / VALOR
---------------------------------

┌────────────────────┬──────────────────────────────┬─────────────────────┐
│ Regra              │ Validação                    │ Mensagem de Erro    │
├────────────────────┼──────────────────────────────┼─────────────────────┤
│ Range              │ [Range(0, 1000000)]          │ Limite PIX deve     │
│                    │                              │ estar entre 0 e     │
│                    │                              │ 1.000.000.          │
└────────────────────┴──────────────────────────────┴─────────────────────┘

================================================================================
11. EXEMPLOS PRÁTICOS - FLUXO COMPLETO
================================================================================

╔════════════════════════════════════════════════════════════════════════════╗
║ CENÁRIO 1: Criar Conta e Fazer Transação Bem-Sucedida                     ║
╚════════════════════════════════════════════════════════════════════════════╝

# 1. Criar conta com limite de R$ 1000
curl -X POST http://localhost:5126/api/conta `
  -H "Content-Type: application/json" `
  -d '{\"cpf\":\"11122233344\",\"agenciaConta\":\"0001\",\"numeroConta\":\"54321\",\"limitePIX\":1000.00}'

Resposta: 201 Created

# 2. Buscar conta criada
curl http://localhost:5126/api/conta?cpf=11122233344&numeroConta=54321

Resposta: 200 OK
{"cpf":"11122233344","agenciaConta":"0001","numeroConta":"54321","limitePIX":1000.00}

# 3. Fazer transação de R$ 300
curl -X POST http://localhost:5126/api/transacaopix `
  -H "Content-Type: application/json" `
  -d '{\"cpf\":\"11122233344\",\"numeroConta\":\"54321\",\"valor\":300.00}'

Resposta: 200 OK
{"aprovada":true,"mensagem":"Transação aprovada."}

# 4. Verificar limite atualizado
curl http://localhost:5126/api/conta?cpf=11122233344&numeroConta=54321

Resposta: 200 OK
{"cpf":"11122233344","agenciaConta":"0001","numeroConta":"54321","limitePIX":700.00}

╔════════════════════════════════════════════════════════════════════════════╗
║ CENÁRIO 2: Transação Negada por Limite Insuficiente                       ║
╚════════════════════════════════════════════════════════════════════════════╝

# 1. Tentar transação de R$ 800 (limite atual: R$ 700)
curl -X POST http://localhost:5126/api/transacaopix `
  -H "Content-Type: application/json" `
  -d '{\"cpf\":\"11122233344\",\"numeroConta\":\"54321\",\"valor\":800.00}'

Resposta: 200 OK
{"aprovada":false,"mensagem":"Limite insuficiente."}

# 2. Verificar que o limite não mudou
curl http://localhost:5126/api/conta?cpf=11122233344&numeroConta=54321

Resposta: limitePIX continua 700.00

╔════════════════════════════════════════════════════════════════════════════╗
║ CENÁRIO 3: Atualizar Limite e Fazer Nova Transação                        ║
╚════════════════════════════════════════════════════════════════════════════╝

# 1. Aumentar limite para R$ 2000
curl -X PUT http://localhost:5126/api/conta `
  -H "Content-Type: application/json" `
  -d '{\"cpf\":\"11122233344\",\"numeroConta\":\"54321\",\"limitePIX\":2000.00}'

Resposta: 200 OK - "Limite atualizado com sucesso."

# 2. Fazer transação de R$ 800 (agora possível)
curl -X POST http://localhost:5126/api/transacaopix `
  -H "Content-Type: application/json" `
  -d '{\"cpf\":\"11122233344\",\"numeroConta\":\"54321\",\"valor\":800.00}'

Resposta: 200 OK - {"aprovada":true,"mensagem":"Transação aprovada."}

╔════════════════════════════════════════════════════════════════════════════╗
║ CENÁRIO 4: Remover Conta                                                   ║
╚════════════════════════════════════════════════════════════════════════════╝

# 1. Remover conta
curl -X DELETE http://localhost:5126/api/conta?cpf=11122233344&numeroConta=54321

Resposta: 200 OK - "Conta removida com sucesso."

# 2. Tentar buscar conta removida
curl http://localhost:5126/api/conta?cpf=11122233344&numeroConta=54321

Resposta: 404 Not Found - "Conta não encontrada."

================================================================================
12. DOCKER E DYNAMODB LOCAL
================================================================================

CONFIGURAÇÃO DO DOCKER-COMPOSE.YML
-----------------------------------

version: '3.8'

services:
  dynamodb-local:
    command: "-jar DynamoDBLocal.jar -sharedDb -dbPath ./data"
    image: amazon/dynamodb-local:latest
    container_name: dynamodb-local
    ports:
      - "8000:8000"
    volumes:
      - "./docker/dynamodb:/home/dynamodblocal/data"
    working_dir: /home/dynamodblocal

COMANDOS ÚTEIS DOCKER
---------------------

# Iniciar DynamoDB Local
docker-compose up -d

# Parar DynamoDB Local
docker-compose down

# Ver logs
docker-compose logs -f

# Verificar status
docker ps

# Reiniciar container
docker-compose restart

# Remover volumes (limpar dados)
docker-compose down -v

INTERAGIR COM DYNAMODB LOCAL
-----------------------------

# Listar tabelas
aws dynamodb list-tables --endpoint-url http://localhost:8000

# Descrever tabela
aws dynamodb describe-table --table-name Conta --endpoint-url http://localhost:8000

# Escanear dados (cuidado em produção!)
aws dynamodb scan --table-name Conta --endpoint-url http://localhost:8000

# Deletar tabela
aws dynamodb delete-table --table-name Conta --endpoint-url http://localhost:8000

================================================================================
13. TROUBLESHOOTING
================================================================================

PROBLEMA: Docker não inicia
----------------------------

Erro: Cannot connect to the Docker daemon

SOLUÇÃO:
# Verificar se Docker Desktop está rodando
docker --version

# Reiniciar Docker Desktop
# Windows: Restart Docker Desktop do menu

PROBLEMA: Porta 8000 já está em uso
------------------------------------

Erro: bind: address already in use

SOLUÇÃO:
# Identificar processo usando porta 8000
netstat -ano | findstr :8000

# Matar processo (substitua PID)
taskkill /PID <PID> /F

# Ou alterar porta no docker-compose.yml
ports:
  - "8001:8000"

PROBLEMA: Tabelas não encontradas
----------------------------------

Erro: ResourceNotFoundException: Cannot do operations on a non-existent table

SOLUÇÃO:
# Verificar se tabelas existem
aws dynamodb list-tables --endpoint-url http://localhost:8000

# Recriar tabelas (ver Passo 3 da configuração)

PROBLEMA: Erro ao conectar no DynamoDB
---------------------------------------

Erro: Unable to reach DynamoDB endpoint

SOLUÇÃO:
Verificar configuração em Program.cs:

builder.Services.AddSingleton<IAmazonDynamoDB>(sp =>
{
    var config = new AmazonDynamoDBConfig
    {
        ServiceURL = "http://localhost:8000" // Verificar porta
    };
    return new AmazonDynamoDBClient(
        new Amazon.Runtime.BasicAWSCredentials("fake", "fake"),
        config
    );
});

PROBLEMA: Testes falhando
--------------------------

SOLUÇÃO:
# Limpar e reconstruir
dotnet clean
dotnet restore
dotnet build

# Executar testes com mais detalhes
dotnet test --logger "console;verbosity=detailed"

================================================================================
14. ROADMAP - MELHORIAS FUTURAS
================================================================================

SEGURANÇA
---------
☐ Implementar autenticação JWT
☐ Adicionar autorização baseada em roles (admin, user)
☐ Implementar rate limiting por IP
☐ Adicionar validação real de CPF (algoritmo verificador)
☐ Implementar HTTPS obrigatório em produção
☐ Adicionar CORS configurável

FUNCIONALIDADES
---------------
☐ Endpoint para listar transações de uma conta
☐ Filtros e paginação em listagens
☐ Soft delete de contas (ao invés de exclusão física)
☐ Suporte a múltiplos tipos de transação (TED, DOC)
☐ Notificações por email/SMS após transações
☐ Agendamento de transações futuras

PERFORMANCE E ESCALABILIDADE
-----------------------------
☐ Implementar cache com Redis
☐ Adicionar índices secundários no DynamoDB
☐ Implementar padrão CQRS
☐ Event Sourcing para histórico de transações
☐ Filas (AWS SQS/RabbitMQ) para processamento assíncrono

OBSERVABILIDADE
---------------
☐ Logging estruturado com Serilog
☐ Integração com Application Insights
☐ Health checks customizados
☐ Métricas com Prometheus
☐ Tracing distribuído com OpenTelemetry
☐ Dashboard de monitoramento

DEVOPS
------
☐ CI/CD com GitHub Actions
☐ Deploy automatizado para AWS
☐ Testes de integração com TestContainers
☐ Testes de carga com k6
☐ Versionamento semântico automatizado
☐ Documentação de API com Stoplight

QUALIDADE DE CÓDIGO
--------------------
☐ Aumentar cobertura de testes para 80%+
☐ Adicionar testes de integração
☐ Implementar mutation testing
☐ Análise estática de código (SonarQube)
☐ Pre-commit hooks com Husky

================================================================================
15. RECURSOS ADICIONAIS
================================================================================

DOCUMENTAÇÃO OFICIAL
---------------------
- ASP.NET Core
  https://docs.microsoft.com/aspnet/core

- AWS DynamoDB
  https://docs.aws.amazon.com/dynamodb

- AWS SDK for .NET
  https://docs.aws.amazon.com/sdk-for-net

- xUnit
  https://xunit.net/

- Docker
  https://docs.docker.com/

TUTORIAIS RELACIONADOS
-----------------------
- Clean Architecture no .NET
  https://learn.microsoft.com/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures

- DynamoDB Best Practices
  https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/best-practices.html

- REST API Design
  https://restfulapi.net/

================================================================================
16. NOTAS IMPORTANTES
================================================================================

AMBIENTE DE DESENVOLVIMENTO
----------------------------
- Este projeto usa DynamoDB Local para desenvolvimento
- Credenciais "fake" são usadas localmente (não usar em produção)
- Dados são persistidos em ./docker/dynamodb/

MIGRAÇÃO PARA PRODUÇÃO
-----------------------
Para usar em produção com AWS DynamoDB real:

1. Remover configuração local do Program.cs:

// Remover ServiceURL e credenciais fake
builder.Services.AddAWSService<IAmazonDynamoDB>();

2. Configurar credenciais AWS:

aws configure

3. Atualizar appsettings.Production.json:

{
  "AWS": {
    "Region": "us-east-1"
  }
}

================================================================================
INFORMAÇÕES DO PROJETO
================================================================================

Desenvolvido para: Teste Técnico BTG Pactual
Data: Maio de 2026
Tecnologias: .NET 8, ASP.NET Core, AWS DynamoDB, Docker
Workspace: C:\Users\rhc\Downloads\Teste-BTG-main\Teste-BTG-main\

================================================================================
CONTRIBUIÇÃO
================================================================================

Este projeto foi desenvolvido como teste técnico. Para contribuir:

1. Fork o repositório
2. Crie uma branch (git checkout -b feature/nova-funcionalidade)
3. Commit suas mudanças (git commit -am 'Adiciona nova funcionalidade')
4. Push para a branch (git push origin feature/nova-funcionalidade)
5. Abra um Pull Request

================================================================================
SUPORTE
================================================================================

Para dúvidas, problemas ou sugestões:
- Abra uma Issue no repositório
- Entre em contato através do email fornecido

================================================================================

PixApi - Sistema de Controle de Limite PIX © 2026
Obrigado por revisar este projeto! Feedback é sempre bem-vindo.

================================================================================
FIM DO DOCUMENTO
================================================================================