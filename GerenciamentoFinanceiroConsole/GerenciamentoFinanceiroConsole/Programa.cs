using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciamentoFinanceiroConsole
{

    public class Transacao
    {
        public double valor { get; set; }
        public DateTime data { get; set; }
        public string tipo { get; set; }
    }

    public class Categoria
    {
        public string nome { get; set; }
    }

    internal class Programa
    {
    }
}
