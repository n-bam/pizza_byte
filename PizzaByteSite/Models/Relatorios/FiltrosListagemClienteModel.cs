using PizzaByteDto.RetornosRequisicoes;
using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    public class FiltrosListagemClienteModel : BaseFiltrosRelatorioModel
    {
        public FiltrosListagemClienteModel()
        {
            ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoCliente();
        }

        /// <summary>
        /// Procurar com fragmentos do nome
        /// </summary>
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Procurar pelo telefone
        /// </summary>
        [Display(Name = "Telefone")]
        public string Telefone { get; set; }

        /// <summary>
        /// Procurar pelo CPF
        /// </summary>
        [Display(Name = "CPF")]
        public string Cpf { get; set; }

        /// <summary>
        /// Converte de model para DTO
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref RequisicaoObterListagemClienteDto requisicaoDto, ref string mensagemErro)
        {
            if (!base.ConverterModelParaDto(requisicaoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                requisicaoDto.Cpf = string.IsNullOrWhiteSpace(Cpf) ? "" : Cpf.Trim().Replace("-", "").Replace(".", "");
                requisicaoDto.Telefone = string.IsNullOrWhiteSpace(Telefone) ? "" : Telefone.Trim().Replace("-", "").Replace("(", "").Replace(")", "");
                requisicaoDto.Nome = string.IsNullOrWhiteSpace(Nome) ? "" : Nome.Trim();

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }
    }
}