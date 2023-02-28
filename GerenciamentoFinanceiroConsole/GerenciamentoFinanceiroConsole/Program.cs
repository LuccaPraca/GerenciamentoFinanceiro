using System;
using System.Collections.Generic;
using System.Globalization;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GerenciadorFinanceiro
{
    public class Transacao
    {
        public ObjectId Id { get; set; }
        public double Valor { get; set; }
        public DateTime Data { get; set; }
        public string Tipo { get; set; }
    }

    class Categoria
    {
        public ObjectId Id { get; set; }
        public string Nome { get; set; }
    }

    class GerenciadorFinanceiro
    {
        private IMongoCollection<Transacao> _transacoesCollection;
        private IMongoCollection<Categoria> _categoriasCollection;

        public GerenciadorFinanceiro(IMongoCollection<Transacao> transacoesCollection, IMongoCollection<Categoria> categoriasCollection)
        {
            _transacoesCollection = transacoesCollection;
            _categoriasCollection = categoriasCollection;
        }

        public void AdicionarTransacao(Transacao transacao)
        {
            _transacoesCollection.InsertOne(transacao);
        }

        public void AdicionarCategoria(Categoria categoria)
        {
            _categoriasCollection.InsertOne(categoria);
        }

        public List<Transacao> BuscarTransacoes(DateTime dataInicial, DateTime dataFinal)
        {
            var builder = Builders<Transacao>.Filter;
            var filtro = builder.Gte("Data", dataInicial) & builder.Lte("Data", dataFinal);

            return _transacoesCollection.Find(filtro).ToList();
        }

        public double CalcularSaldo(DateTime dataInicial, DateTime dataFinal)
        {
            var builder = Builders<Transacao>.Filter;
            var filtro = builder.Gte("Data", dataInicial) & builder.Lte("Data", dataFinal);

            var transacoes = _transacoesCollection.Find(filtro).ToList();

            double saldo = 0;

            foreach (var transacao in transacoes)
            {
                if (transacao.Tipo == "receita")
                {
                    saldo += transacao.Valor;
                }
                else
                {
                    saldo -= transacao.Valor;
                }
            }

            return saldo;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            // Conecta ao banco de dados MongoDB
            var client = new MongoClient("mongodb+srv://Usuario:Senha@cluster0.yu1abtv.mongodb.net/?retryWrites=true&w=majority");
            var database = client.GetDatabase("financeiro");
            var transacoesCollection = database.GetCollection<Transacao>("Transacoes");
            var categoriasCollection = database.GetCollection<Categoria>("Categorias");
            var gerenciadorFinanceiro = new GerenciadorFinanceiro(transacoesCollection, categoriasCollection);


            Console.WriteLine("Bem-vindo ao Gerenciador Financeiro - By: Lucca Peixoto Praça");

            while (true)
            {
                Console.WriteLine("\nEscolha uma opção:");
                Console.WriteLine("1. Adicionar transação");
                Console.WriteLine("2. Adicionar categoria");
                Console.WriteLine("3. Gerar relatório de transações");
                Console.WriteLine("4. Sair");

                var opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        Console.WriteLine("\nAdicionar transação");
                        Console.Write("Digite o valor da transação: ");
                        var valor = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
                        Console.Write("Digite a data da transação (DD/MM/AAAA): ");
                        var data = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        Console.Write("Digite o tipo da transação (receita/despesa): ");
                        var tipo = Console.ReadLine();
                        // Cria uma nova instância da classe Transacao com as informações inseridas pelo usuário
                        var transacao = new Transacao
                        {
                            Valor = valor,
                            Data = data,
                            Tipo = tipo
                        };

                        gerenciadorFinanceiro.AdicionarTransacao(transacao);
                        Console.WriteLine("Transação adicionada com sucesso!");
                        break;
                    case "2":
                        Console.WriteLine("\nAdicionar categoria");
                        Console.Write("Digite o nome da categoria: ");
                        var nomeCategoria = Console.ReadLine();
                        var categoria = new Categoria
                        {
                            Nome = nomeCategoria
                        };
                        gerenciadorFinanceiro.AdicionarCategoria(categoria);
                        Console.WriteLine("Categoria adicionada com sucesso!");
                        break;
                    case "3":
                        Console.WriteLine("\nGerar relatório de transações");
                        Console.Write("Digite a data inicial (DD/MM/AAAA): ");
                        var dataInicial = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        Console.Write("Digite a data final (DD/MM/AAAA  DATA LIMITE: HOJE): ");
                        var dataFinal = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        var transacoes = gerenciadorFinanceiro.BuscarTransacoes(dataInicial, dataFinal);
                 

                        Console.WriteLine("\nTransações realizadas no período de {0} a {1}:", dataInicial.ToString("dd/MM/yyyy"), dataFinal.ToString("dd/MM/yyyy"));
                        foreach (var transacaoumdois in transacoes)
                        {
                            Console.WriteLine("{0} - {1}: R${2}", transacaoumdois.Data.ToString("dd/MM/yyyy"), transacaoumdois.Tipo, transacaoumdois.Valor.ToString("F2", CultureInfo.InvariantCulture));
                        }

                        var saldo = gerenciadorFinanceiro.CalcularSaldo(dataInicial, dataFinal);
                        Console.WriteLine("\nSaldo no período: R${0}", saldo.ToString("F2", CultureInfo.InvariantCulture));
                        break;
                    case "4":
                        Console.WriteLine("Saindo...");
                        return;
                    default:
                        Console.WriteLine("Opção inválida");
                        break;
                }
            }
        }
    }
}