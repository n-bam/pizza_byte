using PizzaByteBll.Base;
using PizzaByteDal;
using PizzaByteDto.ClassesBase;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteVo;
using PizzaByteVo.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteBll
{
    public class UsuarioBll : BaseBll<UsuarioVo, UsuarioDto>
    {
        private static LogBll logBll = new LogBll("UsuarioBll");
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public UsuarioBll(bool salvarAlteracoes) : base(logBll)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public UsuarioBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(logBll, contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui um usuario no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<UsuarioDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para incluir um novo usuário é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirUsuario, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            UsuarioVo usuarioVo = new UsuarioVo();

            // Converte para VO a ser incluída no banco de dados
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref usuarioVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o usuario para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirUsuario, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(usuarioVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o usuario para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirUsuario, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirUsuario, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui um usuário do banco de dados a partir do ID
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar os usuários é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirUsuario, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirUsuario, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Faz o login com email e senha
        /// </summary>
        public bool FazerLogin(RequisicaoFazerLoginDto requisicaoDto, ref RetornoFazerLoginDto retornoDto)
        {
            LogVo logVo = new LogVo()
            {
                Id = Guid.NewGuid(),
                IdEntidade = Guid.Empty,
                IdUsuario = Guid.Empty,
                Recurso = LogRecursos.FazerLogin
            };

            //Validar email e senha
            if (string.IsNullOrWhiteSpace(requisicaoDto.Email))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "O email é obrigatório para fazer o login";

                logVo.Mensagem = retornoDto.Mensagem;
                logBll.RegistrarLogVo(logVo);
                return false;
            }

            if (string.IsNullOrWhiteSpace(requisicaoDto.Senha))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "A senha é obrigatória para fazer o login";

                logVo.Mensagem = retornoDto.Mensagem;
                logBll.RegistrarLogVo(logVo);
                return false;
            }

            string nomeUsuario = "Suporte";
            Guid idUsuario = UtilitarioBll.RetornarIdUsuarioSuporte();
            bool usuarioAdm = false;

            //Se for o usuário suporte
            if (requisicaoDto.Email.Trim().ToUpper() == "SUPORTE")
            {
                string senhaCriptografada = "";
                UtilitarioBll.CriptografarSenha(DateTime.Now.AddDays(-2).Date.ToString("dd/MM/yyyy").Replace("/", ""), ref senhaCriptografada);

                if (requisicaoDto.Senha.Trim() != senhaCriptografada)
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Senha de suporte incorreta.";

                    logVo.Mensagem = retornoDto.Mensagem;
                    logBll.RegistrarLogVo(logVo);
                    return false;
                }

                usuarioAdm = true;
            }
            else
            {
                string mensagemErro = "";
                IQueryable<UsuarioVo> query;
                if (!ObterQueryBd(out query, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = $"Falha ao listar os usuários: {mensagemErro}";

                    logVo.Mensagem = retornoDto.Mensagem;
                    logBll.RegistrarLogVo(logVo);
                    return false;
                }

                UsuarioVo usuarioVo;

                //Procurar o email com o flag ativo
                query = query.Where(u => u.Email.Trim() == requisicaoDto.Email.Trim() && u.Inativo == false);

                try
                {
                    usuarioVo = query.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = $"Falha ao obter o usuário do banco de dados: {ex.Message}";

                    logVo.Mensagem = retornoDto.Mensagem;
                    logBll.RegistrarLogVo(logVo);
                    return false;
                }

                if (usuarioVo == null)
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = $"Email ou senha inválidos. ";

                    logVo.Mensagem = retornoDto.Mensagem;
                    logBll.RegistrarLogVo(logVo);
                    return false;
                }

                if (!requisicaoDto.Senha.Equals(usuarioVo.Senha))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = $"Email ou senha inválidos ";

                    logVo.Mensagem = retornoDto.Mensagem;
                    logBll.RegistrarLogVo(logVo);
                    return false;
                }

                usuarioAdm = usuarioVo.Administrador;
                nomeUsuario = usuarioVo.Nome;
                idUsuario = usuarioVo.Id;
            }

            string identificacao = DateTime.Now.ToString("dd/MM/yyyy hh:mm") + UtilitarioBll.RetornaGuidValidação() + idUsuario.ToString() + $"Adm={(usuarioAdm ? "1" : "0")}";
            string identificacaoCriptografada = "";

            if (!UtilitarioBll.CriptografarIdentificacao(identificacao, ref identificacaoCriptografada))
            {
                retornoDto.Mensagem = "Falha ao fazer o login: Não foi possível obter a identificação.";
                retornoDto.Retorno = false;

                logVo.Mensagem = retornoDto.Mensagem;
                logBll.RegistrarLogVo(logVo);
                return false;
            }

            retornoDto.IdUsuario = idUsuario;
            retornoDto.NomeUsuario = nomeUsuario;
            retornoDto.Identificacao = identificacaoCriptografada;
            retornoDto.UsuarioAdministrador = usuarioAdm;
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Envia um email com uma nova senha
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        public bool EnviarEmailRecuperacao(RequisicaoFazerLoginDto requisicaoDto, RetornoDto retornoDto)
        {
            LogVo logVo = new LogVo()
            {
                Id = Guid.NewGuid(),
                IdEntidade = Guid.Empty,
                IdUsuario = Guid.Empty,
                Recurso = LogRecursos.FazerLogin
            };

            if (string.IsNullOrWhiteSpace(requisicaoDto.Email))
            {
                retornoDto.Mensagem = "Informe o email para recuperar a senha.";
                retornoDto.Retorno = false;

                logVo.Mensagem = retornoDto.Mensagem;
                logBll.RegistrarLogVo(logVo);
                return false;
            }

            string mensagemErro = "";
            IQueryable<UsuarioVo> query;
            if (!ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = $"Erro ao listar os usuários: {mensagemErro}";

                logVo.Mensagem = retornoDto.Mensagem;
                logBll.RegistrarLogVo(logVo);
                return false;
            }

            //Procurar o email com o flag válido
            UsuarioVo usuarioVo;
            query = query.Where(u => u.Email.Trim() == requisicaoDto.Email.Trim() && u.Inativo == false);

            try
            {
                usuarioVo = query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = $"Falha ao obter o usuário do banco de dados: {ex.Message}";

                logVo.Mensagem = retornoDto.Mensagem;
                logBll.RegistrarLogVo(logVo);
                return false;
            }

            if (usuarioVo == null)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = $"Email não encontrado.";

                logVo.Mensagem = retornoDto.Mensagem;
                logBll.RegistrarLogVo(logVo);
                return false;
            }

            string opcoes = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@abcdefghijklmnopqrstuvxzwy";
            Random random = new Random();
            string senha = new string(Enumerable.Repeat(opcoes, 8).Select(s => s[random.Next(s.Length)]).ToArray());

            string senhaCriptografada = "";
            UtilitarioBll.CriptografarSenha(senha, ref senhaCriptografada);
            usuarioVo.Senha = senhaCriptografada;

            if (!EditarBd(usuarioVo, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Erro ao editar o usuário: {retornoDto.Mensagem}";

                logVo.Mensagem = retornoDto.Mensagem;
                logBll.RegistrarLogVo(logVo);
                return false;
            }

            UsuarioDto usuarioDto = new UsuarioDto();
            if (!ConverterVoParaDto(usuarioVo, ref usuarioDto, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Erro ao converter o usuário de Vo para Dto: {mensagemErro}";
                retornoDto.Retorno = false;

                logVo.Mensagem = retornoDto.Mensagem;
                logBll.RegistrarLogVo(logVo);
                return false;
            }

            string corpoEmail = $"<p> Olá <strong>{usuarioVo.Nome}</strong></p>" +
                                  "<p> Sua senha para acessar o sistema PizzaByte foi recuperada. Você poderá utilizar essa senha para acessar " +
                                  "o sistema e, se desejar, você poderá alterar esta senha editando o seu usuário.</p>" +
                                  $"<p> Sua nova senha é: <strong>{senha}</strong></p><br/>" +
                                  "<p> Por favor não responda este e-mail.</p>";

            if (!UtilitarioBll.EnviarEmail(usuarioVo.Email, "Recuperação de senha - PizzaByte", corpoEmail, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Problemas para enviar o email com a nova senha. Se o erro persistir, entre em contato com o suporte. Mensagem: " + mensagemErro;
                retornoDto.Retorno = false;

                logVo.Mensagem = retornoDto.Mensagem;
                logBll.RegistrarLogVo(logVo);
                return false;
            }

            // Salva as alterações
            if (!pizzaByteContexto.Salvar(ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Problemas para salvar a nova senha: " + mensagemErro;

                logVo.Mensagem = retornoDto.Mensagem;
                logBll.RegistrarLogVo(logVo);
                return false;
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Obtém um usuário pelo ID (não traz a senha no objeto)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<UsuarioDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar os usuários é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterUsuario, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            UsuarioVo usuarioVo;
            if (!ObterPorIdBd(requisicaoDto.Id, out usuarioVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o usuario: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterUsuario, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            retornoDto.Mensagem = "Ok";
            if (usuarioVo == null)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Usuário não encontrado";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterUsuario, usuarioVo.Id, retornoDto.Mensagem);
                return false;
            }

            UsuarioDto usuarioDto = new UsuarioDto();
            if (!ConverterVoParaDto(usuarioVo, ref usuarioDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o usuário: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterUsuario, usuarioVo.Id, retornoDto.Mensagem);
                return false;
            }

            retornoDto.Entidade = usuarioDto;
            retornoDto.Entidade.Senha = "";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Converte um usuario Dto para um usuario Vo
        /// </summary>
        /// <param name="usuarioDto"></param>
        /// <param name="usuarioVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(UsuarioDto usuarioDto, ref UsuarioVo usuarioVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(usuarioDto, ref usuarioVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                usuarioVo.Email = string.IsNullOrWhiteSpace(usuarioDto.Email) ? "" : usuarioDto.Email.Trim();
                usuarioVo.Nome = string.IsNullOrWhiteSpace(usuarioDto.Nome) ? "" : usuarioDto.Nome.Trim();
                usuarioVo.Senha = string.IsNullOrWhiteSpace(usuarioDto.Senha) ? "" : usuarioDto.Senha.Trim();
                usuarioVo.Id = usuarioDto.Id;
                usuarioVo.Administrador = usuarioDto.Administrador;
                usuarioVo.Inativo = usuarioDto.Inativo;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o usuario para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um usuario Dto para um usuario Vo
        /// </summary>
        /// <param name="usuarioVo"></param>
        /// <param name="usuarioDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(UsuarioVo usuarioVo, ref UsuarioDto usuarioDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(usuarioVo, ref usuarioDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                usuarioDto.Email = string.IsNullOrWhiteSpace(usuarioVo.Email) ? "" : usuarioVo.Email.Trim();
                usuarioDto.Nome = string.IsNullOrWhiteSpace(usuarioVo.Nome) ? "" : usuarioVo.Nome.Trim();
                usuarioDto.Senha = string.IsNullOrWhiteSpace(usuarioVo.Senha) ? "" : usuarioVo.Senha.Trim();
                usuarioDto.Administrador = usuarioVo.Administrador;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o usuario para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtém uma lista de usuários com filtros aplicados, podendo ser paginada (não traz a senha nos objetos)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<UsuarioDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar os usuários é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaUsuario, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Obter a query primária
            IQueryable<UsuarioVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os usuários: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaUsuario, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "EMAIL":
                        query = query.Where(p => p.Email.Contains(filtro.Value));
                        break;

                    case "NOME":
                        query = query.Where(p => p.Nome.Contains(filtro.Value));
                        break;

                    case "ADMINISTRADOR":

                        bool filtroAdministrador;
                        if (!bool.TryParse(filtro.Value, out filtroAdministrador))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'administrador'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaUsuario, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Administrador == filtroAdministrador);
                        break;

                    case "INATIVO":

                        bool filtroInativo;
                        if (!bool.TryParse(filtro.Value, out filtroInativo))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'inativo'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaUsuario, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Inativo == filtroInativo);
                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaUsuario, Guid.Empty, retornoDto.Mensagem);
                        return false;
                }
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "NOME":
                    query = query.OrderBy(p => p.Nome);
                    break;

                case "EMAIL":
                    query = query.OrderBy(p => p.Email);
                    break;

                default:
                    query = query.OrderBy(p => p.Nome);
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

            List<UsuarioVo> listaVo = query.ToList();
            foreach (var usuario in listaVo)
            {
                UsuarioDto usuarioDto = new UsuarioDto();
                if (!ConverterVoParaDto(usuario, ref usuarioDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaUsuario, usuario.Id, retornoDto.Mensagem);
                    return false;
                }

                usuarioDto.Senha = "";
                retornoDto.ListaEntidades.Add(usuarioDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Edita um usuário com a confirmação da senha atual
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<UsuarioDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar os usuários é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarUsuario, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            UsuarioVo usuarioVo = new UsuarioVo();
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out usuarioVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar o usuário: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarUsuario, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!string.Equals(usuarioVo.Senha, requisicaoDto.EntidadeDto.SenhaAntiga))
            {
                retornoDto.Mensagem = "A senha atual informada não está correta.";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarUsuario, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref usuarioVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter o usuário para Vo: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarUsuario, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }


            if (!EditarBd(usuarioVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao salvar os novos dados do usuário: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarUsuario, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarUsuario, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        public bool ObterListaParaSelecao(BaseRequisicaoDto requisicaoDto, ref IDictionary<Guid, string> listaUsuarios, ref string mensagemErro)
        {
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaUsuariosParaSelecao, Guid.Empty, mensagemErro);
                return false;
            }

            // Obter a query primária
            IQueryable<UsuarioVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                mensagemErro = $"Erro ao obter a query: {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaUsuario, Guid.Empty, mensagemErro);
                return false;
            }

            listaUsuarios = query.Where(p => p.Excluido == false).Select(p => new { p.Id, p.Nome }).AsEnumerable().ToDictionary(p => p.Id, p => p.Nome);
            return true;
        }
    }
}
