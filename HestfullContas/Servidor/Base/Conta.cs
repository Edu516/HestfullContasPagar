public class Conta
{
    private static int _contadorCodConta = 0;

    public int CodConta { get; private set; }
    public string Nome { get; set; }
    public float ValorTotal { get; set; }
    public int QuantParcela { get; set; }
    public List<Parcela> Parcelas { get; set; }
    public DateTime DataCriacao { get; private set; }
    public TipoConta TipoConta { get; set; }
    public Status StatusConta { get; set; }

    public Conta()
    {
        CodConta = ++_contadorCodConta;
        DataCriacao = DateTime.Now;
        Parcelas = new List<Parcela>();
    }

    public void SetQuantParcela(int quantidade)
    {
        QuantParcela = quantidade;
        Parcelas.Clear();

        DateTime dataVencimento = DateTime.Now.AddMonths(1);

        for (int i = 1; i <= quantidade; i++)
        {
            Parcela parcela = new Parcela
            {
                CodConta = this.CodConta,
                NumeroParcela = i,
                DataVencimento = dataVencimento,
                Valor = ValorTotal / quantidade,
                Status = Status.AVencer
            };

            Parcelas.Add(parcela);
            dataVencimento = dataVencimento.AddMonths(1);
        }

        AtualizarStatusConta();
    }

    public void PagarReceberParcela(int numeroParcela)
    {
        Parcela parcela = Parcelas.Find(p => p.NumeroParcela == numeroParcela);
        if (parcela != null)
        {
            parcela.Status = Status.Pago;
            AtualizarStatusConta();
        }
    }

    public void AtualizarStatusConta()
    {
        if (Parcelas.Exists(p => p.Status == Status.Vencido))
        {
            StatusConta = Status.Vencido;
        }
        else if (Parcelas.TrueForAll(p => p.Status == Status.Pago))
        {
            StatusConta = Status.Pago;
        }
        else
        {
            StatusConta = Status.AVencer;
        }
    }
}