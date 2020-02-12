using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using PizzaByteDal;
using PizzaByteDto.ClassesBase;
using PizzaByteDto.RetornosRequisicoes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteBll.Base
{
    public class RecursosBdBll
    {
        private static LogBll logBll = new LogBll("RecursosBdBll");

        /// <summary>
        /// Executa o backup do sistema e retorna um arquivo .sql
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool FazerBackupSistema(BaseRequisicaoDto requisicaoDto, ref RetornoObterArquivoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.BaseIncluir, Guid.Empty, mensagemErro);
                return false;
            }

            PizzaByteContexto context = new PizzaByteContexto();

            try
            {
                StringBuilder deletes = new StringBuilder(string.Empty);
                StringBuilder inserts = new StringBuilder(string.Empty);
                StringBuilder insertsFk = new StringBuilder(string.Empty);

                ScriptingOptions scriptOptions = new ScriptingOptions();
                SqlConnection conn = new SqlConnection(context.Database.Connection.ConnectionString);
                Server srv1 = new Server(new ServerConnection(conn));
                Database db1 = srv1.Databases[context.Database.Connection.Database];

                deletes.AppendLine("Use " + context.Database.Connection.Database);
                deletes.AppendLine("GO ");
                deletes.AppendLine("BEGIN TRY ");
                deletes.AppendLine("BEGIN TRANSACTION ");

                List<string> tabelasComChaveEstrangeira = new List<string>();

                Scripter scr = new Scripter(srv1);
                foreach (Table table in db1.Tables)
                {
                    for (int i = 0; i < table.ForeignKeys.Count; i++)
                    {
                        if (!tabelasComChaveEstrangeira.Contains(table.ForeignKeys[i].ReferencedTable))
                        {
                            if (table.ForeignKeys[i].ReferencedTable == "Pedidos")
                            {
                                tabelasComChaveEstrangeira.Insert(0, table.ForeignKeys[i].ReferencedTable);
                            }
                            else
                            {
                                tabelasComChaveEstrangeira.Add(table.ForeignKeys[i].ReferencedTable);
                            }
                        }
                    }
                }

                // Tabelas que não fazer a fk de outras tabelas
                foreach (Table table in db1.Tables)
                {
                    if (table.Name != "__MigrationHistory" && !tabelasComChaveEstrangeira.Contains(table.Name))
                    {
                        ScriptingOptions options = new ScriptingOptions();
                        options.DriAll = false;
                        options.ScriptSchema = false;
                        options.ScriptData = true;
                        scr.Options = options;

                        deletes.AppendLine("DELETE FROM [PizzaByte].[" + table.Name + "]");

                        // Add script to file content 
                        foreach (string scriptLine in scr.EnumScript(new Urn[] { table.Urn }))
                        {
                            string line = scriptLine;
                            line = line.Replace("SET ANSI_NULLS ON", string.Empty);
                            line = line.Replace("SET QUOTED_IDENTIFIER ON", string.Empty);
                            line = line.Replace("SET ANSI_NULLS OFF", string.Empty);
                            line = line.Replace("SET QUOTED_IDENTIFIER OFF", string.Empty);
                            inserts.AppendLine(line.Trim());
                        }
                    }
                }

                deletes.AppendLine("DELETE FROM [PizzaByte].[Pedidos]");

                // Depois as tabelas que são fk
                foreach (Table table in db1.Tables)
                {
                    if (table.Name != "__MigrationHistory" && tabelasComChaveEstrangeira.Contains(table.Name))
                    {
                        ScriptingOptions options = new ScriptingOptions();
                        options.DriAll = false;
                        options.ScriptSchema = false;
                        options.ScriptData = true;
                        scr.Options = options;

                        if (table.Name != "Pedidos")
                        {
                            deletes.AppendLine("DELETE FROM [PizzaByte].[" + table.Name + "]");
                        }

                        // Add script to file content 
                        foreach (string scriptLine in scr.EnumScript(new Urn[] { table.Urn }))
                        {
                            string line = scriptLine;
                            line = line.Replace("SET ANSI_NULLS ON", string.Empty);
                            line = line.Replace("SET QUOTED_IDENTIFIER ON", string.Empty);
                            line = line.Replace("SET ANSI_NULLS OFF", string.Empty);
                            line = line.Replace("SET QUOTED_IDENTIFIER OFF", string.Empty);
                            insertsFk.AppendLine(line.Trim());
                        }
                    }
                }

                deletes.Append(insertsFk.ToString());
                deletes.Append(inserts.ToString());
                deletes.AppendLine("COMMIT TRAN ");
                deletes.AppendLine("END TRY ");
                deletes.AppendLine("BEGIN CATCH ");
                deletes.AppendLine("ROLLBACK TRAN ");
                deletes.AppendLine("END CATCH ");

                string backupCriptografado = "";
                if (!UtilitarioBll.CriptografarString(deletes.ToString(), ref backupCriptografado))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Erro ao criptografar o backup";
                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.Backup, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }

                byte[] bytes = Encoding.UTF8.GetBytes(backupCriptografado);
                string base64 = Convert.ToBase64String(bytes);

                retornoDto.ArquivoBase64 = base64;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Erro ao executar o backup: " + ex.Message;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.Backup, Guid.Empty, retornoDto.Mensagem);
                return false;
            }
        }

        /// <summary>
        /// Restaura o banco de dados a partir do backup
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool RestaurarBackup(RequisicaoArquivoDto requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RestaurarBackup, Guid.Empty, mensagemErro);
                return false;
            }

            try
            {
                PizzaByteContexto context = new PizzaByteContexto();
                Byte[] bytes = Convert.FromBase64String(requisicaoDto.ArquivoBase64);

                string comandosSql = Encoding.UTF8.GetString(bytes);

                string backupDescriptografado = "";
                if (!UtilitarioBll.DescriptografarString(comandosSql, ref backupDescriptografado))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Erro ao criptografar o backup";
                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.RestaurarBackup, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }

                using (SqlConnection conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    Server server = new Server(new ServerConnection(conn));
                    if (server.ConnectionContext.ExecuteNonQuery(backupDescriptografado) < 0)
                    {
                        retornoDto.Mensagem = "Falha ao restaurar o backup: ";

                        retornoDto.Retorno = false;
                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.RestaurarBackup, Guid.Empty, retornoDto.Mensagem);
                        return false;
                    }
                }
                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Mensagem = "Falha ao restaurar o backup: " + ex.Message;

                retornoDto.Retorno = false;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RestaurarBackup, Guid.Empty, retornoDto.Mensagem);
                return false;
            }
        }
    }
}
