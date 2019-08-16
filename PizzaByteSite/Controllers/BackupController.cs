using PizzaByteBll;
using PizzaByteBll.Base;
using PizzaByteDto.Base;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteSite.Models;
using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PizzaByteSite.Controllers
{
    public class BackupController : Controller
    {
        /// <summary>
        /// Chama a tela com a listagem de mensagens de sackup
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Chamar a view
            return View();
        }

        /// <summary>
        /// Obtem uma listra filtrada de mensagens
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public string ObterLista(BaseFiltrosModel filtros)
        {
            //Requisição para obter a lista
            RequisicaoObterListaDto requisicaoDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "DATADECRESCENTE",
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                NaoPaginarPesquisa = false,
                Pagina = filtros.Pagina,
                NumeroItensPorPagina = 5
            };

            //Consumir o serviço
            BackupBll sackupBll = new BackupBll(true);
            RetornoObterListaDto<BackupDto> retornoDto = new RetornoObterListaDto<BackupDto>();
            sackupBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

        /// <summary>
        /// Inclui uma mensagem de sackup
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public string IncluirMensagemBackup(BackupDto sackupDto)
        {
            RetornoDto retornoDto = new RetornoDto();
            if (sackupDto == null)
            {
                retornoDto.Mensagem = "Não foram recebidos os dados da mensagem";
                retornoDto.Retorno = false;
            }

            sackupDto.Id = Guid.NewGuid();

            //Requisição para incluir a mensagem
            RequisicaoEntidadeDto<BackupDto> requisicaoDto = new RequisicaoEntidadeDto<BackupDto>()
            {
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                EntidadeDto = sackupDto
            };

            //Consumir o serviço
            BackupBll sackupBll = new BackupBll(true);
            sackupBll.Incluir(requisicaoDto, ref retornoDto);

            RetornoGuidDto retornoGuidDto = new RetornoGuidDto()
            {
                Id = sackupDto.Id,
                Retorno = retornoDto.Retorno,
                Mensagem = retornoDto.Mensagem
            };

            string retorno = new JavaScriptSerializer().Serialize(retornoGuidDto);
            return retorno;
        }

        /// <summary>
        /// Exclui uma mensagem de sackup
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public string ExcluirMensagem(Guid idMensagem)
        {
            RetornoDto retornoDto = new RetornoDto();
            if (idMensagem == Guid.Empty)
            {
                retornoDto.Mensagem = "O id da mensagem não foi informado.";
                retornoDto.Retorno = false;
            }

            //Requisição para excluir
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                Id = idMensagem
            };

            //Consumir o serviço
            BackupBll sackupBll = new BackupBll(true);
            sackupBll.Excluir(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }
    }
}