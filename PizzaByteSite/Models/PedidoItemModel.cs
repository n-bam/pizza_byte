using PizzaByteDto.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um pedido no site
    /// </summary>
    public class PedidoItemModel : BaseModel
    {
        public PedidoItemModel()
        {
            ListaTipos = Utilidades.RetornarListaTiposPedido();
        }

        /// <summary>
        /// Breve descritivo do produto
        /// </summary>
        [Required(ErrorMessage = "Por favor, preencha a descrição do produto. Ex.: Pizza de frango")]
        [Display(Name = "Descrição")]
        [StringLength(300, ErrorMessage = "Informe a descrição para o produto de 3 a 300 letras.", MinimumLength = 3)]
        public string DescricaoProduto { get; set; }

        /// <summary>
        /// Preço de venda do produto
        /// </summary>
        [Required(ErrorMessage = "Por favor, preencha o preço de venda do produto")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0.01, 999999.99, ErrorMessage = "Informe o preço de venda")]
        [Display(Name = "Preço")]
        public float PrecoProduto { get; set; }

        /// <summary>
        /// Indica o tipo do produto (bebida, pizza, etc.)
        /// </summary>
        [Required(ErrorMessage = "Por favor, selecione qual o tipo do produto")]
        [Display(Name = "Tipo")]
        public TipoProduto TipoProduto { get; set; }

        [Display(Name = "Quantidade")]
        [Required(ErrorMessage = "Por favor, informe a quantidade de cada item")]
        public float Quantidade { get; set; }

        /// <summary>
        /// Indica qual é o outro sabor de uma pizza meio a meio
        /// </summary>
        public Guid? IdProdutoComposto { get; set; }

        /// <summary>
        /// Identifica o produto adicionado no pedido
        /// </summary>
        public Guid IdProduto { get; set; }

        /// <summary>
        /// Identifica o pedido que o item pertence
        /// </summary>
        public Guid IdPedido { get; set; }

        /// <summary>
        /// Lista com as opções de tipos de pedido
        /// </summary>
        public List<SelectListItem> ListaTipos { get; set; }

        /// <summary>
        /// Converte um item de DTO para Model
        /// </summary>
        /// <param name="itemDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(PedidoItemDto itemDto, ref string mensagemErro)
        {
            try
            {
                this.DescricaoProduto = string.IsNullOrWhiteSpace(itemDto.DescricaoProduto) ? "" : itemDto.DescricaoProduto.Trim();
                this.IdPedido = itemDto.IdPedido;
                this.IdProduto = itemDto.IdProduto;
                this.IdProdutoComposto = itemDto.IdProdutoComposto;
                this.PrecoProduto = itemDto.PrecoProduto;
                this.Quantidade = itemDto.Quantidade;
                this.TipoProduto = itemDto.TipoProduto;
                this.DataAlteracao = itemDto.DataAlteracao;
                this.DataInclusao = itemDto.DataInclusao;
                this.Id = itemDto.Id;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um item de Model para Dto
        /// </summary>
        /// <param name="cepDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref PedidoItemDto itemDto, ref string mensagemErro)
        {
            try
            {
                itemDto.DescricaoProduto = string.IsNullOrWhiteSpace(this.DescricaoProduto) ? "" : this.DescricaoProduto.Trim();
                itemDto.IdPedido = this.IdPedido;
                itemDto.IdProduto = this.IdProduto;
                itemDto.IdProdutoComposto = this.IdProdutoComposto;
                itemDto.PrecoProduto = this.PrecoProduto;
                itemDto.Quantidade = this.Quantidade;
                itemDto.TipoProduto = this.TipoProduto;
                itemDto.DataAlteracao = this.DataAlteracao;
                itemDto.DataInclusao = this.DataInclusao;
                itemDto.Id = this.Id;

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