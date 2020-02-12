using PizzaByteBll.Base;
using PizzaByteDto.ClassesBase;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteSite.Models;
using System;
using System.Web.Mvc;

namespace PizzaByteSite.Controllers
{
    public class RecursosBdController : BaseController
    {
        /// <summary>
        /// Chama a tela com a listagem de ceps
        /// </summary>
        /// <returns></returns>
        public ActionResult Backup()
        {
            // Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            RetornoObterArquivoDto retornoDto = new RetornoObterArquivoDto();
            BaseRequisicaoDto requisicaoDto = new BaseRequisicaoDto()
            {
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            RecursosBdBll recursosBdBll = new RecursosBdBll();
            if (!recursosBdBll.FazerBackupSistema(requisicaoDto, ref retornoDto))
            {
                ViewBag.MensagemErro = retornoDto.Mensagem;
                return View("Erro");
            }

            byte[] byteArray = Convert.FromBase64String(retornoDto.ArquivoBase64);
            return File(byteArray, System.Net.Mime.MediaTypeNames.Application.Octet, "Backup " + DateTime.Now.ToShortDateString() + ".sql");
        }

        /// <summary>
        /// Restaura um backup
        /// </summary>
        /// <returns></returns>
        public ActionResult RestaurarBackup()
        {
            // Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para restaurar um backup é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            ArquivoModel model = new ArquivoModel();
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para restaurar um backup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RestaurarBackup(ArquivoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para restaurar um backup é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Converter para DTO
            RequisicaoArquivoDto requisicaoDto = new RequisicaoArquivoDto
            {
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref requisicaoDto, ref mensagemErro))
            {
                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();

            //Consumir o serviço
            RecursosBdBll recursosBll = new RecursosBdBll();
            recursosBll.RestaurarBackup(requisicaoDto, ref retorno);

            //Verificar o retorno 
            if (retorno.Retorno == false)
            {
                //Se houver erro, exibir na tela de inclusão
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            TempData["Retorno"] = "BACKUPRESTAURADO";

            //Retornar para inicio
            return RedirectToAction("Inicio", "Usuario");
        }
    }
}
