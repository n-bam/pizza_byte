using PizzaByteBll;
using PizzaByteDto.ClassesBase;
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
        /// Popula as opções para tipos de pedido
        /// </summary>
        /// <param name="listaTipos"></param>
        public static List<SelectListItem> RetornarListaTiposPedido()
        {
            List<SelectListItem> listaTipos = new List<SelectListItem>();
            listaTipos.Add(new SelectListItem { Value = TipoPedido.Entrega.ToString(), Text = "Entrega" });
            listaTipos.Add(new SelectListItem { Value = TipoPedido.Balcao.ToString(), Text = "Balcão" });
            listaTipos.Add(new SelectListItem { Value = TipoPedido.Retirada.ToString(), Text = "Retirada" });

            return listaTipos;
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
    }
}