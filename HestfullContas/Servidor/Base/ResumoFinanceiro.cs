public class ResumoFinanceiro
{
    public List<Conta> Contas { get; set; }
    public float TotalReceber => Contas.Where(c => c.TipoConta == TipoConta.Receber).Sum(c => c.ValorTotal);
    public float TotalPagar => Contas.Where(c => c.TipoConta == TipoConta.Pagar).Sum(c => c.ValorTotal);
    public float Saldo => TotalReceber - TotalPagar;
}