using System;
using System.IO;
using System.Net.Sockets;

class Cliente
{
    public static void Main(string[] args)
    {
        try
        {
            // Conectar ao servidor na porta 8888
            TcpClient client = new TcpClient("127.0.0.1", 8888);
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

            bool running = true;

            // Loop principal do cliente
            while (running)
            {
                // Receber e exibir o menu do servidor
                string menu = reader.ReadLine();
                Console.WriteLine(menu);
                
                // Enviar a escolha do usuário para o servidor
                string option = Console.ReadLine();
                writer.WriteLine(option);

                // Verificar se o usuário escolheu sair
                if (option == "4")
                {
                    running = false;
                }
                else
                {
                    // Receber e exibir a resposta do servidor
                    while (true)
                    {
                        string response = reader.ReadLine();
                        if (string.IsNullOrEmpty(response))
                            break;

                        Console.WriteLine(response);
                    }
                }
            }

            // Fechar a conexão com o servidor
            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro: " + ex.Message);
        }
    }
}
