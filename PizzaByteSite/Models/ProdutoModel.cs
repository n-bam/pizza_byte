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
    public class ProdutoModel : BaseModel
    {
        public ProdutoModel()
        {
            ListaTipos = Utilidades.RetornarListaTiposProduto();
        }

        /// <summary>
        /// Breve descritivo do produto
        /// </summary>
        [Required(ErrorMessage = "Por favor, preencha a descrição do produto. Ex.: Pizza de frango")]
        [Display(Name = "Descrição")]
        [StringLength(150, ErrorMessage = "Informe a descrição para o produto de 3 a 150 letras.", MinimumLength = 3)]
        public string Descricao { get; set; }

        /// <summary>
        /// Mais detalhes sobre o produto
        /// </summary>
        [Display(Name = "Detalhes")]
        [StringLength(200, ErrorMessage = "Informe os detalhes para o produto de 3 a 200 letras.", MinimumLength = 3)]
        [DataType(DataType.MultilineText)]
        public string Detalhes { get; set; }

        /// <summary>
        /// Preço de venda do produto
        /// </summary>
        [Required(ErrorMessage = "Por favor, preencha o preço de venda do produto")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0.01, 999999.99, ErrorMessage = "Informe o preço de venda")]
        [Display(Name = "Preço")]
        public float Preco { get; set; }

        /// <summary>
        /// Indica o tipo do produto (bebida, pizza, etc.)
        /// </summary>
        [Required(ErrorMessage = "Por favor, selecione qual o tipo do produto")]
        [Display(Name = "Tipo")]
        public TipoProduto Tipo { get; set; }

        /// <summary>
        /// Lista com as opções de tipos de produtos
        /// </summary>
        public List<SelectListItem> ListaTipos { get; set; }

        /// <summary>
        /// Converte um produto de DTO para Model
        /// </summary>
        /// <param name="produtoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(ProdutoDto produtoDto, ref string mensagemErro)
        {
            try
            {
                Descricao = string.IsNullOrWhiteSpace(produtoDto.Descricao) ? "" : produtoDto.Descricao.Trim();
                Detalhes = string.IsNullOrWhiteSpace(produtoDto.Detalhes) ? "" : produtoDto.Detalhes.Trim();
                Preco = produtoDto.Preco;
                Tipo = produtoDto.Tipo;
                DataAlteracao = produtoDto.DataAlteracao;
                DataInclusao = produtoDto.DataInclusao;
                Id = produtoDto.Id;
                Inativo = produtoDto.Inativo;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um produto de Model para Dto
        /// </summary>
        /// <param name="produtoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref ProdutoDto produtoDto, ref string mensagemErro)
        {
            try
            {
                produtoDto.Descricao = string.IsNullOrWhiteSpace(Descricao) ? "" : Descricao.Trim();
                produtoDto.Detalhes = string.IsNullOrWhiteSpace(Detalhes) ? "" : Detalhes.Trim();
                produtoDto.Preco = Preco;
                produtoDto.Tipo = Tipo;
                produtoDto.DataAlteracao = this.DataAlteracao;
                produtoDto.DataInclusao = this.DataInclusao;
                produtoDto.Id = this.Id;
                produtoDto.Inativo = this.Inativo;

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