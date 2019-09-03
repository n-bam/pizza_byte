using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace PizzaByteBll.Base
{
    public class UtilitarioBll
    {
        /// <summary>
        /// Criptografa a senha dos usuários
        /// </summary>
        /// <param name="senha"></param>
        /// <param name="senhaCriptografada"></param>
        public static void CriptografarSenha(string senha, ref string senhaCriptografada)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(senha + ChaveCriptografia()));
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                senhaCriptografada = sBuilder.ToString();
            }
        }

        /// <summary>
        /// Criptografa uma string para enviar nas requisições
        /// </summary>
        /// <param name="descriptografado"></param>
        /// <param name="criptografado"></param>
        /// <returns></returns>
        internal static bool CriptografarString(string descriptografado, ref string criptografado)
        {
            byte[] Results;
            UTF8Encoding UTF8 = new UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(ChaveCriptografia()));

            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            byte[] DataToEncrypt = UTF8.GetBytes(descriptografado);
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            criptografado = Convert.ToBase64String(Results);
            return true;
        }

        /// <summary>
        /// Decriptografa uma string da requisição
        /// </summary>
        /// <param name="criptografado"></param>
        /// <param name="descriotografado"></param>
        /// <returns></returns>
        internal static bool DescriptografarString(string criptografado, ref string descriotografado)
        {
            byte[] Results;
            UTF8Encoding UTF8 = new UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(ChaveCriptografia()));

            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            byte[] DataToDecrypt = Convert.FromBase64String(criptografado);

            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            descriotografado = UTF8.GetString(Results);
            return true;
        }

        /// <summary>
        /// Valida a identificação recebida das requisições e retorna se o usuário é administrador
        /// </summary>
        /// <param name="identificacao"></param>
        /// <param name="idUsuario"></param>
        /// <param name="usuarioAdm"></param>
        /// <param name="mensagemRetorno"></param>
        /// <returns></returns>
        internal static bool ValidarUsuarioAdm(string identificacao, ref string mensagemRetorno)
        {
            string descriptografado = "";
            if (!DescriptografarString(identificacao, ref descriptografado))
            {
                mensagemRetorno = "A identificação da requisição é inválida.";
                return false;
            }

            // Se tiver a configuração de ADM
            if (descriptografado.Contains("Adm="))
            {
                bool adm;
                string indicadorAdm = descriptografado.Substring(descriptografado.LastIndexOf("Adm=") + 4, 1);

                if (!bool.TryParse((indicadorAdm == "0" ? "false" : "true"), out adm))
                {
                    return false;
                }

                // Retornar se o usuário é adm
                return adm;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Valida a identificação recebida das requisições
        /// </summary>
        /// <param name="identificacao"></param>
        /// <param name="idUsuario"></param>
        /// <param name="mensagemRetorno"></param>
        /// <returns></returns>
        internal static bool ValidarIdentificacao(string identificacao, Guid idUsuario, ref string mensagemRetorno)
        {
            string descriptografado = "";
            if (!DescriptografarString(identificacao, ref descriptografado))
            {
                mensagemRetorno = "Acesso  negado: a identificação da requisição é inválida.";
                return false;
            }

            DateTime dataRequisicao;

            try
            {
                string dataGeracao = descriptografado.Substring(0, 16);
                dataRequisicao = Convert.ToDateTime(dataGeracao);
            }
            catch (Exception)
            {
                mensagemRetorno = "Problemas para converter a data de geração da identificação.";
                return false;
            }

            //Se for anterior a 5 dias e maior que a data atual
            if (dataRequisicao < DateTime.Now.AddDays(-5) || dataRequisicao > DateTime.Now)
            {
                mensagemRetorno = "A data da identificação é inválida.";
                return false;
            }

            //Validar o guid 
            string idRequisicao = descriptografado.Substring(16, 36);
            if (idRequisicao.ToUpper() != RetornaGuidValidação().ToString().ToUpper())
            {
                mensagemRetorno = "O digito verificador é inválido.";
                return false;
            }

            //Validar o usuário
            idRequisicao = descriptografado.Substring(52, 36);
            if (idRequisicao.ToUpper() != idUsuario.ToString().ToUpper())
            {
                mensagemRetorno = "O usuário da requisição é diferente da identificação.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Guid de validação das requisições
        /// </summary>
        /// <returns></returns>
        internal static string RetornaGuidValidação()
        {
            return "06DDEE6F-30A9-43CF-A906-802A7C7AF110";
        }

        /// <summary>
        /// Retorna a chave para criptografia e descriptografia
        /// </summary>
        /// <returns></returns>
        private static string ChaveCriptografia()
        {
            return "huxdribarmirnat";
        }

        /// <summary>
        /// Envia um email para o destinatário informado
        /// </summary>
        /// <param name="destinatario"></param>
        /// <param name="assunto"></param>
        /// <param name="corpo"></param>
        /// <returns></returns>
        internal static bool EnviarEmail(string destinatario, string assunto, string corpo, ref string mensagemErro)
        {
            try
            {
                MailMessage mail = new MailMessage("pizzarianacoesbsi@gmail.com", destinatario, assunto, corpo);
                mail.BodyEncoding = Encoding.UTF8;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("pizzarianacoesbsi@gmail.com", "unis@l123");
                client.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao enviar o e-mail: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Retorna o id fixo do usuário de suporte
        /// </summary>
        /// <returns></returns>
        internal static Guid RetornarIdUsuarioSuporte()
        {
            return new Guid("F93C1FC2-A836-4FC7-A061-C48806DD0F69");
        }

    }
}
