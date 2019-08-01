using PizzaByteSite.Models;
using System;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Controllers
{
    public class PedidoController : Controller
    {
        ///// <summary>
        ///// Chama a tela com a listagem dos pedidos cadastrados
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult Index(FiltrosPedidoModel filtros = null)
        //{
        //    //Iniciar uma model, se estiver vazia
        //    if (filtros == null)
        //    {
        //        filtros = new FiltrosPedidoModel()
        //        {
        //            NomeCliente = "",
        //            IdCliente = Guid.Empty,
        //            Tipo = TipoPedido.NaoIdentificado,
        //            Pagina = 1
        //        };
        //    }

        //    PedidoModel pedido1 = new PedidoModel()
        //    {
        //        Id = Guid.NewGuid(),
        //        NomeCliente = "Nathália Nascimento",
        //        Inativo = false,
        //        Total = 24.5,
        //        Tipo = TipoPedido.Retirada,
        //        DataInclusao = DateTime.Now.AddDays(-4),
        //        DataAlteracao = null
        //    };

        //    PedidoModel pedido2 = new PedidoModel()
        //    {
        //        Id = Guid.NewGuid(),
        //        NomeCliente = "Adriele Ramos",
        //        Inativo = false,
        //        Total = 78.8,
        //        Tipo = TipoPedido.Entrega,
        //        DataInclusao = DateTime.Now.AddDays(-4),
        //        DataAlteracao = null
        //    };

        //    PedidoModel pedido3 = new PedidoModel()
        //    {
        //        Id = Guid.NewGuid(),
        //        NomeCliente = "Bárbara Cocato",
        //        Inativo = false,
        //        Total = 15.6,
        //        Tipo = TipoPedido.Entrega,
        //        DataInclusao = DateTime.Now.AddDays(-4),
        //        DataAlteracao = null
        //    };

        //    PedidoModel pedido4 = new PedidoModel()
        //    {
        //        Id = Guid.NewGuid(),
        //        NomeCliente = "Mirella Andreoli",
        //        Inativo = false,
        //        Total = 37.6,
        //        Tipo = TipoPedido.Entrega,
        //        DataInclusao = DateTime.Now.AddDays(-4),
        //        DataAlteracao = null
        //    };


        //    return View(filtros);
        //}

        ///// <summary>
        ///// Chama a tela para inclusão de um novo pedido
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult Incluir()
        //{
        //    PedidoModel model = new PedidoModel()
        //    {
        //        NomeCliente = "Alberto Oliveira",
        //        Obs = "Sem azeitonas",
        //        ClienteEndereco = new ClienteEnderecoModel()
        //        {
        //            Cep = "13084-200",
        //            NumeroEndereco = "247",
        //            EnderecoCep = new CepModel()
        //            {
        //                Bairro = "Jd das Palmeiras",
        //                Cidade = "Americana",
        //                Cep = "13084-200",
        //                Logradouro = "Rua dos Ipês",
        //            }
        //        }
        //    };

        //    return View(model);
        //}

        ///// <summary>
        ///// Post para incluir um pedido
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Incluir(PedidoModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    TempData.Add("Retorno", "INCLUIDO");
        //    return RedirectToAction("Index");
        //}

        ///// <summary>
        ///// Chama a tela para alterar um pedido
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult Editar(Guid id)
        //{
        //    PedidoModel model = new PedidoModel()
        //    {
        //        Obs = "Não colocar cebola",
        //        Id = id,
        //        IdCliente = Guid.NewGuid(),
        //        RecebidoCartao = 27.3,
        //        RecebidoDinheiro = 0,
        //        TaxaEntrega = 2.8,
        //        Troco = 0,
        //        Inativo = false,
        //        Total = 24.5,
        //        Tipo = TipoPedido.Balcao,
        //        DataInclusao = DateTime.Now.AddDays(-4),
        //        DataAlteracao = DateTime.Now.AddDays(-2),
        //    };

        //    return View(model);
        //}

        ///// <summary>
        ///// Post para salvar as alterações de um pedido
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Editar(PedidoModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    TempData["Retorno"] = "ALTERADO";
        //    return RedirectToAction("Index");
        //}

        ///// <summary>
        ///// Chama a tela para visualizar um pedido
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult Visualizar(Guid id)
        //{
        //    PedidoModel model = new PedidoModel()
        //    {
        //        Obs = "Não colocar cebola",
        //        Id = id,
        //        IdCliente = Guid.NewGuid(),
        //        RecebidoCartao = 27.3,
        //        RecebidoDinheiro = 0,
        //        TaxaEntrega = 2.8,
        //        Troco = 0,
        //        Inativo = false,
        //        Total = 24.5,
        //        Tipo = TipoPedido.Balcao,
        //        DataInclusao = DateTime.Now.AddDays(-4),
        //        DataAlteracao = DateTime.Now.AddDays(-2),
        //    };

        //    return View(model);
        //}

        ///// <summary>
        ///// Método get para excluir um pedido
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public ActionResult Excluir(ExclusaoModel model)
        //{
        //    return View(model);
        //}

        ///// <summary>
        ///// Post para excluir um pedido
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExcluirPedido(ExclusaoModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    TempData["Retorno"] = "EXCLUIDO";
        //    return RedirectToAction("Index");
        //}
    }
}