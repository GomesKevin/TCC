﻿
namespace TCC.BackEnd.API.Core.Models
{
    public class Produto
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string CaminhoImagem { get; set; }
        public decimal Valor { get; set; }
    }
}