using PizzaByteBll;
using PizzaByteDto.ClassesBase;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;
namespace PizzaByteSite
{
    public static class Utilidades
    {
        /// <summary>
        /// Popula as opções para tipos de produtos
        /// </summary>
        /// <param name="listaTipos"></param>
        public static List<SelectListItem> RetornarListaTiposProduto()
        {
            List<SelectListItem> listaTipos = new List<SelectListItem>();
            listaTipos.Add(new SelectListItem { Value = TipoProduto.Pizza.ToString(), Text = "Pizza" });
            listaTipos.Add(new SelectListItem { Value = TipoProduto.Bebida.ToString(), Text = "Bebida" });

            return listaTipos;
        }

        /// <summary>
        /// Popula as opções para tipos de funcionario
        /// </summary>
        /// <param name="listaTipos"></param>
        public static List<SelectListItem> RetornarListaTiposFuncionario()
        {
            List<SelectListItem> listaTipos = new List<SelectListItem>();
            TipoFuncionario[] elementos = Enum.GetValues(typeof(TipoFuncionario)) as TipoFuncionario[];
            foreach (var recurso in elementos)
            {
                listaTipos.Add(new SelectListItem()
                {
                    Text = recurso.ToString(),
                    Value = ((int)recurso).ToString()
                });
            }

            listaTipos.RemoveAt(0);
            listaTipos = listaTipos.OrderBy(p => p.Text).ToList();
            return listaTipos;
        }

        /// <summary>
        /// Retorna uma lista de funcinários de acordo com o tipo passado
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> RetornarListaFuncionarios(TipoFuncionario tipo)
        {
            RequisicaoObterListaDto requisicaoDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "NOME",
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                NaoPaginarPesquisa = true,
            };

            if (tipo != TipoFuncionario.NaoIdentificado)
            {
                requisicaoDto.ListaFiltros.Add("TIPO", ((int)tipo).ToString());
            }

            requisicaoDto.ListaFiltros.Add("INATIVO", "false");

            FuncionarioBll funcionarioBll = new FuncionarioBll(false);
            RetornoObterListaDto<FuncionarioDto> retornoDto = new RetornoObterListaDto<FuncionarioDto>();
            if (!funcionarioBll.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return null;
            }

            List<SelectListItem> listaRetorno = new List<SelectListItem>();
            listaRetorno.Add(new SelectListItem()
            {
                Value = "null",
                Text = "Todos"
            });

            foreach (var func in retornoDto.ListaEntidades)
            {
                listaRetorno.Add(new SelectListItem()
                {
                    Value = func.Id.ToString(),
                    Text = func.Nome
                });
            }

            return listaRetorno;
        }

        /// <summary>
        /// Popula as opções para tipos de pedido
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> RetornarListaTiposPedido()
        {
            List<SelectListItem> listaTipos = new List<SelectListItem>();
            TipoPedido[] elementos = Enum.GetValues(typeof(TipoPedido)) as TipoPedido[];
            foreach (var recurso in elementos)
            {
                listaTipos.Add(new SelectListItem()
                {
                    Text = recurso.ToString(),
                    Value = ((int)recurso).ToString()
                });
            }

            listaTipos.RemoveAt(0);
            listaTipos = listaTipos.OrderBy(p => p.Text).ToList();

            return listaTipos;
        }

        /// <summary>
        /// Popula as opções para tipos de conta
        /// </summary>
        /// <param name="listaStatus"></param>
        public static List<SelectListItem> RetornarListaStatusConta()
        {
            List<SelectListItem> listaStatus = new List<SelectListItem>();
            StatusConta[] elementos = Enum.GetValues(typeof(StatusConta)) as StatusConta[];
            foreach (var recurso in elementos)
            {
                listaStatus.Add(new SelectListItem()
                {
                    Text = recurso.ToString(),
                    Value = ((int)recurso).ToString()
                });
            }

            listaStatus.RemoveAt(0);
            listaStatus = listaStatus.OrderBy(p => p.Text).ToList();
            return listaStatus;
        }

        /// <summary>
        /// Popula as opções para entidades inativas
        /// </summary>
        /// <param name="listaTipos"></param>
        public static List<SelectListItem> RetornarListaOpcaoInativo()
        {
            List<SelectListItem> listaInativo = new List<SelectListItem>();
            listaInativo.Add(new SelectListItem() { Text = "Todos", Value = "" });
            listaInativo.Add(new SelectListItem() { Text = "Apenas ativos", Value = "false", Selected = true });
            listaInativo.Add(new SelectListItem() { Text = "Apenas inativos", Value = "true" });

            return listaInativo;
        }

        /// <summary>
        /// Popula as opções para ordenar clientes
        /// </summary>
        /// <param name="listaTipos"></param>
        public static List<SelectListItem> RetornarListaOpcaoOrdenacaoCliente()
        {
            List<SelectListItem> listaCampoOrdem = new List<SelectListItem>();
            listaCampoOrdem.Add(new SelectListItem() { Text = "Nome", Value = "NOME", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Telefone", Value = "TELEFONE" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Data cadastro crescente", Value = "DATACADASTROCRESCENTE" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Data cadastro decrescente", Value = "DATACADASTRODECRESCENTE" });

            return listaCampoOrdem;
        }

        /// <summary>
        /// Popula as opções para ordenar fornecedores
        /// </summary>
        /// <param name="listaTipos"></param>
        public static List<SelectListItem> RetornarListaOpcaoOrdenacaoFornecedores()
        {
            List<SelectListItem> listaCampoOrdem = new List<SelectListItem>();
            listaCampoOrdem.Add(new SelectListItem() { Text = "Nome Fantasia", Value = "NOMEFANTASIA", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Razao Social", Value = "RAZAOSOCIAL", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Telefone", Value = "TELEFONE" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Cnpj", Value = "CNPJ" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Data cadastro crescente", Value = "DATACADASTROCRESCENTE" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Data cadastro decrescente", Value = "DATACADASTRODECRESCENTE" });

            return listaCampoOrdem;
        }

        /// <summary>
        /// Popula as opções para ordenar contas
        /// </summary>
        /// <param name="listaTipos"></param>
        public static List<SelectListItem> RetornarListaOpcaoOrdenacaoContaPagar()
        {
            List<SelectListItem> listaCampoOrdem = new List<SelectListItem>();
            listaCampoOrdem.Add(new SelectListItem() { Text = "Competência", Value = "DATACOMPETENCIA", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Vencimento", Value = "DATAVENCIMENTO" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Descrição", Value = "DESCRICAO" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Valor", Value = "VALOR" });
           

            return listaCampoOrdem;
        }

        /// <summary>
        /// Popula as opções para ordenar motoboys
        /// </summary>
        /// <param name="listaTipos"></param>
        public static List<SelectListItem> RetornarListaOpcaoOrdenacaoMotoboy()
        {
            List<SelectListItem> listaCampoOrdem = new List<SelectListItem>();
            listaCampoOrdem.Add(new SelectListItem() { Text = "Nome", Value = "NOME", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Taxa", Value = "VALOR", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Telefone", Value = "TELEFONE", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Quantidade", Value = "QUANTIDADE" });

            return listaCampoOrdem;
        }

        /// <summary>
        /// Popula as opções para ordenar produtos
        /// </summary>
        /// <param name="listaTipos"></param>
        public static List<SelectListItem> RetornarListaOpcaoOrdenacaoProduto()
        {
            List<SelectListItem> listaCampoOrdem = new List<SelectListItem>();
            listaCampoOrdem.Add(new SelectListItem() { Text = "Descrição", Value = "DESCRICAO", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Preço", Value = "PRECO" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Tipo", Value = "TIPO" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Data cadastro crescente", Value = "DATACADASTROCRESCENTE" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Data cadastro decrescente", Value = "DATACADASTRODECRESCENTE" });

            return listaCampoOrdem;
        }

        /// <summary>
        /// Popula as opções para ordenar taxas de entrega
        /// </summary>
        /// <param name="listaTipos"></param>
        public static List<SelectListItem> RetornarListaOpcaoOrdenacaoTaxaEntrega()
        {
            List<SelectListItem> listaCampoOrdem = new List<SelectListItem>();
            listaCampoOrdem.Add(new SelectListItem() { Text = "Bairro", Value = "BAIRROCIDADE", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Valor da Taxa", Value = "VALORTAXA" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Data cadastro crescente", Value = "DATACADASTROCRESCENTE" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Data cadastro decrescente", Value = "DATACADASTRODECRESCENTE" });

            return listaCampoOrdem;
        }

        /// <summary>
        /// Popula as opções para ordenar de entrega
        /// </summary>
        /// <param name="listaTipos"></param>
        public static List<SelectListItem> RetornarListaOpcaoOrdenacaoEntrega()
        {
            List<SelectListItem> listaCampoOrdem = new List<SelectListItem>();
            listaCampoOrdem.Add(new SelectListItem() { Text = "Endereço", Value = "ENDERECO", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Funcionário", Value = "FUNCIONARIO" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Valor retornado", Value = "VALORRETORNO" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Pedidos por data crescente", Value = "DATACADASTROCRESCENTE" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Pedidos por data decrescente", Value = "DATACADASTRODECRESCENTE" });

            return listaCampoOrdem;
        }

        /// <summary>
        /// Popula as opções para ordenar pedidos
        /// </summary>
        /// <param name="listaTipos"></param>
        public static List<SelectListItem> RetornarListaOpcaoOrdenacaoPedido()
        {
            List<SelectListItem> listaCampoOrdem = new List<SelectListItem>();
            listaCampoOrdem.Add(new SelectListItem() { Text = "Data crescente", Value = "DATACADASTROCRESCENTE", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Data decrescente", Value = "DATACADASTRODECRESCENTE", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Valor total", Value = "TOTAL" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Cliente", Value = "NOMECLIENTE" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Taxa de entrega", Value = "TAXAENTREGA" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Tipo", Value = "TIPO" });

            return listaCampoOrdem;
        }

        /// <summary>
        /// Popula as opções para ordenar pedidos
        /// </summary>
        /// <param name="listaTipos"></param>
        public static List<SelectListItem> RetornarListaOpcaoOrdenacaoPedidoPorBairro()
        {
            List<SelectListItem> listaCampoOrdem = new List<SelectListItem>();
            listaCampoOrdem.Add(new SelectListItem() { Text = "Bairro", Value = "BAIRRO", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Cidade", Value = "CIDADE" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Valor total", Value = "VALORCRESCENTE" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Taxa de entrega", Value = "TAXAENTREGA", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Valor descrescente", Value = "VALORDECRESCENTE" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Taxa de entrega descrescente", Value = "TAXAENTREGADECRESCENTE" });

            return listaCampoOrdem;
        }

        /// <summary>
        /// Popula as opções para ordenar clientes
        /// </summary>
        /// <param name="listaTipos"></param>
        public static List<SelectListItem> RetornarListaOpcaoOrdenacaoMelhoresProdutos()
        {
            List<SelectListItem> listaCampoOrdem = new List<SelectListItem>();
            listaCampoOrdem.Add(new SelectListItem() { Text = "Quantidade", Value = "QUANTIDADE", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Valor", Value = "VALOR" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Descrição", Value = "DESCRICAO" });

            return listaCampoOrdem;
        }

        /// <summary>
        /// Popula as opções para pedidos de IFood
        /// </summary>
        /// <param name="listaTipos"></param>
        public static List<SelectListItem> RetornarListaOpcaoIFood()
        {
            List<SelectListItem> listaOpcoes = new List<SelectListItem>();
            listaOpcoes.Add(new SelectListItem() { Text = "Todos", Value = "", Selected = true });
            listaOpcoes.Add(new SelectListItem() { Text = "Apenas IFood", Value = "true" });
            listaOpcoes.Add(new SelectListItem() { Text = "Apenas internos", Value = "false" });

            return listaOpcoes;
        }
        
        /// <summary>
        /// Popula as opções para ordenar clientes
        /// </summary>
        /// <param name="listaTipos"></param>
        public static List<SelectListItem> RetornarListaOpcaoOrdenacaoMelhoresClientes()
        {
            List<SelectListItem> listaCampoOrdem = new List<SelectListItem>();
            listaCampoOrdem.Add(new SelectListItem() { Text = "Nome", Value = "NOME", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Telefone", Value = "TELEFONE" });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Valor", Value = "VALOR", Selected = true });
            listaCampoOrdem.Add(new SelectListItem() { Text = "Quantidade", Value = "QUANTIDADE" });
           


            return listaCampoOrdem;
        }

        /// <summary>
        /// Retorna as opções de pesquisa por data de contas
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> RetornarOpcoesPesquisaContas()
        {
            List<SelectListItem> listaOpcoes = new List<SelectListItem>();
            listaOpcoes.Add(new SelectListItem() { Text = "Vencimento", Value = "DATAVENCIMENTO", Selected = true });
            listaOpcoes.Add(new SelectListItem() { Text = "Competência", Value = "DATACOMPETENCIA" });
            listaOpcoes.Add(new SelectListItem() { Text = "Pagamento", Value = "DATAPAGAMENTO" });

            return listaOpcoes;
        }

        /// <summary>
        /// Valide se o endereço foi preenchido, caso o CEP seja informado
        /// </summary>
        /// <param name="model"></param>
        /// <param name="erros"></param>
        /// <returns></returns>
        public static bool ValidarEndereco(CepModel model, ref Dictionary<string, string> erros)
        {
            if (string.IsNullOrWhiteSpace(model.Cep))
            {
                return true;
            }

            bool retorno = true;
            if (string.IsNullOrWhiteSpace(model.Logradouro))
            {
                erros.Add("Endereco.Logradouro", "O logradouro é obrigatório para incluir o endereço.");
                retorno = false;
            }

            if (string.IsNullOrWhiteSpace(model.Bairro))
            {
                erros.Add("Endereco.Bairro", "O bairro é obrigatório para incluir o endereço.");
                retorno = false;
            }

            if (string.IsNullOrWhiteSpace(model.Cidade))
            {
                erros.Add("Endereco.Cidade", "A cidade é obrigatória para incluir o endereço.");
                retorno = false;
            }

            return retorno;
        }

        /// <summary>
        /// Preenche as listas de filtros dos logs
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool PreencherListasFiltrosLog(FiltrosLogModel model, ref string mensagemErro)
        {
            // Preencher todos os recursos disponíveis
            LogRecursos[] elementos = Enum.GetValues(typeof(LogRecursos)) as LogRecursos[];
            foreach (var recurso in elementos)
            {
                model.ListaRecursos.Add(new SelectListItem()
                {
                    Text = recurso.ToString(),
                    Value = ((int)recurso).ToString()
                });
            }

            model.ListaRecursos.RemoveAt(0);
            model.ListaRecursos = model.ListaRecursos.OrderBy(p => p.Text).ToList();

            UsuarioBll usuarioBll = new UsuarioBll(false);
            RetornoObterDicionarioDto<Guid, string> retornoDto = new RetornoObterDicionarioDto<Guid, string>();
            BaseRequisicaoDto requisicaoDto = new BaseRequisicaoDto()
            {
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            // Obter a lista de usuários cadastrados
            if (!usuarioBll.ObterListaParaSelecao(requisicaoDto, ref retornoDto))
            {
                mensagemErro = retornoDto.Mensagem;
                return false;
            }

            // Popular a lista da model com os usuários retornados
            foreach (var usuario in retornoDto.ListaEntidades)
            {
                model.ListaUsuarios.Add(new SelectListItem()
                {
                    Text = usuario.Value,
                    Value = usuario.Key.ToString()
                });
            }

            return true;
        }

        /// <summary>
        /// Retorna o id do produto da promoção
        /// </summary>
        /// <returns></returns>
        public static Guid RetornaIdProdutoPromocao()
        {
            return new Guid("E6219299-E232-43C8-B07E-C7B1CAD8C19D");
        }

        /// <summary>
        /// Retorna um texto com os filtros utilizados
        /// </summary>
        /// <param name="model"></param>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public static bool RetornarFiltroPreenchidosPedido(FiltrosListagemPedidoModel model, ref string filtros)
        {
            if (model.IdCliente != null)
            {
                filtros += $"Pedidos do cliente: {model.NomeCliente}. \n";
            }

            if (model.IndicadorCredito)
            {
                filtros += $"Apenas pedidos com cartão de crédito. \n";
            }

            if (model.IndicadorDebito)
            {
                filtros += $"Apenas pedidos com cartão de débito. \n";
            }

            if (model.IndicadorDinheiro)
            {
                filtros += $"Apenas pedidos no dinheiro. \n";
            }

            if (model.IndicadorPromocao)
            {
                filtros += $"Apenas pedidos da promoção (seg/qui). \n";
            }

            if (!string.IsNullOrWhiteSpace(model.JustificativaCancelamento))
            {
                filtros += $"Com a justificativa de estorno: {model.JustificativaCancelamento.Trim()} \n";
            }

            if (!string.IsNullOrWhiteSpace(model.Obs))
            {
                filtros += $"Com as observações: {model.Obs.Trim()} \n";
            }

            if (!string.IsNullOrWhiteSpace(model.ObterInativos))
            {
                filtros += $"Apenas pedidos {(model.ObterInativos.Trim() == "false" ? " não estornados." : "estornados.")} \n";
            }

            if (!string.IsNullOrWhiteSpace(model.PedidoIfood))
            {
                filtros += $"Apenas pedidos {(model.PedidoIfood.Trim() == "false" ? " internos." : "via IFood.")} \n";
            }

            if (model.TaxaEntregaInicial > 0)
            {
                filtros += $"Apenas pedidos com taxa de entrega maior que R${String.Format("{0:0.00}", model.TaxaEntregaInicial)} ";
            }

            if (model.TaxaEntregaFinal > 0)
            {
                if (model.TaxaEntregaInicial > 0)
                {
                    filtros += $"e menor que R${String.Format("{0:0.00}", model.TaxaEntregaFinal)} \n";
                }
                else
                {
                    filtros += $"Apenas pedidos com taxa de entrega menor que R${String.Format("{0:0.00}", model.TaxaEntregaFinal)} \n";
                }
            }

            if (model.Tipo != TipoPedido.NaoIdentificado)
            {
                filtros += $"Apenas pedidos do tipo '{model.Tipo.ToString()}'. \n";
            }

            if (model.TotalInicial > 0)
            {
                filtros += $"Apenas pedidos com total maior que R${String.Format("{0:0.00}", model.TotalInicial)} ";
            }

            if (model.TotalFinal > 0)
            {
                if (model.TotalInicial > 0)
                {
                    filtros += $"e menor que R${String.Format("{0:0.00}", model.TotalFinal)}. \n";
                }
                else
                {
                    filtros += $"Apenas pedidos com total menor que R${String.Format("{0:0.00}", model.TotalFinal)}. \n";
                }
            }

            if (model.TrocoInicial > 0)
            {
                filtros += $"Apenas pedidos com troco maior que R${String.Format("{0:0.00}", model.TrocoInicial)} ";
            }

            if (model.TrocoFinal > 0)
            {
                if (model.TotalInicial > 0)
                {
                    filtros += $"e menor que R${String.Format("{0:0.00}", model.TrocoFinal)}. \n";
                }
                else
                {
                    filtros += $"Apenas pedidos com troco menor que R${String.Format("{0:0.00}", model.TrocoFinal)}. \n";
                }
            }

            return true;
        }

        /// <summary>
        /// Retorna um texto com os filtros utilizados
        /// </summary>
        /// <param name="model"></param>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public static bool RetornarFiltroPreenchidosContas(FiltrosListagemContasModel model, ref string filtros)
        {
            if (!string.IsNullOrWhiteSpace(model.Descricao))
            {
                filtros += $"Com a descrição: {model.Descricao.Trim()} \n";
            }

            if (model.PrecoFim > 0)
            {
                filtros += $"Apenas contas valor menor que R${String.Format("{0:0.00}", model.PrecoFim)} ";
            }

            if (model.PrecoInicio > 0)
            {
                filtros += $"Apenas contas valor maior que R${String.Format("{0:0.00}", model.PrecoInicio)} ";
            }

            if (model.Status != StatusConta.NaoIdentificado)
            {
                filtros += $"Apenas pedidos do tipo '{model.Status.ToString()}'. \n";
            }

            return true;
        }

    }
}