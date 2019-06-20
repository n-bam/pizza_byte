﻿using PizzaByteSite.Models;
using System.Collections.Generic;
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
    }
}