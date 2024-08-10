using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Servidor
{
    private static List<Conta> _contas = new List<Conta>();

    public static void Main(string[] args)
    {
        Thread validaStatusThread = new Thread(ValidarStatusContas);
        validaStatusThread.Start();

        TcpListener server = new TcpListener(IPAddress.Any, 8888);
        server.Start();
        Console.WriteLine("Servidor iniciado...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Thread clientThread = new Thread(() => AtenderCliente(client));
            clientThread.Start();
        }
    }

    private static void AtenderCliente(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        StreamReader reader = new StreamReader(stream);
        StreamWriter writer = new StreamWriter(stream);
        writer.AutoFlush = true;

        bool connected = true;
        while (connected)
        {
            writer.WriteLine("Escolha uma opção:\n1. Exibir contas\n2. Incluir conta\n3. Pagar/Receber conta\n4. Sair");
            string opcao = reader.ReadLine();

            switch (opcao)
            {
                case "1":
                    ExibirContas(writer, reader);
                    break;
                case "2":
                    IncluirConta(writer, reader);
                    break;
                case "3":
                    PagarReceberConta(writer, reader);
                    break;
                case "4":
                    connected = false;
                    break;
            }
        }

        client.Close();
    }

    private static void ExibirContas(StreamWriter writer, StreamReader reader)
    {
        writer.WriteLine("Exibir:\n1. Contas a receber\n2. Contas a pagar\n3. Todas as contas\n4. Contas a vencer\n5. Contas vencidas\n6. Contas pagas");
        string opcao = reader.ReadLine();

        List<Conta> contasFiltradas = opcao switch
        {
            "1" => _contas.Where(c => c.TipoConta == TipoConta.Receber).ToList(),
            "2" => _contas.Where(c => c.TipoConta == TipoConta.Pagar).ToList(),
            "3" => _contas,
            "4" => _contas.Where(c => c.StatusConta == Status.AVencer).ToList(),
            "5" => _contas.Where(c => c.StatusConta == Status.Vencido).ToList(),
            "6" => _contas.Where(c => c.StatusConta == Status.Pago).ToList(),
            _ => new List<Conta>()
        };

        foreach (var conta in contasFiltradas)
        {
            writer.WriteLine($"Cod: {conta.CodConta}, Nome: {conta.Nome}, Status: {conta.StatusConta}");
        }
    }

    private static void IncluirConta(StreamWriter writer, StreamReader reader)
    {
        writer.WriteLine("Nome da conta:");
        string nome = reader.ReadLine();

        writer.WriteLine("Valor total:");
        float valorTotal = float.Parse(reader.ReadLine());

        writer.WriteLine("Quantidade de parcelas:");
        int quantParcela = int.Parse(reader.ReadLine());

        writer.WriteLine("Tipo de conta (1 - Pagar, 2 - Receber):");
        TipoConta tipoConta = (TipoConta)(int.Parse(reader.ReadLine()) - 1);

        Conta conta = new Conta
        {
            Nome = nome,
            ValorTotal = valorTotal,
            TipoConta = tipoConta
        };

        conta.SetQuantParcela(quantParcela);
        _contas.Add(conta);

        writer.WriteLine("Conta adicionada com sucesso!");
    }

    private static void PagarReceberConta(StreamWriter writer, StreamReader reader)
    {
        writer.WriteLine("1. Pagar\n2. Receber");
        TipoConta tipoConta = (TipoConta)(int.Parse(reader.ReadLine()) - 1);

        List<Conta> contasFiltradas = _contas.Where(c => c.TipoConta == tipoConta).ToList();
        foreach (var conta in contasFiltradas)
        {
            writer.WriteLine($"Cod: {conta.CodConta}, Nome: {conta.Nome}, Status: {conta.StatusConta}");
        }

        writer.WriteLine("Informe o codConta:");
        int codConta = int.Parse(reader.ReadLine());

        writer.WriteLine("Informe o número da parcela a ser paga/recebida:");
        int numeroParcela = int.Parse(reader.ReadLine());

        Conta contaSelecionada = contasFiltradas.FirstOrDefault(c => c.CodConta == codConta);
        contaSelecionada?.PagarReceberParcela(numeroParcela);

        writer.WriteLine("Parcela paga/recebida com sucesso!");
    }

    private static void ValidarStatusContas()
    {
        while (true)
        {
            foreach (var conta in _contas)
            {
                foreach (var parcela in conta.Parcelas)
                {
                    if (parcela.Status == Status.AVencer && parcela.DataVencimento < DateTime.Now)
                    {
                        parcela.Status = Status.Vencido;
                        conta.AtualizarStatusConta();
                    }
                }
            }

            Thread.Sleep(60000); // Verifica a cada minuto
        }
    }
}
