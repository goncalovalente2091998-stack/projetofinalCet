using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Models
{
    public class Pagamento
    {
        public int IdPagamento { get; set; }

        public int IdCliente { get; set; }

        public string NomeCliente { get; set; } = string.Empty;

        public DateTime DataPagamento { get; set; }

        public decimal Valor { get; set; }

        public string MetodoPagamento { get; set; } = string.Empty;

        public string Observacoes { get; set; } = string.Empty;

        public string Estado { get; set; } = "Pendente";

        public string ReferenciaExterna { get; set; } = string.Empty;

        public string IdTransacaoExterna { get; set; } = string.Empty;

        public DateTime? DataConfirmacao { get; set; }

        public int? IdInscricao { get; set; }

        public string NomePlano { get; set; } = string.Empty;
    }
}
