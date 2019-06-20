using PizzaByteBll.Base;
using PizzaByteDal;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteVo;
using System;
using System.Collections.Generic;
using System.Linq;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteBll
{
    public class ProdutoBll : BaseBll<ProdutoVo, ProdutoDto>
    {
        private static LogBll logBll = new LogBll("ProdutoBll");
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public ProdutoBll(bool salvarAlteracoes) : base(logBll)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public ProdutoBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(logBll, contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui um produto no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<ProdutoDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            ProdutoVo produtoVo = new ProdutoVo();
            string mensagemErro = "";

            // Converte para VO a ser incluída no banco de dados
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref produtoVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o produto para VO: " + mensagemErro;
                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(produtoVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o produto para VO: " + mensagemErro;
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui um produto do banco de dados a partir do ID
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Excluir(RequisicaoObterDto requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Excluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para excluir produtos é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém um produto pelo ID 
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<ProdutoDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            ProdutoVo produtoVo;

            if (!ObterPorIdBd(requisicaoDto.Id, out produtoVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o produto: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            retornoDto.Mensagem = "Ok";
            if (produtoVo == null)
            {
                retornoDto.Mensagem = "Produto não encontrado";
            }

            ProdutoDto produtoDto = new ProdutoDto();
            if (!ConverterVoParaDto(produtoVo, ref produtoDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o produto: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            retornoDto.Entidade = produtoDto;
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Converte um produto Dto para um produto Vo
        /// </summary>
        /// <param name="produtoDto"></param>
        /// <param name="produtoVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(ProdutoDto produtoDto, ref ProdutoVo produtoVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(produtoDto, ref produtoVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                produtoVo.Descricao = string.IsNullOrWhiteSpace(produtoDto.Descricao) ? "" : produtoDto.Descricao.Trim();
                produtoVo.Preco = produtoDto.Preco;
                produtoVo.Tipo = produtoDto.Tipo;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o produto para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um produto Dto para um produto Vo
        /// </summary>
        /// <param name="produtoVo"></param>
        /// <param name="produtoDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(ProdutoVo produtoVo, ref ProdutoDto produtoDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(produtoVo, ref produtoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                produtoDto.Descricao = string.IsNullOrWhiteSpace(produtoVo.Descricao) ? "" : produtoVo.Descricao.Trim();
                produtoDto.Preco = produtoVo.Preco;
                produtoDto.Tipo = produtoVo.Tipo;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o produto para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtém uma lista de produtoes com filtros aplicados, podendo ser paginada
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<ProdutoDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            IQueryable<ProdutoVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os produtoes: {mensagemErro}";
                retornoDto.Retorno = false;
                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "DESCRICAO":
                        query = query.Where(p => p.Descricao.Contains(filtro.Value));
                        break;

                    case "PRECOMAIOR":
                        float precoMaior;
                        if (!float.TryParse(filtro.Value, out precoMaior))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de preço (maior que).";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.Preco >= precoMaior);
                        break;

                    case "PRECOMENOR":
                        float precoMenor;
                        if (!float.TryParse(filtro.Value, out precoMenor))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de preço (menor que).";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.Preco <= precoMenor);
                        break;

                    case "PRECO":
                        float preco;
                        if (!float.TryParse(filtro.Value, out preco))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de preço.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.Preco <= preco);
                        break;

                    case "TIPO":

                        int tipo;
                        if (!int.TryParse(filtro.Value, out tipo))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de tipo.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.Tipo == (TipoProduto)tipo);
                        break;

                    case "INATIVO":

                        bool filtroInativo;
                        if (!bool.TryParse(filtro.Value, out filtroInativo))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'inativo'.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.Inativo == filtroInativo);
                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;
                        return false;
                }
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "DESCRICAO":
                    query = query.OrderBy(p => p.Descricao).ThenBy(p => p.Tipo).ThenBy(p => p.Preco);
                    break;

                case "PRECOCRESCENTE":
                    query = query.OrderBy(p => p.Preco).ThenBy(p => p.Descricao).ThenBy(p => p.Tipo);
                    break;

                case "PRECODESCRESCENTE":
                    query = query.OrderByDescending(p => p.Preco).ThenBy(p => p.Descricao).ThenBy(p => p.Tipo);
                    break;

                default:
                    query = query.OrderBy(p => p.Descricao).ThenBy(p => p.Tipo).ThenBy(p => p.Preco);
                    break;
            }

            double totalItens = query.Count();
            double paginas = totalItens <= requisicaoDto.NumeroItensPorPagina ? 1 : totalItens / requisicaoDto.NumeroItensPorPagina;
            retornoDto.NumeroPaginas = (int)Math.Ceiling(paginas);

            int pular = (requisicaoDto.Pagina - 1) * requisicaoDto.NumeroItensPorPagina;
            query = query.Skip(pular).Take(requisicaoDto.NumeroItensPorPagina);

            if (totalItens == 0)
            {
                retornoDto.Mensagem = "Nenhum resultado encontrado.";
                retornoDto.Retorno = true;
                return true;
            }

            List<ProdutoVo> listaVo = query.ToList();
            foreach (var produto in listaVo)
            {
                ProdutoDto produtoDto = new ProdutoDto();
                if (!ConverterVoParaDto(produto, ref produtoDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;
                    return false;
                }

                retornoDto.ListaEntidades.Add(produtoDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Edita um produto
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<ProdutoDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            ProdutoVo produtoVo = new ProdutoVo();
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out produtoVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar o produto: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref produtoVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter o produto para Vo: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            if (!EditarBd(produtoVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao salvar os novos dados do produto: " + mensagemErro;
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }
    }
}
