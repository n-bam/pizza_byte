using PizzaByteBll.Base;
using PizzaByteDto.Base;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteSite.Models;
using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PizzaByteSite.Controllers
{
    public class BackupController : BaseController
    {
        /// <summary>
        /// Chama a tela com a listagem de Backup
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            // Model a ser utilizada na tela
            FiltrosBackupModel model = new FiltrosBackupModel()
            {
                Pagina = 1,
                DataFinal = DateTime.Now,
                DataInicial = DateTime.Now
            };

            string mensagemErro = "";
            if (!Utilidades.PreencherListasFiltrosBackup(model, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Obtem uma listra filtrada de logs
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public string ObterListaFiltrada(FiltrosBackupModel filtros)
        {
            //Requisição para obter a lista
            RequisicaoObterListaDto requisicaoDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "DATAINCLUSAO",
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                NaoPaginarPesquisa = filtros.NaoPaginaPesquisa,
                Pagina = filtros.Pagina,
                NumeroItensPorPagina = 10
            };

            requisicaoDto.ListaFiltros.Add("DATAINCLUSAOINICIAL", filtros.DataInicial.ToString());
            requisicaoDto.ListaFiltros.Add("DATAINCLUSAOFINAL", filtros.DataFinal.ToString());

            //Adicionar filtros utilizados
            if (!string.IsNullOrWhiteSpace(filtros.Mensagem))
            {
                requisicaoDto.ListaFiltros.Add("MENSAGEM", filtros.Mensagem.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtros.IdUsuario))
            {
                requisicaoDto.ListaFiltros.Add("IDUSUARIO", filtros.IdUsuario.ToString());
            }


            //Consumir o serviço
            BackupBll logBll = new BackupBll("LogController");
            RetornoObterListaDto<BackupDto> retornoDto = new RetornoObterListaDto<BackupDto>();
            BackupBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }
    }
}
