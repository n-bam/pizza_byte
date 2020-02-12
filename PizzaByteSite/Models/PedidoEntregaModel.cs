using PizzaByteDto.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um produto no site
    /// </summary>
    public class PedidoEntregaModel : BaseModel
    {
        public PedidoEntregaModel()
        {
            ClienteEndereco = new ClienteEnderecoModel();
            ListaFuncionarios = Utilidades.RetornarListaFuncionarios(TipoFuncionario.Motoboy);
        }

        /// <summary>
        /// Indica se o retorno da entrega já teve os valores conferidos
        /// </summary>
        [Display(Name = "Conferido")]
        public bool Conferido { get; set; }

        /// <summary>
        /// Valor em dinheiro retornado na entrega
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Valor retornado")]
        public float ValorRetorno { get; set; }

        /// <summary>
        /// Observações gerais da entrega
        /// </summary>
        [Display(Name = "Observações")]
        [DataType(DataType.MultilineText)]
        [StringLength(2000, ErrorMessage = "Informe uma observação de 0 a 2000 letras.")]
        public string Obs { get; set; }

        /// <summary>
        /// Identificação do endereço de entrega
        /// </summary>
        [Required]
        public Guid IdEndereco { get; set; }

        /// <summary>
        /// Identifica o funcionário que fez a entrega
        /// </summary>
        [Display(Name = "Funcionário")]
        public Guid? IdFuncionario { get; set; }

        /// <summary>
        /// Identificação do pedido que gerou a entrega
        /// </summary>
        [Required]
        public Guid IdPedido { get; set; }

        /// <summary>
        /// Dados do endereço do cliente
        /// </summary>
        public ClienteEnderecoModel ClienteEndereco { get; set; }

        /// <summary>
        /// Lista com as opções de funcionários
        /// </summary>
        public List<SelectListItem> ListaFuncionarios { get; set; }

        /// <summary>
        /// Converte um produto de Model para Dto
        /// </summary>
        /// <param name="entregaDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref PedidoEntregaDto entregaDto, ref string mensagemErro)
        {
            try
            {
                entregaDto.Obs = string.IsNullOrWhiteSpace(Obs) ? "" : Obs.Trim();
                entregaDto.Conferido = this.Conferido;
                entregaDto.IdEndereco = this.IdEndereco;
                entregaDto.IdFuncionario = this.IdFuncionario;
                entregaDto.IdPedido = this.IdPedido;
                entregaDto.ValorRetorno = this.ValorRetorno;
                entregaDto.DataAlteracao = this.DataAlteracao;
                entregaDto.DataInclusao = this.DataInclusao;
                entregaDto.Id = this.Id;
                entregaDto.Inativo = this.Inativo;

                // Converter endereço
                ClienteEnderecoDto clienteEnderecoDto = new ClienteEnderecoDto();
                if (!ClienteEndereco.ConverterModelParaDto(ref clienteEnderecoDto, ref mensagemErro))
                {
                    return false;
                }

                entregaDto.ClienteEndereco = clienteEnderecoDto;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um pedido de DTO para Model
        /// </summary>
        /// <param name="entregaDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(PedidoEntregaDto entregaDto, ref string mensagemErro)
        {
            try
            {
                Obs = string.IsNullOrWhiteSpace(entregaDto.Obs) ? "" : entregaDto.Obs.Trim();
                Conferido = entregaDto.Conferido;
                IdEndereco = entregaDto.IdEndereco;
                IdFuncionario = entregaDto.IdFuncionario;
                IdPedido = entregaDto.IdPedido;
                ValorRetorno = entregaDto.ValorRetorno;
                DataAlteracao = entregaDto.DataAlteracao;
                DataInclusao = entregaDto.DataInclusao;
                Id = entregaDto.Id;
                Inativo = entregaDto.Inativo;

                // Converter endereço
                ClienteEnderecoModel clienteEnderecoModel = new ClienteEnderecoModel();
                if (entregaDto.ClienteEndereco != null)
                {
                    if (!clienteEnderecoModel.ConverterDtoParaModel(entregaDto.ClienteEndereco, ref mensagemErro))
                    {
                        return false;
                    }

                }
                else
                {
                    clienteEnderecoModel.Id = entregaDto.IdEndereco;
                }

                ClienteEndereco = clienteEnderecoModel;

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